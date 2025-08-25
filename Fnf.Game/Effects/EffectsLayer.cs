using Fnf.Framework;
using System.Collections.Generic;

namespace Fnf.Game
{
    public class EffectsLayer : IUpdatable, IRenderable
    {
        public bool isUpdatable { get; set; } = true;
        public bool isRenderable { get; set; } = true;
        
        internal List<Effect> currentEffects = new List<Effect>();
        internal List<Effect> removalQueue = new List<Effect>();

        public void Add(Effect effect)
        {
            currentEffects.Add(effect);
        }

        public void Clear()
        {
            removalQueue.AddRange(currentEffects);
        }

        public void Update()
        {
            // Remove the deactivated effects first
            for (int i = 0; i < currentEffects.Count; i++)
            {
                if (currentEffects[i].isFinished())
                {
                    removalQueue.Add(currentEffects[i]);
                }
            }

            for (int i = 0; i < removalQueue.Count; i++)
            {
                currentEffects.Remove(removalQueue[i]);
            }

            removalQueue.Clear();

            // Update the effects
            for (int i = 0; i < currentEffects.Count; i++)
            {
                if(currentEffects[i] is IUpdatable effect)
                {
                    effect.Update();
                }
            }
        }

        public void Render()
        {
            for (int i = 0; i < currentEffects.Count; i++)
            {
                if (currentEffects[i] is IRenderable effect)
                {
                    effect.Render();
                }
            }
        }
    }

    public abstract class Effect
    {
        protected float cooldown;  // Time until the effect is shown
        protected float duration;  // The duration the effect will last
        protected float remaining; // The remaining time of the effect

        public abstract bool isFinished();
    }
}