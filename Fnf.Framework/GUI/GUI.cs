using System;

namespace Fnf.Framework
{
    /// <summary>
    /// Basically a <seealso cref="GameObject"/> but with more features for advanced GUI behaviour
    /// </summary>
    public abstract class GUI : GameObject
    {
        // Some global tracking values TODO: rename them bitch

        /// <summary>
        /// the gui that the raycast hit
        /// </summary>
        private static GUI raycastGUI;

        /// <summary>
        /// The current gui
        /// </summary>
        private static GUI currentGUI;
        private static GUI selectedGUI; // The last selected UI

        /// <summary>
        /// Is the current <seealso cref="GUI"/> selected
        /// </summary>
        public bool isSelected => selectedGUI == this;

        public bool isRaycastable = true;
        public float width, height;


        internal static void StartInvokingEvents()
        {
            if (currentGUI == null)
            {
                if (selectedGUI == null) return;

                foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
                {
                    if (Input.GetButtonDown(button))
                    {
                        selectedGUI = null;
                        return;
                    }
                }
            }
            else
            {
                Vector2 ui = Input.GetGridMousePosition();
                bool moved = Input.GridMouseMoved();

                currentGUI.OnMouseHover();

                if (moved) currentGUI.OnMouseMove();

                foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
                {
                    // Yes i know that its not the best way to do it
                    // but it took me long enough to make it work :')
                    if (Input.GetButtonDown(button))
                    {
                        selectedGUI = currentGUI;
                        currentGUI.OnMouseDown(button);
                    }
                    if (Input.GetButton(button) && moved && currentGUI.isSelected) currentGUI.OnMouseDrag(button);
                    if (Input.GetButtonUp(button)) currentGUI.OnMouseUp(button);
                }
            }
        }

        internal static void StartRaycasting()
        {
            // Yes this makes updates 1 tick behind while also allowingthe draging
            // to work no matter the mouse speed and doesn't let go randomly

            // Check if the mouse in not over any GUI
            if (raycastGUI == null)
            {
                // Check if a current GUI is available and leave it
                if (currentGUI != null) currentGUI.OnMouseLeave();
            }
            else
            {
                // Check if there is a current GUI
                if (currentGUI == null)
                {
                    // Entered a new control

                    raycastGUI.OnMouseEnter();
                }
                else
                {
                    // Mouse is still on a control

                    if (raycastGUI != currentGUI)
                    {
                        // Mouse moved to a new control

                        currentGUI.OnMouseLeave();
                        raycastGUI.OnMouseEnter();
                    }
                }
            }

            currentGUI = raycastGUI;
            raycastGUI = null;
        }
    
        /// <summary>
        /// Indicate that this GUI is the top GUI.
        /// Must be invoked when rendering the custom <seealso cref="GUI"/>. Also it automatically handles the <seealso cref="isRaycastable"/>
        /// </summary>
        protected void RaycastHit()
        {
            if (isRaycastable) raycastGUI = this;
        }

        /// <summary>
        /// Returns if the mouse is over the <seealso cref="GUI"/>
        /// </summary>
        protected bool IsOverGUI()
        {
            var mat = GetObjectWorldInverseTransformMatrix();
            Vector2 mousePosition = (mat * Input.GetGridMousePosition().ToHomogeneous()).ToEuclidean() + new Vector2(width, height) / 2;
            return (mousePosition.x >= 0 && mousePosition.x <= width) && (mousePosition.y >= 0 && mousePosition.y <= height);
        }

        /// <summary>
        /// Invoked when the mouse is ontop of the <seealso cref="GUI"/>
        /// </summary>
        protected virtual void OnMouseHover() { }

        /// <summary>
        /// Invoked when the mouse is moving over the <seealso cref="GUI"/>
        /// </summary>
        protected virtual void OnMouseMove() { }

        /// <summary>
        /// Invoked when the mouse is holding a button and moving
        /// </summary>
        protected virtual void OnMouseDrag(MouseButton button) { }

        /// <summary>
        /// Invoked when a mouse button is pressed
        /// </summary>
        protected virtual void OnMouseDown(MouseButton button) { }

        /// <summary>
        /// Invoked when a mouse button is released
        /// </summary>
        protected virtual void OnMouseUp(MouseButton button) { }

        /// <summary>
        /// Invoked when the mouse enters
        /// </summary>
        protected virtual void OnMouseEnter() { }

        /// <summary>
        /// Invoked when the mouse leaves
        /// </summary>
        protected virtual void OnMouseLeave() { }
    }
}