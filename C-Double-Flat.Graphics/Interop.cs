using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;
namespace C_Double_Flat.Graphics
{
    /// <summary>
    /// This class is largely inspired by what Raylib-cs already does, but it is modified for my needs.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    unsafe internal class Interop
    {

        

        // Window Stuff (In no particular order)

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern void InitWindow(int width, int height, string title);
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetWindowSize(int width, int height);
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CloseWindow();
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ToggleFullscreen();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte WindowShouldClose();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetScreenWidth();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetScreenHeight();

        // Drawing Methods

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void BeginDrawing();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void EndDrawing();
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClearBackground(Color color);
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawPixelV(Vector2 position, Color color);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawLineV(Vector2 startPos, Vector2 endPos, Color color);
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawLineEx(Vector2 startPos, Vector2 endPos, float thick, Color color);

        // Shapes
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawRectangle(int posX, int posY, int width, int height, Color color);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawEllipse(int centerX, int centerY, float radiusH, float radiusV, Color color);
        
        // Misc.

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern string GetClipboardText();
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern string SetClipboardText(string text);
      
    }

}
