using System;
using C_Double_Flat.Core.Utilities;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Numerics;
using C_Double_Flat.Graphics.Structs;
namespace C_Double_Flat.Graphics
{
    public class Graphics : ILoadable
    {
        internal static readonly string CurrentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        internal static readonly string RaylibPath = Path.Combine(CurrentDir, "./raylib.dll");

        internal static Dictionary<int, Image> Images;
        
        public List<CustomFunction> GetFunctions()
        {
            C_Double_Flat.Core.Runtime.Interpreter.GetFunction("disp_echo").Run(new() { new ValueVariable("Loading CBB Raylib Graphics") });
            return new()
            {
                // Gets readyness of graphics lib
                new("graphics_loaded", args =>
                {
                    try
                    {
                        Interop.InitWindow(1,1,"");
                        Interop.CloseWindow();
                        return new ValueVariable(true);
                    }
                    catch
                    {
                        return new ValueVariable(false);
                    }
                }),
                // Entry point for graphics lib, required to run for any function to work given a Width, Height, and Title.
                new("graphics_init", args =>
                {
                    var width = args.Count < 1 ? 100 : Convert.ToInt32(args[0].AsDouble());
                    var height = args.Count < 2 ? 100 : Convert.ToInt32(args[1].AsDouble());
                    var title = args.Count < 3 ? "" : args[2].AsString();

                    Interop.InitWindow(width,height,title);
                    return ValueVariable.Default;
                }),
                // Sets size of window by Width, and Height
                new("graphics_set_size", args =>
                {
                    var width = args.Count < 1 ? 100 : Convert.ToInt32(args[0].AsDouble());
                    var height = args.Count < 2 ? 100 : Convert.ToInt32(args[1].AsDouble());
                    Interop.SetWindowSize(width,height);
                    return ValueVariable.Default;
                }),
                // Closes window
                new ("graphics_close", args =>
                {
                    Interop.CloseWindow();
                    return ValueVariable.Default;
                }),
                // Toggles fullscreen mode
                new ("graphics_fullscreen", args =>
                {
                    Interop.ToggleFullscreen();
                    return ValueVariable.Default;
                }),
                // Gets window width
                new ("graphics_width", args =>
                {
                    return new ValueVariable(Interop.GetScreenWidth());
                }),
                // Gets window height
                new ("graphics_height", args =>
                {
                    return new ValueVariable(Interop.GetScreenHeight());
                }),
                // Gets whether the window is in closeable state
                new("graphics_should_close", args =>
                {
                    return new ValueVariable(Interop.WindowShouldClose().AsBool());
                }),
                // Begins 2D drawing mode
                new ("graphics_start_draw", args =>
                {
                  Interop.BeginDrawing();
                  return ValueVariable.Default;
                }),
                // Ends 2D drawing mode
                new ("graphics_end_draw", args =>
                {
                  Interop.EndDrawing();
                  return ValueVariable.Default;
                }),
                // Clears background with color (Either a string corresponding to the color, or a list of 4 numbers R.G.B.A)
                new ("graphics_clear_bg", args =>
                {
                  Interop.ClearBackground(args.Count < 1 ? Color.Blank : args[0].AsColor());
                  return ValueVariable.Default;
                }),
                
                // Sets pixel at vector (list of 2 numbers) to color (Either a string corresponding to the color, or a list of 4 numbers R.G.B.A)
                new ("graphics_draw_pixel", args =>
                {
                    if (args.Count < 2)
                        return ValueVariable.Default;
                    Interop.DrawPixelV(args[0].AsVector2(), args[1].AsColor());
                    return ValueVariable.Default;
                }),
                // Draws line from two vectors (list of 2 numbers) to color (Either a string corresponding to the color, or a list of 4 numbers R.G.B.A)
                // Or from two vectors (list of 2 numbers) at a thickness (number) to color (Either a string corresponding to the color, or a list of 4 numbers R.G.B.A) 
                new ("graphics_draw_line", args =>
                {
                    if (args.Count < 3)
                        return ValueVariable.Default;
                    if (args.Count < 4)
                        Interop.DrawLineV(args[0].AsVector2(), args[1].AsVector2(), args[2].AsColor());
                    else
                        Interop.DrawLineEx(args[0].AsVector2(), args[1].AsVector2(), (float)args[2].AsDouble(), args[3].AsColor());
                    return ValueVariable.Default;
                }),
                // Draws filled rectangle from vector (list of 2 numbers), width, height, to a color (Either a string corresponding to the color, or a list of 4 numbers R.G.B.A)
                new ("graphics_fill_rect", args =>
                {
                    if (args.Count < 3)
                        return ValueVariable.Default;
                    var location = args[0].AsVector2();
                    Interop.DrawRectangle((int)location.X, (int)location.Y, (int)args[1].AsDouble(), (int)args[2].AsDouble(), args[3].AsColor());
                    return ValueVariable.Default;
                }),
                // Draws outline of rectangle from vector (list of 2 numbers), width, height, to a color (Either a string corresponding to the color, or a list of 4 numbers R.G.B.A)
                new("graphics_outline_rect", args => 
                {
                    if (args.Count < 3)
                        return ValueVariable.Default;
                    var location = args[0].AsVector2();
                    Interop.DrawRectangleLines((int)location.X, (int)location.Y, (int)args[1].AsDouble(), (int)args[2].AsDouble(), args[3].AsColor());
                    return ValueVariable.Default;                    
                }),
                // Draws filled ellipse from center vector (list of 2 numbers) size of horizontal axis, then vertical, to color (Either a string corresponding to the color, or a list of 4 numbers R.G.B.A)
                new ("graphics_fill_ellipse", args => 
                {
                    if (args.Count < 4)
                        return ValueVariable.Default;
                    var location = args[0].AsVector2();

                    Interop.DrawEllipse((int)location.X, (int)location.Y, (float)args[1].AsDouble(), (float)args[2].AsDouble(), args[3].AsColor());
                    return ValueVariable.Default;
                }),
                // Draws outline of ellipse from center vector (list of 2 numbers) size of horizontal axis, then vertical, to color (Either a string corresponding to the color, or a list of 4 numbers R.G.B.A)
                new ("graphics_outline_ellipse", args =>
                {
                    if (args.Count < 4)
                        return ValueVariable.Default;
                    var location = args[0].AsVector2();

                    Interop.DrawEllipse((int)location.X, (int)location.Y, (float)args[1].AsDouble(), (float)args[2].AsDouble(), args[3].AsColor());
                    return ValueVariable.Default;
                }),
                // Draws fills a polygon from center vector (list of 2 numbers) number of sides, radius, and initial rotation, to color (Either a string corresponding to the color, or a list of 4 numbers R.G.B.A)
                new("graphics_fill_polygon", args =>
                {
                   if (args.Count < 5)
                        return ValueVariable.Default;
                   Interop.DrawPoly(args[0].AsVector2(), (int)args[1].AsDouble(),(int)args[2].AsDouble(),(int)args[3].AsDouble(),args[4].AsColor());
                   return ValueVariable.Default; 
                }),
                // Draws fills a polygon from center vector (list of 2 numbers) number of sides, radius, initial rotation, line thickness, to color (Either a string corresponding to the color, or a list of 4 numbers R.G.B.A)
                new("graphics_outline_polygon", args =>
                {
                   if (args.Count < 6)
                        return ValueVariable.Default;
                   Interop.DrawPolyLinesEx(args[0].AsVector2(), (int)args[1].AsDouble(),(int)args[2].AsDouble(),(float)args[3].AsDouble(),(float)args[4].AsDouble(),args[5].AsColor());
                   return ValueVariable.Default;
                }),
                
                // Gets clipboard contents
                new ("clipboard_get", args => {
                    return new ValueVariable(Interop.GetClipboardText());
                }),
                // Inserts clipboard contents
                new ("clipboard_set", args => {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    Interop.SetClipboardText(args[0].AsString());
                    return ValueVariable.Default;
                }),
            };
        }

    }
}
