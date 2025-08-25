using Fnf.Framework;
using System.Collections.Generic;

namespace Fnf.Game
{
    public static class EffectsManager
    {
        internal static List<Effect> currentEffects = new List<Effect>();
        internal static List<Effect> removalQueue = new List<Effect>();

        public static void Clear()
        {
            removalQueue.AddRange(currentEffects);
            currentEffects.Clear();
        }

        public static void Update()
        {
            // Remove the deactivated effects first
            for (int i = 0; i < removalQueue.Count; i++)
            {
                currentEffects.Remove(removalQueue[i]);
            }
            if(removalQueue.Count > 0) removalQueue.Clear();

            // Update the effects
            for (int i = 0; i < currentEffects.Count; i++)
            {
                if(currentEffects[i] is IUpdatable effect)
                {
                    effect.Update();
                }
            }
        }

        public static void Render()
        {
            // Render the effects
            for (int i = 0; i < currentEffects.Count; i++)
            {
                if (currentEffects[i] is IRenderable effect)
                {
                    effect.Render();
                }
            }
        }
    }
}