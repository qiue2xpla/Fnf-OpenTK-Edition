using Fnf.Framework.Audio;
using Fnf.Framework;
using Fnf.Game;
using System;

using Fnf.Framework.TrueType;
using System.IO;
using System.Collections.Generic;

namespace Fnf
{
    class Program
    {
        static void Main()
        {
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