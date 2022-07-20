using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C_Double_Flat.Core.Utilities;
using System.Numerics;
namespace C_Double_Flat.Graphics
{
    internal static class Extensions
    {
        internal static bool AsBool(this sbyte b)
        {
            return b != 0;
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
