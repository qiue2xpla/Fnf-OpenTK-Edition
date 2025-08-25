using Fnf.Framework.Graphics;

namespace Fnf.Framework
{
    public class Panel : UI, IRenderable
    {
        public bool isRenderable { get; set; } = true;

        public Color color = Color.White;
        public float borderRadius = 0;
        public float borderSmoothness = 0;

        public void Render()
        {
            if (!isRenderable) return;
            if (raycast && IsInside()) SetTopControl();

            Gizmos.DrawRoundQuad(globalPosition, globalScale, width, height, globalRotation, borderRadius, borderSmoothness, color);
        }
    }
}