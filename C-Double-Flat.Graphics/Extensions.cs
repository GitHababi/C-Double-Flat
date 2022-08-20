using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C_Double_Flat.Core.Utilities;
using System.Numerics;
using C_Double_Flat.Graphics.Structs;
namespace C_Double_Flat.Graphics
{
    internal static class Extensions
    {
        internal static bool AsBool(this sbyte b)
        {
            return b != 0;
        }
        
        internal static ConfigFlags AsConfigFlag(this string str)
        {
            if (Enum.TryParse(typeof(ConfigFlags), str.ToUpper(), true, out object result))
            {
                return (ConfigFlags)result;
            }
            return 0x0;
        }

        internal static KeyboardKey AsKeyboardKey(this string str)
        {
            if (Enum.TryParse(typeof(KeyboardKey), str.ToUpper(), true, out object result))
            {
                return (KeyboardKey)result;
            }
            return 0x0;
        }

        internal static MouseButton AsMouseButton(this string str)
        {
            if (Enum.TryParse(typeof(MouseButton), str.ToUpper(), true, out object result))
            {
                return (MouseButton)result;
            }
            return 0x0;
        }
        
        internal static GamepadAxis AsGamepadAxis(this string str)
        {
            if (Enum.TryParse(typeof(GamepadAxis), str.ToUpper(), true, out object result))
            {
                return (GamepadAxis)result;
            }
            return 0x0;
        }

        internal static GamepadButton AsGamepadButton(this string str)
        {
            if (Enum.TryParse(typeof(GamepadButton), str.ToUpper(), true, out object result))
            {
                return (GamepadButton)result;
            }
            return 0x0;
        }

        internal static IVariable AsVariable(this Vector2 vector)
        {
            return new CollectionVariable(new IVariable[] { new ValueVariable(vector.X), new ValueVariable(vector.Y) });
        }
        internal static Vector2 AsVector2(this IVariable variable)
        {
            if (variable.Type() == VariableType.Value)
                return new((float)variable.AsDouble());
            CollectionVariable collection = (CollectionVariable)variable;
            return new(
                (float)collection.TryGetVariable(0).AsDouble(), // Yeah this is totally fine i guess
                (float)collection.TryGetVariable(1).AsDouble());
        }

        internal static Color AsColor(this IVariable variable)
        {
            if (variable.Type() == VariableType.Value)
                return variable.AsString().AsColor();
            CollectionVariable collection = (CollectionVariable)variable; 
            return new(
                (byte)(int)collection.TryGetVariable(0).AsDouble(), // Yeah this is totally fine i guess
                (byte)(int)collection.TryGetVariable(1).AsDouble(), 
                (byte)(int)collection.TryGetVariable(2).AsDouble(), 
                (byte)(int)collection.TryGetVariable(3).AsDouble());
        }
        internal static Color AsColor(this string str)
        {
            
            return str.ToLower().Trim() switch
            {

                "lightgray" => Color.LightGray,
                "gray" => Color.Gray,
                "darkgray" => Color.DarkGray,
                "yellow" => Color.Yellow,
                "gold" => Color.Gold,
                "orange" => Color.Orange,
                "pink" => Color.Pink,
                "red" => Color.Red,
                "maroon" => Color.Maroon,
                "green" => Color.Green,
                "lime" => Color.Lime,
                "darkgreen" => Color.DarkGreen,
                "skyblue" => Color.SkyBlue,
                "blue" => Color.Blue,
                "darkblue" => Color.DarkBlue,
                "purple" => Color.Purple,
                "violet" => Color.Violet,
                "darkpurple" => Color.DarkPurple,
                "beige" => Color.Beige,
                "brown" => Color.Brown,
                "darkbrown" => Color.DarkBrown,
                "white" => Color.White,
                "black" => Color.Black,
                "blank" => Color.Blank,
                "magenta" => Color.Magenta,
                "raywhite" => Color.RayWhite,
                _ => Color.White
            };
        }
    }
}
