using Fnf.Framework.Graphics;
using Fnf.Framework.TrueType.Rendering;
using System;
using System.Net;

namespace Fnf.Framework
{
    /// <summary>
    /// An easy to use class to handle button clicking with some customizations.
    /// To make colors lerp over time set the <seealso cref="smoothColor"/> to true 
    /// </summary>
    public class Button : GUI, IRenderable
    {
        public bool isRenderable { get; set; } = true;

        public Text overlayText;

        public event Action OnClick;

        public Color backgroundColor = Color.White;
        public Color hoverColor = new Color(0.8f, 1);
        public Color pressColor = new Color(0.6f, 1);

        public float borderSmoothness = 10;
        public float cornerRadius = 20;
        public bool smoothColor;

        private Color target;
        private Color current;

        public Button(FontAtlas atlas)
        {
            overlayText = new Text(atlas);
            overlayText.parent = this;
            overlayText.color = Color.Black;
            overlayText.isRaycastable = false;

            target = backgroundColor;
            current = backgroundColor;
        }

        public void Render()
        {
            if (!isRenderable) return;
            if (IsOverGUI()) RaycastHit();
            if(smoothColor) current = Color.Lerp(current, target, Time.deltaTime * 20);

            Gizmos.DrawRoundQuad(this, current, cornerRadius, borderSmoothness);
            overlayText.Render();
        }

        protected override void OnMouseEnter()
        {
            target = hoverColor;
            if(!smoothColor) current = hoverColor;
        }

        protected override void OnMouseLeave()
        {
            target = backgroundColor;
            if(!smoothColor) current = backgroundColor;
        }

        protected override void OnMouseDown(MouseButton button)
        {
            if (button != MouseButton.Left) return;

            target = pressColor;
            if (!smoothColor) current = pressColor;
        }

        protected override void OnMouseUp(MouseButton button)
        {
            if (button != MouseButton.Left) return;

            target = hoverColor;
            if (!smoothColor) current = hoverColor;

            OnClick?.Invoke();
        }
    }
}