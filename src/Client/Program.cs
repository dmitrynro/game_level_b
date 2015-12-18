using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Client.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Shared;
using Shared.Ini;

namespace Client
{
    public static class Program
    {
        public static GameWindow Window;
        public static Renderer Renderer;
        public static NetClient NetClient;
        public static ContentManager Content;

        public static List<GameObject> GameObjects = new List<GameObject>();
        public static Dictionary<int, Player> Players = new Dictionary<int, Player>();

        private static void Main(string[] args)
        {
            using (Window = new GameWindow(800, 600, new GraphicsMode(new ColorFormat(32), 0, 0, 0, ColorFormat.Empty, 2, false), "Fag Game Client",
                GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible))
            {
                Vector2 susekaPosition = new Vector2(100, 100);

                Window.Load += (s, e) =>
                {
                    IniFile settings = IniParser.Parse(File.ReadAllLines("settings.ini"));

                    string host = "127.0.0.1";
                    string port = "10000";

                    settings.TryGetValue("ServerHost", out host);
                    settings.TryGetValue("ServerPort", out port);

                    Renderer = new Renderer();
                    NetClient = new NetClient(host, int.Parse(port));
                    Content = new ContentManager();

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
                    NetClient.ProcessMessages((client) =>
                    {
                        while (client.Available > 0)
                        {
                            Stream stream = client.GetStream();

                            switch ((NetMessageType)stream.ReadByte())
                            {
                                case NetMessageType.SetPosition:
                                    int id = stream.ReadInt32();
                                    Vector2 position = new Vector2(stream.ReadSingle(), stream.ReadSingle());

                                    if (!Players.ContainsKey(id))
                                    {
                                        Player player = new Player();
                                        Players.Add(id, player);
                                        GameObjects.Add(player);
                                    }

                                    Players[id].Position = position;
                                    break;
                            }
                        }
                    });

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

                        Stream stream = NetClient.TcpClient.GetStream();
                        stream.WriteByte((byte)NetMessageType.Move);
                        stream.Write(moveDirection.X);
                        stream.Write(moveDirection.Y);
                    }
                };

                Window.RenderFrame += (s, e) =>
                {
                    Window.MakeCurrent();

                    Renderer.Clear();
                    Renderer.BeginPass(Content.Get<ShaderProgram>("TextureShader"));

                    foreach (var gameObj in GameObjects)
                        gameObj.Draw();

                    Renderer.EndPass();

                    Window.SwapBuffers();
                };

                Window.Run();

                Content?.Dispose();
            }
        }
    }
}
