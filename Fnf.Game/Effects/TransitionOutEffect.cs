using Fnf.Framework.Graphics;
using Fnf.Framework;
using System;


namespace Fnf.Game
{
    public class TransitionOutEffect : Effect, IRenderable, IUpdatable
    {
        public bool isRenderable { get; set; } = true;
        public bool isUpdatable { get; set; } = true;

        Color color;
        event Action onEnd;
        float alpha;

        public TransitionOutEffect(float cooldown, float duration, Color color, Action callback = null)
        {
            this.cooldown = cooldown;
            this.duration = duration;
            this.color = color;
            
            onEnd = callback;
            remaining = duration;
            alpha = 1;
        }

        public void Update()
        {
            if (!isUpdatable) return;

            if (cooldown > 0)
            {
                alpha = 1;
                cooldown -= Time.deltaTime;
                return;
            }

            alpha = remaining / duration;
            remaining -= Time.deltaTime;

            if (alpha > 0 && remaining <= 0)
            {
                onEnd?.Invoke();
            }
        }

        public void Render()
        {
            if (!isRenderable) return;
            if (remaining < 0) return;
            if (cooldown > 0) return;

            Texture.Use(OpenGL.NULL);

            OpenGL.BeginDrawing(DrawMode.Quads);

            float offset = (1 - alpha) * 4;

            OpenGL.Color4(color, 1);
            OpenGL.Vertex2(1, 1 - offset);
            OpenGL.Vertex2(-1, 1 - offset);
            OpenGL.Vertex2(-1, -1 - offset);
            OpenGL.Vertex2(1, -1 - offset);

            OpenGL.Color4(color, 1);
            OpenGL.Vertex2(1, 1 - offset);
            OpenGL.Vertex2(-1, 1 - offset);

            OpenGL.Color4(0f, 0f, 0f, 0f);
            OpenGL.Vertex2(-1, 3 - offset);
            OpenGL.Vertex2(1, 3 - offset);

            OpenGL.EndDrawing();
        }

        public override bool isFinished()
        {
            return remaining <= 0;
        }
    }
}