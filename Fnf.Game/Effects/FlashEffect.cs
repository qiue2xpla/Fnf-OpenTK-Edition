using Fnf.Framework.Graphics;
using Fnf.Framework;

namespace Fnf.Game
{
    public class FlashEffect : Effect, IRenderable, IUpdatable
    {
        public bool isRenderable { get; set; } = true;
        public bool isUpdatable { get; set; } = true;

        Color color;
        float alpha;

		public FlashEffect(float cooldown, float duration, Color color)
		{
            this.cooldown = cooldown;
            this.duration = duration;
			this.color = color;

            remaining = duration;
            alpha = 0;
        }

        public void Update()
        {
			if (!isUpdatable) return;

			if (cooldown > 0)
			{
				alpha = 0;
				cooldown -= Time.deltaTime;
				return;
			}

			alpha = remaining / duration;
			remaining -= Time.deltaTime;
		}

        public void Render()
        {
			if (!isRenderable) return;
			if (remaining < 0) return;
			if (cooldown > 0) return;

			Texture.Use(OpenGL.NULL);

			OpenGL.BeginDrawing(DrawMode.Quads);
			OpenGL.Color4(color, alpha);
			OpenGL.Vertex2( 1,  1);
			OpenGL.Vertex2(-1,  1);
			OpenGL.Vertex2(-1, -1);
			OpenGL.Vertex2( 1, -1);
			OpenGL.EndDrawing();
		}

        public override bool isFinished()
        {
			return remaining <= 0;
        }
    }
}