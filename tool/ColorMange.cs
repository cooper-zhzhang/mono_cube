using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace tool
{
    public class ColorHelper
    {
         public static Color GetColorFromChar(char c)
        {
            switch (c)
            {
                case 'U': return Color.White;
                case 'D': return Color.Yellow;
                case 'F': return Color.Red;
                case 'B': return Color.Orange;
                case 'L': return Color.Green;
                case 'R': return Color.Blue;
                default: return Color.Black;
            }
        }

        public static Color GetDefaultColor()
        {
            return Color.Black;
        }
    }
}