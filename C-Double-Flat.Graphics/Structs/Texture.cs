/*
 * Code Shamelessly Stolen from Raylib-cs
 * https://github.com/ChrisDill/Raylib-cs
 * All credit goes to them.
 */

using System.Runtime.InteropServices;
namespace C_Double_Flat.Graphics.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Texture2D
    {
        /// <summary>
        /// OpenGL texture id
        /// </summary>
        public uint id;

        /// <summary>
        /// Texture base width
        /// </summary>
        public int width;

        /// <summary>
        /// Texture base height
        /// </summary>
        public int height;

        /// <summary>
        /// Mipmap levels, 1 by default
        /// </summary>
        public int mipmaps;

        /// <summary>
        /// Data format (PixelFormat type)
        /// </summary>
        public PixelFormat format;
    }
}
