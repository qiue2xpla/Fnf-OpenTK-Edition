namespace Fnf.Game
{
    public static class SharedGameSystems
    {
        public static VolumeControl VolumeControl;

        public static void InitiateSystens()
        {
            VolumeControl = new VolumeControl();
        }
    }
}