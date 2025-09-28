using Fnf.Framework.Graphics;
using System.Net;

namespace Fnf.Framework
{
    public class Button : GUI, IRenderable, IUpdatable
    {
        public Text overlayText = new Text();

        public Color backgroundColor = Color.White;
        public Color hoverColor = new Color(220, 220, 220, 255);
        public Color pressColor = new Color(180, 180, 180, 255);

        public bool isRenderable { get; set; } = true;
        public bool isUpdatable { get; set; } = false;

        public float borderSmoothness = 2;
        public float cornerRadius = 5;

        private Color target;
        private Color current;

        public Button()
        {
            target = backgroundColor;
            current = backgroundColor;

            overlayText.parent = this;
            overlayText.color = Color.Black;
            overlayText.isRaycastable = false;
        }

        public void Update()
        {
            if (!isUpdatable) return;

            current = Color.Lerp(current, target, Time.deltaTime * 20);
        }

        public void Render()
        {
            if (!isRenderable) return;
            if (IsOverGUI()) RaycastHit();

            overlayText.width = width;
            overlayText.height = height;

            Gizmos.DrawRoundQuad(
                globalPosition, 
                globalScale, 
                width, height, 
                globalRotation, 
                cornerRadius, 
                borderSmoothness,
                current);

            overlayText.Render();
        }

        protected override void OnMouseEnter()
        {
            target = hoverColor;
            if(!isUpdatable) current = hoverColor;
        }

        protected override void OnMouseLeave()
        {
            target = backgroundColor;
            if(!isUpdatable) current = backgroundColor;
        }

        protected override void OnMouseDown(MouseButton button)
        {
            if (button != MouseButton.Left) return;

            target = pressColor;
            if (!isUpdatable) current = pressColor;
        }

        protected override void OnMouseUp(MouseButton button)
        {
            if (button != MouseButton.Left) return;

            target = hoverColor;
            if (!isUpdatable) current = hoverColor;
        }
    }
}