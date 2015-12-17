using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;

using BitmapData = System.Drawing.Imaging.BitmapData;
using GDIPixelFormat = System.Drawing.Imaging.PixelFormat;
using ImageLockMode = System.Drawing.Imaging.ImageLockMode;

namespace Client.OpenGL
{
    public sealed class Texture : GLObject
    {
        public Texture(Bitmap bitmap, bool repeat = true, bool createMipmap = true, bool linearFiltration = true) : base()
        {
            SetImage(bitmap, true);

            SetParameter(TextureParameterName.TextureBaseLevel, 0);
            SetParameter(TextureParameterName.TextureMaxLevel, 0);

            if (linearFiltration)
            {
                if (createMipmap)
                    SetParameter(TextureParameterName.TextureMinFilter, TextureMinFilter.LinearMipmapLinear);
                else
                    SetParameter(TextureParameterName.TextureMinFilter, TextureMinFilter.Linear);

                SetParameter(TextureParameterName.TextureMagFilter, TextureMagFilter.Linear);
            }
            else
            {
                SetParameter(TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest);
                SetParameter(TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest);
            }

            if (createMipmap)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            if (repeat)
            {
                SetParameter(TextureParameterName.TextureWrapS, TextureWrapMode.Repeat);
                SetParameter(TextureParameterName.TextureWrapT, TextureWrapMode.Repeat);
            }
            else
            {
                SetParameter(TextureParameterName.TextureWrapS, TextureWrapMode.ClampToBorder);
                SetParameter(TextureParameterName.TextureWrapT, TextureWrapMode.ClampToBorder);
            }
        }

        protected override void CreateHandle()
        {
            Handle = GL.GenTexture();
        }

        protected override void DeleteHandle()
        {
            GL.DeleteTexture(Handle);
        }

        public void Bind(int unit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public override void Bind()
        {
            Bind(0);
        }

        private static int NextPowerOf2(int value)
        {
            if (value <= 1)
                return 1;

            value -= 1;

            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;

            return value + 1;
        }

        public void SetImage(Bitmap bitmap, bool scaleToPowerOf2 = true)
        {
            Bind();

            using (var bitmapCopy = scaleToPowerOf2 ? new Bitmap(bitmap, NextPowerOf2(bitmap.Width), NextPowerOf2(bitmap.Height)) : (Bitmap)bitmap.Clone())
            {
                PixelFormat format;
                PixelInternalFormat internalFormat;
                PixelType type;

                switch (bitmapCopy.PixelFormat)
                {
                    case GDIPixelFormat.Format24bppRgb:
                        format = PixelFormat.Rgb;
                        internalFormat = PixelInternalFormat.Rgb;
                        type = PixelType.UnsignedByte;
                        break;

                    case GDIPixelFormat.Format32bppArgb:
                        format = PixelFormat.Bgra;
                        internalFormat = PixelInternalFormat.Rgba;
                        type = PixelType.UnsignedByte;
                        break;

                    default:
                        throw new ArgumentException("PixelFormat \"" + bitmapCopy.PixelFormat.ToString() + "\" not supported", "bitmap");
                }

                BitmapData data = bitmapCopy.LockBits(new Rectangle(0, 0, bitmapCopy.Width, bitmapCopy.Height), ImageLockMode.ReadOnly, bitmapCopy.PixelFormat);
                GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, bitmapCopy.Width, bitmapCopy.Height, 0, format, type, data.Scan0);
                bitmapCopy.UnlockBits(data);
            }
        }

        public void SetParameter(TextureParameterName parameter, int value)
        {
            Bind();
            GL.TexParameter(TextureTarget.Texture2D, parameter, value);
        }

        public void SetParameter(TextureParameterName parameter, object value)
        {
            SetParameter(parameter, (int)value);
        }
    }
}
