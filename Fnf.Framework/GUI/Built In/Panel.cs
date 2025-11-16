using Fnf.Framework.Graphics;

namespace Fnf.Framework
{
    public class Panel : GUI, IRenderable
    {
        public bool isRenderable { get; set; } = true;

        public Color color = Color.White;
        public float borderRadius = 0;
        public float borderSmoothness = 0;

        public void Render()
        {
            if (!isRenderable) return;
            if (isRaycastable && IsMouseOverGUI()) RaycastHit();

            Gizmos.DrawRoundQuad(this, color, borderRadius, borderSmoothness);
        }
    }
}