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

        [DllImport("raylib",CallingConvention = CallingConvention.Cdecl)]
        public static extern void MaximizeWindow();
        
        [DllImport("raylib",CallingConvention = CallingConvention.Cdecl)]
        public static extern void MinimizeWindow();
        
        [DllImport("raylib",CallingConvention = CallingConvention.Cdecl)]
        public static extern void RestoreWindow();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte WindowShouldClose();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetScreenWidth();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetScreenHeight();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMonitorCount();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetCurrentMonitor();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMonitorWidth(int monitor);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMonitorHeight(int monitor);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMonitorRefreshRate(int monitor);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern Vector2 GetWindowPosition();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowIcon(Image image);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowTitle(string title);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowPosition(int x, int y);

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


        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte IsKeyPressed(KeyboardKey key);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte IsKeyDown(KeyboardKey key);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte IsKeyReleased(KeyboardKey key);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetKeyPressed();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetCharPressed();


        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte IsMouseButtonPressed(MouseButton button);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte IsMouseButtonDown(MouseButton button);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte IsMouseButtonReleased(MouseButton button);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte IsMouseButtonUp(MouseButton button);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern Vector2 GetMousePosition();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMousePosition(int x, int y);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetMouseWheelMove();
        
        
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
        public static extern void UnloadImage(Image image);

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

        // Audio

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitAudioDevice();
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CloseAudioDevice();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sound LoadSound(string fileName);
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void UnloadSound(Sound sound);
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PlaySoundMulti(Sound sound);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte IsSoundPlaying(Sound sound);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopSoundMulti(Sound sound);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PlaySound(Sound sound);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopSound(Sound sound);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PauseSound(Sound sound);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ResumeSound(Sound sound);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSoundVolume(Sound sound, float volume);

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSoundPitch(Sound sound, float pitch);

        // Misc.

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetTargetFPS(int fps);
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFPS();
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetFrameTime();

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern string GetClipboardText();
        
        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        public static extern string SetClipboardText(string text);
      
    }

}
