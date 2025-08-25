namespace Fnf.Framework.Audio
{
    public struct VolumeLevel
    {
        public float left { get; }
        public float right { get; }
        public float avrage { get; }

        public VolumeLevel(float left, float right)
        {
            this.right = right;
            this.left = left;
            avrage = (right + left) / 2;
        }
    }
}