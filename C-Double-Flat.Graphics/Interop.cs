using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;
using C_Double_Flat.Graphics.Structs;
namespace C_Double_Flat.Graphics
{
    /// <summary>
    /// This class is largely inspired by what Raylib-cs already does, but it is modified for my needs.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    unsafe internal class Interop
    {
        // Flags
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetTraceLogLevel(TraceLogLevel logLevel);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte IsWindowState(ConfigFlags flag);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetConfigFlags(ConfigFlags flags);        
        
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

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowIcon(Image image);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowTitle(string title);


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
        public static extern void DrawRectangleLines(int posX, int posY, int width, int height, Color color);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawEllipse(int centerX, int centerY, float radiusH, float radiusV, Color color);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawEllipseLines(int centerX, int centerY, float radiusH, float radiusV, Color color);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawPoly(Vector2 center, int sides, float radius, float rotation, Color color);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawPolyLinesEx(Vector2 center, int sides, float radius, float rotation, float lineThick, Color color);

        // Input

        // Fonts and Text
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern Font LoadFont(string fileName);
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void UnloadFont(Font font);
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawFPS(int posX, int posY);
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawText(string text, int posX, int posY, int fontSize, Color color);
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawTextEx(Font font, string text, Vector2 position, float fontSize, float spacing, Color tint);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern Vector2 MeasureTextEx(Font font, string text, float fontSize, float spacing);

        // Images

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern Image LoadImage(string fileName);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern Texture2D LoadTexture(string fileName);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void UnloadTexture(Texture2D texture);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern Texture2D LoadTextureFromImage(Image image);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawTextureV(Texture2D texture, Vector2 position, Color tint);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawTextureEx(Texture2D texture, Vector2 position, float rotation, float scale, Color tint);

        // Misc.

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern string GetClipboardText();
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern string SetClipboardText(string text);
      
    }

}
