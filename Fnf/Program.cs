using Fnf.Framework.Audio;
using Fnf.Framework;

namespace Fnf
{
    class Program
    {
        static void Main()
        {
            Script.AssignStartupScript<StoryMenu>();
            ClipsManager.AppVolume = 0.1f;

            Window.Initiate();

            Window.IsGridFixed = true;


            using (Window window = new Window())
            {
                // TODO: Make it make sense
                Window.isGridFixed = true;
                Window.WindowSize = Window.GridSize * 0.9f;
                window.Title = "Friday Night Funkin";
                window.Run();
            }
        }
    }
}