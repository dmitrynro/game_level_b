using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Client.GameObjects;
using Client.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Shared;
using Shared.GameObjects;
using Shared.Ini;

namespace Client
{
    public static class Program
    {
        public static GameWindow Window;
        public static Renderer Renderer;
        public static NetClient NetClient;
        public static ContentManager Content;

        public static int ClientId = 0;
        public static Dictionary<int, GameObject> GameObjects = new Dictionary<int, GameObject>();

        public static IEnumerable<ClientPlayer> Players { get { return GameObjects.Where(obj => obj.Value is ClientPlayer).Select(obj => obj.Value as ClientPlayer); } }

        private static void Main(string[] args)
        {
            using (Window = new GameWindow(800, 600, new GraphicsMode(new ColorFormat(32), 0, 0, 0, ColorFormat.Empty, 2, false), "Fag Game Client",
                GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible))
            {
                Window.Load += (s, e) =>
                {
                    IniFile settings = IniParser.Parse(File.ReadAllLines("settings.ini"));

                    Renderer = new Renderer();
                    NetClient = new NetClient();
                    Content = new ContentManager();

                    string host, port;

                    settings.TryGetValue("ServerHost", out host, "127.0.0.1");
                    settings.TryGetValue("ServerPort", out port, "10000");

                    NetClient.Connect(host, int.Parse(port));

                    NetClient.ReceivedData += (sender, connection) =>
                    {
                        while (connection.Available > 0)
                        {
                            Stream stream = connection.GetStream();

                            switch ((NetMessageType)stream.ReadByte())
                            {
                                case NetMessageType.ClientConnected:
                                    {
                                        int playerId = stream.ReadInt32();

                                        if (!GameObjects.ContainsKey(playerId))
                                        {
                                            ClientPlayer player = new ClientPlayer(playerId);
                                            GameObjects.Add(playerId, player);
                                        }
                                    }
                                    break;

                                case NetMessageType.ClientDisconnected:
                                    GameObjects.Remove(stream.ReadInt32());
                                    break;

                                case NetMessageType.SetClientId:
                                    ClientId = stream.ReadInt32();
                                    break;

                                case NetMessageType.SetPosition:
                                    {
                                        int playerId = stream.ReadInt32();
                                        Vector2 position = stream.ReadVector2();

                                        if (!GameObjects.ContainsKey(playerId))
                                        {
                                            ClientPlayer player = new ClientPlayer(playerId);
                                            GameObjects.Add(playerId, player);
                                        }

                                        GameObjects[playerId].Position = position;
                                    }
                                    break;

                                case NetMessageType.SetVelocity:
                                    {
                                        int playerId = stream.ReadInt32();
                                        Vector2 velocity = stream.ReadVector2();

                                        if (!GameObjects.ContainsKey(playerId))
                                        {
                                            ClientPlayer player = new ClientPlayer(playerId);
                                            GameObjects.Add(playerId, player);
                                        }

                                        GameObjects[playerId].Velocity = velocity;
                                    }
                                    break;
                            }
                        }
                    };

                    using (var vertexShader = new Shader(ShaderType.VertexShader, File.ReadAllText(Path.Combine("Data", "texture.vsh")), Console.WriteLine))
                    using (var fragmentShader = new Shader(ShaderType.FragmentShader, File.ReadAllText(Path.Combine("Data", "texture.psh")), Console.WriteLine))
                    {
                        ShaderProgram shader = new ShaderProgram(new[] { vertexShader, fragmentShader }, Console.WriteLine);
                        shader.SetUniform("Texture", 0);
                        Content.Add("TextureShader", shader);
                    }

                    using (var bitmap = new Bitmap(Path.Combine("Data", "suseka.png")))
                        Content.Add("SusekaTexture", new Texture(bitmap));
                };

                Window.Resize += (s, e) =>
                {
                    int w = Window.Width, h = Window.Height;
                    Renderer.Resize(w, h);
                    Content.Get<ShaderProgram>("TextureShader").SetUniform("Transform", Matrix4.CreateOrthographicOffCenter(0, w, h, 0, -1, 1));
                };

                Window.UpdateFrame += (s, e) =>
                {
                    NetClient.ProcessMessages();

                    foreach (var kv in GameObjects)
                        kv.Value.Update(1000 * (float)e.Time);

                    if (!Window.Focused)
                        return;

                    Vector2 moveDirection = Vector2.Zero;

                    KeyboardState kbdState = Keyboard.GetState();

                    if (kbdState.IsKeyDown(Key.A) || kbdState.IsKeyDown(Key.Left))
                        moveDirection -= Vector2.UnitX;

                    if (kbdState.IsKeyDown(Key.D) || kbdState.IsKeyDown(Key.Right))
                        moveDirection += Vector2.UnitX;

                    if (kbdState.IsKeyDown(Key.W) || kbdState.IsKeyDown(Key.Up))
                        moveDirection -= Vector2.UnitY;

                    if (kbdState.IsKeyDown(Key.S) || kbdState.IsKeyDown(Key.Down))
                        moveDirection += Vector2.UnitY;

                    if (moveDirection.LengthSquared > 0)
                    {
                        moveDirection.Normalize();

                        if (GameObjects.ContainsKey(ClientId))
                            GameObjects[ClientId].Velocity = 0.1f * moveDirection;

                        NetClient.SendMessage((byte)NetMessageType.SetMoveDirection, moveDirection);
                    }
                };

                Window.RenderFrame += (s, e) =>
                {
                    Window.MakeCurrent();

                    Renderer.Clear();
                    Renderer.BeginPass(Content.Get<ShaderProgram>("TextureShader"));

                    foreach (var kv in GameObjects)
                        if (kv.Value is IDrawable)
                            (kv.Value as IDrawable).Draw();

                    Renderer.EndPass();

                    Window.SwapBuffers();
                };

                Window.Run();

                NetClient?.Dispose();
                Content?.Dispose();
            }
        }
    }
}
