using Fnf.Framework;

namespace Fnf.Game
{
    public class Animation
    {
        public string name;
        public float frameRate = 30;
        public bool looped = false;
        public Vector2 offset;

        public int texture;
        public Frame[] frames;

        internal Animation(Frame[] frames, int texture)
        {
            this.frames = frames;
            this.texture = texture;
        }

        public override string ToString()
        {
            return name;
        }
    }
}