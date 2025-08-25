namespace Fnf.Game
{
    public static class GlobalSystems
    {
        public static VolumeControl VolumeControl;

        static GlobalSystems()
        {
            VolumeControl = new VolumeControl();
        }
    }
}