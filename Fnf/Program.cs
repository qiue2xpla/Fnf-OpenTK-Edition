using Fnf.Framework.Audio;
using Fnf.Framework;
using Fnf.Game;
using System;

using Fnf.Framework.TrueType;
using System.IO;
using System.Collections.Generic;
using Fnf.Framework.TrueType.Rasterization;

namespace Fnf
{
    class Program
    {
        static void Main()
        {
            Font font = new Font("arial.ttf", true);
            FontAtlas atlas = new FontAtlas(font, 2000, 3, 3, 0, FontAtlas.GetCustomCharset("ULPNS"));

            atlas.map.ToBitmap((f) => new Color(1,1,1,f)).Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\\Woah.png", System.Drawing.Imaging.ImageFormat.Png);
           

            Script.AssignStartupScript<Editor>();
            ClipsManager.AppVolume = 0.1f;

            Window.Initiate();

            Window.IsGridFixed = true;
            Window.WindowSize = Window.GridSize * 0.9f;
            Window.Title = "Friday Night Funkin";

            SharedGameSystems.InitiateSystens();

            Window.Run();
        }
    }
}