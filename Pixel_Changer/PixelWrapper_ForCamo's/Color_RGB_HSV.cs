using System;
using System.Collections.Generic;
using System.Drawing;

class Color_RGB_HSV
{
    public int R;
    public int G;
    public int B;

    public Color_RGB_HSV(int _r, int _g, int _b)
    {
        R = _r;
        G = _g;
        B = _b;
    }

    public double ColourDistance(Color match)
    {
        Color current = Color.FromArgb(R, G, B);
        int redDifference;
        int greenDifference;
        int blueDifference;

        redDifference = current.R - match.R;
        greenDifference = current.G - match.G;
        blueDifference = current.B - match.B;

        return redDifference * redDifference + greenDifference * greenDifference +
                               blueDifference * blueDifference;
    }

    public Color ReturnColor()
    {
        return Color.FromArgb(R, G, B);
    }
}
