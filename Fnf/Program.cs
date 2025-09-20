using Fnf.Framework.Audio;
using Fnf.Framework;
using Fnf.Game;
using System.Runtime.Remoting.Messaging;

namespace Fnf
{
    class Program
    {
        static void Main()
        {
            Vector3 testVector = new Vector3(5, 10, 1);

            Matrix3x3 translation = Matrix3x3.CreateTranslationMatrix(new Vector2(-2, 5));
            Matrix3x3 scale = Matrix3x3.CreateScaleMatrix(new Vector2(-1, 2));
            Matrix3x3 rotation = Matrix3x3.CreateRotationMatrix(MathUtility.ToRadian(-90));

            Vector3 result = translation * rotation * scale * testVector;

            /*Script.AssignStartupScript<Intro>();
            ClipsManager.AppVolume = 0.1f;

            Window.Initiate();

            Window.IsGridFixed = true;
            Window.WindowSize = Window.GridSize * 0.9f;
            Window.Title = "Friday Night Funkin";

            SharedGameSystems.InitiateSystens();

            Window.Run();*/
        }
    }
}