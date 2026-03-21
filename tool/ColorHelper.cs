using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace tool
{
    public class ColorHelper
    {


        public static Color White = Color.White;
        public static Color Yellow = Color.Yellow;
        public static Color Red = Color.Red;
        public static Color Orange = new Color(255, 100, 0); // 更饱和的橙色
        public static Color Green = Color.SeaGreen;
        public static Color Blue = Color.DodgerBlue;

         public static Color GetColorFromChar(char c)
        {
            switch (c)
            {
                case 'U': return White;
                case 'D': return Yellow;
                case 'F': return Red;
                case 'B': return Orange;
                case 'L': return Green;
                case 'R': return Blue;
                default: return Color.Black;
            }
        }

        public static Color GetDefaultColor()
        {
            return Color.Black;
        }

        public static char GetCharFromColor(Color color)
        {
            if (color == Color.White) return 'U';
            if (color == Color.Yellow) return 'D';
            if (color == Color.Red) return 'F';
            if (color == Color.Orange) return 'B';
            if (color == Color.Green) return 'L';
            if (color == Color.Blue) return 'R';
            return 'U';
        }
    }
}