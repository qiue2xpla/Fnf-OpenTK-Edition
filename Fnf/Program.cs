using Fnf.Framework.Audio;
using Fnf.Framework;

namespace Fnf
{
    class Program
    {
        static void Main()
        {
            Script.AssignStartupScript<Intro>();
            ClipsManager.AppVolume = 0.1f;

            Window.Initiate();

            Window.IsGridFixed = true;
            Window.WindowSize = Window.GridSize * 0.9f;
            Window.Title = "Friday Night Funkin";

            Window.Run();
        }
    }
}