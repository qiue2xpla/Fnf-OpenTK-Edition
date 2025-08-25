namespace Fnf.Game
{
    public class Effect
    {
        public bool isActive => EffectsManager.currentEffects.Contains(this);

        protected float cooldown;
        protected float duration;
        protected float remaining;

        public Effect()
        {
            // Add the effect to the effects manager automaticaly
            EffectsManager.currentEffects.Add(this);
        }

        public void DeactivateEffect()
        {
            EffectsManager.removalQueue.Add(this);
        }
    }
}