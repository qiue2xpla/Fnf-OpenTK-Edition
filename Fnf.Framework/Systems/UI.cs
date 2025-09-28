using System;
using System.Runtime.CompilerServices;

namespace Fnf.Framework
{
    public abstract class UI : GameObject
    {
        public bool IsSelected => selectedUI == this;
        public float width, height;
        public bool raycast;

        public UI()
        {
            raycast = true;
            height = 100;
            width = 100;
        }

        protected virtual void OnMouseHover(Vector2 mouse) { }
        protected virtual void OnMouseMove(Vector2 mouse) { }
        protected virtual void OnMouseDrag(MouseButton button) { }
        protected virtual void OnMouseDown(MouseButton button) { }
        protected virtual void OnMouseUp(MouseButton button) { }

        protected virtual void OnMouseEnter() { }
        protected virtual void OnMouseLeave() { }

        protected void SetTopControl()
        {
            if (!raycast) return;
            topUI = this;
        }

        protected bool IsInside()
        {
            Vector2 ui = GridToUISpace(Input.GetGridMousePosition());
            return (ui.x >= 0 && ui.x <= width) && (ui.y >= 0 && ui.y <= height);
        }

        protected Vector2 GridToUISpace(Vector2 mouse)
        {
            mouse -= globalPosition;
            mouse = mouse.Rotate(-globalRotation);
            mouse /= globalScale;

            mouse += new Vector2(width, height) / 2;

            mouse.x = (int)mouse.x;
            mouse.y = (int)mouse.y;

            return mouse;
        }


        private static UI topUI;      // The UI that the mouse is ontop of
        private static UI currentUI;  // The UI that the mouse was ontop of
        private static UI selectedUI; // The last selected UI

        internal static void InvokeEvents()
        {
            if (currentUI == null)
            {
                if (selectedUI == null) return;

                foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
                {
                    if (Input.GetButtonDown(button))
                    {
                        selectedUI = null;
                        return;
                    }
                }
            }
            else
            {
                Vector2 ui = currentUI.GridToUISpace(Input.GetGridMousePosition());
                bool moved = Input.GridMouseMoved();

                currentUI.OnMouseHover(ui);

                if (moved) currentUI.OnMouseMove(ui);

                foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
                {
                    // Yes i know that its not the best way to do it
                    // but it took me long enough to make it work :')
                    if (Input.GetButtonDown(button))
                    {
                        selectedUI = currentUI;
                        currentUI.OnMouseDown(button);
                    }
                    if (Input.GetButton(button) && moved && currentUI.IsSelected) currentUI.OnMouseDrag(button);
                    if (Input.GetButtonUp(button)) currentUI.OnMouseUp(button);
                }
            }
        }

        internal static void Raycast()
        {
            if (topUI == null)
            {
                // Mouse is not on any control

                if (currentUI != null)
                {
                    // Call the leave function on the control we left from

                    currentUI.OnMouseLeave();
                }
            }
            else
            {
                // Mouse is on a control

                if (currentUI == null)
                {
                    // Entered a new control

                    topUI.OnMouseEnter();
                }
                else
                {
                    // Mouse is still on a control

                    if (topUI != currentUI)
                    {
                        // Mouse moved to a new control

                        currentUI.OnMouseLeave();
                        topUI.OnMouseEnter();
                    }
                }
            }

            currentUI = topUI;
            topUI = null;
        }
    }
}