using System;
using C_Double_Flat.Core.Utilities;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Numerics;
using C_Double_Flat.Graphics.Structs;
using System.Threading;
namespace C_Double_Flat.Graphics
{
    public class Graphics : ILoadable
    {
        internal static readonly string CurrentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        internal static readonly string RaylibPath = Path.Combine(CurrentDir, "./raylib.dll");

        internal static Dictionary<Guid, Texture2D> Textures = new();
        internal static Dictionary<Guid, Font> Fonts = new();
        internal static Dictionary<Guid, Sound> Sounds = new();
        public List<CustomFunction> GetFunctions()
        {
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

                    Interop.SetTraceLogLevel(TraceLogLevel.LOG_NONE);
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
                // Sets the corner icon, DOESNT SUPPORT PNGs (idk why)
                new ("graphics_set_icon", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    Interop.SetWindowIcon(Interop.LoadImage(Path.GetFullPath(args[0].AsString())));
                    return ValueVariable.Default;
                }),
                new("graphics_set_title", args =>
                {
                    var title = args.Count < 1 ? "" : args[0].AsString();
                    Interop.SetWindowTitle(title);
                    return ValueVariable.Default;
                }),
                // Sets the windows position on screen
                new("graphics_set_position", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    var location = args[0].AsVector2();
                    Interop.SetWindowPosition((int)location.X, (int)location.Y);
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
                // Minimizes window
                new ("graphics_minimize", args =>
                {
                    Interop.MinimizeWindow();
                    return ValueVariable.Default;
                }),
                // Restores window
                new ("graphics_restore", args =>
                {
                    Interop.RestoreWindow();
                    return ValueVariable.Default;
                }),
                // Maximized window
                new ("graphics_maximize", args =>
                {
                    Interop.MaximizeWindow();
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
                // Gets windows position on screen
                new ("graphics_position", args =>
                {
                    return Interop.GetWindowPosition().AsVariable();
                }),
                // Gets the current monitor width
                new("graphics_monitor_width", args =>
                {
                    return new ValueVariable(Interop.GetMonitorWidth(Interop.GetCurrentMonitor()));
                }),
                // Gets the current monitor height
                new("graphics_monitor_height", args =>
                {
                    return new ValueVariable(Interop.GetMonitorHeight(Interop.GetCurrentMonitor()));
                }),
                // Gets the current monitor refresh rate
                new("graphics_monitor_refresh_rate", args =>
                {
                    return new ValueVariable(Interop.GetMonitorRefreshRate(Interop.GetCurrentMonitor()));
                }),
                // Gets whether the window is in closeable state
                new("graphics_should_close", args =>
                {
                    return new ValueVariable(Interop.WindowShouldClose().AsBool());
                }),
                // Clears background with color (Either a string corresponding to the color, or a list of 4 numbers R.G.B.A)
                new ("graphics_clear_bg", args =>
                {
                  Interop.ClearBackground(args.Count < 1 ? Color.Blank : args[0].AsColor());
                  return ValueVariable.Default;
                }),
                // Toggles flag described. MUST BE CALLED BEFORE graphics_init()
                new("graphics_set_flag", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    Interop.SetConfigFlags(args[0].AsString().AsConfigFlag());
                    return ValueVariable.Default;
                }),
                // Returns true if flag described is set.
                new("graphics_get_flag", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Interop.IsWindowState(args[0].AsString().AsConfigFlag()).AsBool());
                }),
                // Returns time since last frame in milliseconds
                new ("graphics_delta_time", args =>
                {
                    return new ValueVariable(Interop.GetFrameTime() * 1000);
                }),
                // Gets current frame rate
                new ("graphics_fps", args =>
                {
                    return new ValueVariable(Interop.GetFPS());
                }),
                // Set current target frame rate
                new ("graphics_set_fps", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    Interop.SetTargetFPS((int)args[0].AsDouble());
                    return ValueVariable.Default;
                }),

                // Begins 2D drawing mode
                new ("graphics_draw_start", args =>
                {
                  Interop.BeginDrawing();
                  return ValueVariable.Default;
                }),
                // Ends 2D drawing mode
                new ("graphics_draw_end", args =>
                {
                    Interop.EndDrawing();
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
                
                // Allocates a texture by given file path and returns its "address" (key to texture dictionary)
                new ("texture_allocate", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    var identifier = Guid.NewGuid();
                    Textures.Add(identifier,Interop.LoadTexture(Path.GetFullPath(args[0].AsString())));
                    return new ValueVariable(identifier.ToString());
                }),
                // Deallocates a texture by given "address" (key to texture dictionary) returns true if successful
                new ("texture_deallocate",args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    if (Guid.TryParse(args[0].AsString(), out var identifier))
                    {
                        Interop.UnloadTexture(Textures[identifier]);
                        Textures.Remove(identifier);
                        return new ValueVariable(true);
                    }
                    return new ValueVariable(false);
                }),         
                // Renders a texture by given "address" (key to texture dictionary) at vector (list of 2 numbers), with optional parameters for rotation and scale.
                new ("texture_render", args =>
                {
                    if (args.Count < 2)
                        return ValueVariable.Default;
                    bool successful = Guid.TryParse(args[0].AsString(), out var identifier);
                    if (successful && args.Count < 4)
                        Interop.DrawTextureV(Textures[identifier],args[1].AsVector2(),Color.White);
                    else if (successful)
                        Interop.DrawTextureEx(Textures[identifier], args[1].AsVector2(), (float)args[2].AsDouble(), (float)args[3].AsDouble(), Color.White);
                    return ValueVariable.Default;
                    

                }),
                
                // Allocates a font by given file path and returns its "address" (key to font dictionary)
                new ("font_allocate", args => 
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    var identifier = Guid.NewGuid();
                    Fonts.Add(identifier,Interop.LoadFont(Path.GetFullPath(args[0].AsString())));
                    return new ValueVariable(identifier.ToString());
                }),
                // Deallocates a font by given "address" (key to font dictionary) returns true if successful
                new ("font_deallocate",args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    if (Guid.TryParse(args[0].AsString(), out var identifier))
                    {
                        Interop.UnloadFont(Fonts[identifier]);
                        Fonts.Remove(identifier);
                        return new ValueVariable(true);
                    }
                    return new ValueVariable(false);
                }),
                // Draws a text by given "address" (key to font dictionary), text to draw, at vector (list of 2 numbers), size, spacing (in pixels), and color
                // Or text to draw, at vector, size, and color
                new ("graphics_draw_text", args =>
                {
                    if (args.Count < 4)  
                        return ValueVariable.Default;
                    if (args.Count < 6)
                        Interop.DrawText(args[0].AsString(),(int)args[1].AsVector2().X,(int)args[1].AsVector2().Y,(int)args[2].AsDouble(),args[3].AsColor());
                    bool successful = Guid.TryParse(args[0].AsString(), out var identifier);
                    if (successful)
                        Interop.DrawTextEx(Fonts[identifier], args[1].AsString(), args[2].AsVector2(), (float)args[3].AsDouble(), (float)args[4].AsDouble(), args[5].AsColor());
                    return ValueVariable.Default;
                }),
                // Draws fps counter at vector position
                new ("graphics_draw_fps", args =>
                {
                   if (args.Count < 1)
                        return ValueVariable.Default;
                   var location = args[0].AsVector2();
                   Interop.DrawFPS((int)location.X,(int)location.Y);
                   
                   return ValueVariable.Default;
                }),
                // Returns a vector (list of 2 numbers) of the size of the text as if it would be rendered with given "address" (key to font dictionary), text to draw, size, spacing (in pixels)
                new ("graphics_measure_text", args =>
                {
                    if (args.Count < 4)
                        return ValueVariable.Default;
                    bool successful = Guid.TryParse(args[0].AsString(), out var identifier);
                    return Interop.MeasureTextEx(Fonts[identifier],args[1].AsString(),(float)args[2].AsDouble(),(float)args[3].AsDouble()).AsVariable();
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

                // Initialize audio player
                new ("sound_init", args =>
                {
                    Interop.InitAudioDevice();
                    return ValueVariable.Default;
                }),
                // Close audio player
                new ("sound_close", args =>
                {
                    Interop.CloseAudioDevice();
                    return ValueVariable.Default;
                }),
                //Allocates a sound by given file path and returns its "address" (key to sound dictionary)
                new ("sound_allocate", args =>
                {
                   if (args.Count < 1)
                        return ValueVariable.Default;
                   var identifier = Guid.NewGuid();
                   Sounds.Add(identifier,Interop.LoadSound(Path.GetFullPath(args[0].AsString())));
                   return new ValueVariable(identifier.ToString());
                }),
                // Deallocates a sound by given "address" (key to sound dictionary) returns true if successful
                new ("sound_deallocate",args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    if (Guid.TryParse(args[0].AsString(), out var identifier))
                    {
                        Interop.UnloadSound(Sounds[identifier]);
                        Sounds.Remove(identifier);
                        return new ValueVariable(true);
                    }
                    return new ValueVariable(false);
                }),
                // Plays a sound by given "address" (key to sound dictionary)
                new("sound_play", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    if (Guid.TryParse(args[0].AsString(), out var identifier))
                    {
                        var sound = Sounds[identifier];
                        Interop.PlaySound(sound);
                    }
                    return ValueVariable.Default;
                }),
                // Stops a sound by given "address" (key to sound dictionary)
                new("sound_stop", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    if (Guid.TryParse(args[0].AsString(), out var identifier))
                    {
                        var sound = Sounds[identifier];
                        Interop.StopSound(sound);
                    }
                    return ValueVariable.Default;
                }),
                // Plays a sound in a multichannel environment by given "address" (key to sound dictionary)
                // Does not allow for pitch changes, nor volume changes, nor checking if sound is playing.
                new("sound_multichannel_play", args => 
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    if (Guid.TryParse(args[0].AsString(), out var identifier))
                    {
                        var sound = Sounds[identifier];
                        Interop.PlaySoundMulti(sound);
                    }
                    return ValueVariable.Default;
                }),
                // Stops a sound by given "address" (key to sound dictionary)
                new("sound_multichannel_stop", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    if (Guid.TryParse(args[0].AsString(), out var identifier))
                    {
                        var sound = Sounds[identifier];
                        Interop.StopSoundMulti(sound);
                    }
                    return ValueVariable.Default;
                }),
                // Plays a sound by given "address" (key to sound dictionary)
                new("sound_set_volume", args =>
                {
                    if (args.Count < 2)
                        return ValueVariable.Default;
                    if (Guid.TryParse(args[0].AsString(), out var identifier))
                    {
                        var sound = Sounds[identifier];
                        Interop.SetSoundVolume(sound,(float)args[1].AsDouble());
                    }
                    return ValueVariable.Default;
                }),
                // Plays a sound by given "address" (key to sound dictionary)
                new("sound_set_pitch", args =>
                {
                    if (args.Count < 2)
                        return ValueVariable.Default;
                    if (Guid.TryParse(args[0].AsString(), out var identifier))
                    {
                        var sound = Sounds[identifier];
                        Interop.SetSoundPitch(sound,(float)args[1].AsDouble());
                    }
                    return ValueVariable.Default;
                }),
                // Returns true if sound is playing by given "address" (key to sound dictionary)
                new("sound_playing", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    if (Guid.TryParse(args[0].AsString(), out var identifier))
                    {
                        var sound = Sounds[identifier];
                        return new ValueVariable(Interop.IsSoundPlaying(sound).AsBool());
                    }
                    return ValueVariable.Default;
                }),
                
                
                
                // Returns true if keyboard key in first arg is pressed
                new ("input_key_pressed", args => 
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Interop.IsKeyPressed(args[0].AsString().AsKeyboardKey()).AsBool());
                }),
                // Returns true if keyboard key in first arg is down
                new ("input_key_down", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Interop.IsKeyDown(args[0].AsString().AsKeyboardKey()).AsBool());
                }),
                // Returns true if keyboard key in first arg is released
                new ("input_key_released", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Interop.IsKeyReleased(args[0].AsString().AsKeyboardKey()).AsBool());
                }),
                // Returns the key that is currently down.
                new ("input_get_key", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(((KeyboardKey)Interop.GetKeyPressed()).ToString());
                }),
                // Returns the char that is currently down.
                new ("input_get_char", args =>
                {
                    var key = Interop.GetCharPressed();
                    return new ValueVariable(((char)key).ToString());
                }),
                // Returns true if mouse button in first arg is pressed
                 new ("input_mouse_pressed", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Interop.IsMouseButtonPressed(args[0].AsString().AsMouseButton()).AsBool());
                }),
                 // Returns true if mouse button in first arg is down
                new ("input_mouse_down", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Interop.IsMouseButtonDown(args[0].AsString().AsMouseButton()).AsBool());
                }),
                // Returns true if mouse button in first arg is released
                new ("input_mouse_released", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Interop.IsMouseButtonReleased(args[0].AsString().AsMouseButton()).AsBool());
                }),
                // Returns true if mouse button in first arg is up
                new ("input_mouse_up", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Interop.IsMouseButtonUp(args[0].AsString().AsMouseButton()).AsBool());
                }),
                // Returns location of mouse as vector
                new ("input_mouse_pos", args =>
                {
                    return Interop.GetMousePosition().AsVariable();
                }),
                // Sets the position of the mouse to vector on screen at position
                new ("input_mouse_set_pos", args =>
                {
                    if (args.Count < 1)
                        return ValueVariable.Default;
                    var location = args[0].AsVector2();
                    Interop.SetMousePosition((int)location.X,(int)location.Y);
                    return ValueVariable.Default;
                })
            };
        }

    }
}
