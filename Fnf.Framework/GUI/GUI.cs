using System;

namespace Fnf.Framework
{
    /// <summary>
    /// Basically a <seealso cref="GameObject"/> but with more features for advanced GUI behaviour
    /// </summary>
    public class GUI : GameObject
    {
        public (float top, float right, float bottom, float left) padding;
        public (float top, float right, float bottom, float left) margin;
        public float scaleFactor = 1;
        public float width, height;
        public float gap;

        public LayoutMode layoutMode = LayoutMode.HorizontalFlow;
        public SizeMode horizontalSizeMode = SizeMode.Fixed;
        public SizeMode verticalSizeMode = SizeMode.Fixed;
        public VerticalAlignment verticalAlignment = VerticalAlignment.Center;
        public HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center;

        /// <summary>
        /// Updates the layout of the GUI and its children
        /// </summary>
        public void UpdateLayout()
        {
            for (int i = 0; i < children.Length; i++) (children[i] as GUI).UpdateLayout();

            switch(layoutMode)
            {
                case LayoutMode.None: break; // This mode does nothing
                case LayoutMode.Grid: throw new NotImplementedException();
                case LayoutMode.HorizontalFlow: case LayoutMode.VerticalFlow:
                {
                    // Handle width
                    switch(horizontalSizeMode)
                    {
                        case SizeMode.Flexible:
                        {
                            if (parent == null) width = Window.GridSize.width;
                            break;
                        }

                        case SizeMode.Auto:
                        {
                            width = 0;

                            // Loop for each child and add its width if in HorizontalFlow or use the maximum width in VerticalFlow.
                            for (int i = 0; i < children.Length; i++)
                            {
                                GUI child = children[i] as GUI;

                                if (child.horizontalSizeMode == SizeMode.Fixed || child.horizontalSizeMode == SizeMode.Auto)
                                {
                                    // Get the child width with both left and right margins
                                    float childWidth = child.width + child.margin.left + child.margin.right;

                                    if (layoutMode == LayoutMode.HorizontalFlow)
                                    {
                                        width += childWidth;
                                    }
                                    else
                                    {
                                        width = Math.Max(width, childWidth);
                                    }
                                }
                                else
                                {
                                    if (layoutMode == LayoutMode.HorizontalFlow)
                                    {
                                        throw new InvalidLayoutException("'Auto' parent cannot contain a 'Flexible' child");
                                    }
                                }
                            }

                            // Add gap to width
                            if (layoutMode == LayoutMode.HorizontalFlow)
                            {
                                width += gap * Math.Max(children.Length - 1, 0);
                            }

                            // Add padding
                            width += padding.left + padding.right;

                            break;
                        }
                    }

                    // Handle height
                    switch(verticalSizeMode)
                    {
                        case SizeMode.Flexible:
                        {
                            if (parent == null) height = Window.GridSize.height;
                            break;
                        }

                        case SizeMode.Auto:
                        {
                            // Reset height
                            height = 0;

                            // Loop children
                            for (int i = 0; i < children.Length; i++)
                            {
                                GUI child = children[i] as GUI;

                                if (child.verticalSizeMode == SizeMode.Fixed || child.verticalSizeMode == SizeMode.Auto)
                                {
                                    // Get child height with top and bottom margin
                                    float childHeight = child.height + child.margin.top + child.margin.bottom;

                                    if (layoutMode == LayoutMode.HorizontalFlow)
                                    {
                                        height = Math.Max(height, childHeight);
                                    }
                                    else
                                    {
                                        height += childHeight;
                                    }
                                }
                                else
                                {
                                    if (layoutMode == LayoutMode.VerticalFlow)
                                    {
                                        throw new InvalidLayoutException("'Auto' parent cannot contain a 'Flexible' child");
                                    }
                                }
                            }

                            // Add gap to height
                            if (layoutMode == LayoutMode.VerticalFlow)
                            {
                                height += gap * Math.Max(children.Length - 1, 0);
                            }

                            // Add padding
                            height += padding.top + padding.bottom;

                            break;
                        }
                    }

                    // Allocate j to Flexible children

                    float remainingWidth = width - padding.left - padding.right;
                    float totalHUnits = 0;

                    for (int i = 0; i < children.Length; i++)
                    {
                        GUI child = children[i] as GUI;

                        if (child.horizontalSizeMode != SizeMode.Flexible)
                        {
                            float childWidth = child.width + child.margin.left + child.margin.right;

                            remainingWidth -= childWidth;

                            totalHUnits = child.scaleFactor;
                        }
                    }

                    remainingWidth -= gap * Math.Max(children.Length - 1, 0);

                    float widthPerUnit = remainingWidth / totalHUnits;

                    for (int i = 0; i < children.Length; i++)
                    {
                        GUI child = children[i] as GUI;

                        if (child.horizontalSizeMode == SizeMode.Flexible)
                        {
                            child.width = child.scaleFactor * widthPerUnit - child.margin.left - child.margin.right;
                        }
                    }

                    // Allocate width to Flexible children

                    float remainingWidth = width - padding.left - padding.right;
                    float totalHUnits = 0;

                    for (int i = 0; i < children.Length; i++)
                    {
                        GUI child = children[i] as GUI;

                        if (child.horizontalSizeMode != SizeMode.Flexible)
                        {
                            float childWidth = child.width + child.margin.left + child.margin.right;

                            remainingWidth -= childWidth;

                            totalHUnits = child.scaleFactor;
                        }
                    }

                    remainingWidth -= gap * Math.Max(children.Length - 1, 0);

                    float widthPerUnit = remainingWidth / totalHUnits;

                    for (int i = 0; i < children.Length; i++)
                    {
                        GUI child = children[i] as GUI;

                        if (child.horizontalSizeMode == SizeMode.Flexible)
                        {
                            child.width = child.scaleFactor * widthPerUnit - child.margin.left - child.margin.right;
                        }
                    }

                    break;
                }
            }
            /*
            


            // Handle horizontal positioning
            if (containerType == ContainerType.HorizontalFlow)
            {
                float pointer = padding.left;
                for (int i = 0; i < children.Length; i++)
                {
                    GUI child = children[i] as GUI;
                    child.localPosition.x = -width / 2 + pointer + child.width / 2;

                    switch(child.verticalAlignment)
                    {
                        case VerticalAlignment.Top: child.localPosition.y = height / 2 - child.height / 2 - padding.top; break;
                        case VerticalAlignment.Center: child.localPosition.y = 0; break;
                        case VerticalAlignment.Bottom: child.localPosition.y = -height / 2 + padding.bottom; break;
                    }

                    pointer += child.width + gap;
                }
            }

            // Handle vertical positioning
            if (containerType == ContainerType.VerticalFlow)
            {
                float pointer = padding.top;
                for (int i = 0; i < children.Length; i++)
                {
                    GUI child = children[i] as GUI;
                    child.localPosition.x = -width / 2 + child.width / 2;
                    child.localPosition.y = height / 2 - pointer - child.height / 2;
                    pointer += child.width;
                }
            }*/
        }










































        // Some global tracking values

        /// <summary>
        /// Is the current <seealso cref="GUI"/> selected
        /// </summary>
        public bool isSelected => selectedGUI == this;

        public bool isRaycastable = true;


    
        /// <summary>
        /// Indicate that this GUI is the top GUI
        /// </summary>
        protected void RaycastHit()
        {
            raycastGUI = this;
        }

        /// <summary>
        /// Returns if the mouse is over this <seealso cref="GUI"/>
        /// </summary>
        public bool IsMouseOverGUI()
        {
            return IsMouseOverGUI(globalPosition, globalScale, globalRotation, width, height);
        }

        

        #region Static

        /// <summary>
        /// The gui that the raycast hit
        /// </summary>
        private static GUI raycastGUI;

        /// <summary>
        /// The current top gui to use
        /// </summary>
        private static GUI currentGUI;

        /// <summary>
        /// The last selected UI
        /// </summary>
        private static GUI selectedGUI;

        internal static void InvokeEvents()
        {
            if (currentGUI == null)
            {
                if (selectedGUI != null)
                {
                    for (int i = 0; i < 13; i++)
                    {
                        if (Input.GetButtonDown((MouseButton)i))
                        {
                            // If the mouse in not over any gui and it presses  
                            // any button the deselect the current selected gui
                            selectedGUI = null;
                            return;
                        }
                    }
                }
            }
            else
            {
                bool moved = Input.GridMouseMoved();

                currentGUI.OnMouseHover();
                if (moved) currentGUI.OnMouseMove();

                for(int i = 0; i < 13; i++)
                {
                    MouseButton button = (MouseButton)i;


                    if (Input.GetButtonDown(button))
                    {
                        selectedGUI = currentGUI;
                        currentGUI.OnMouseDown(button);
                    }
                    if (Input.GetButtonUp(button)) currentGUI.OnMouseUp(button);

                    if (Input.GetButton(button) && currentGUI.isSelected)
                    {
                        currentGUI.OnMouse(button);
                        if(moved) currentGUI.OnMouseDrag(button);
                    }
                }
            }

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
        /// Returns if the mouse is over the given <seealso cref="GUI"/> data
        /// </summary>
        public static bool IsMouseOverGUI(Vector2 pos, Vector2 size, float rotation, float width, float height)
        {
            float hw = width / 2, hh = height / 2;
            Vector2 mp = Matrix3.InverseTransform(pos, -MathUtility.ToRadian(rotation), size) * Input.GetGridMousePosition();
            return Math.Abs(mp.x) <= hw && Math.Abs(mp.y) <= hh;
        }

        #endregion

        #region Events

        /// <summary>
        /// Invoked when the mouse is ontop of the <seealso cref="GUI"/>
        /// </summary>
        protected virtual void OnMouseHover() { }

        /// <summary>
        /// Invoked when the mouse is moving over the <seealso cref="GUI"/>
        /// </summary>
        protected virtual void OnMouseMove() { }

        /// <summary>
        /// Invoked when the mouse is holding a button
        /// </summary>
        protected virtual void OnMouse(MouseButton button) { }
        
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

        #endregion
    }

    public enum LayoutMode
    {
        HorizontalFlow,
        VerticalFlow,
        Grid,
        None,
    }

    public enum SizeMode
    {
        Fixed,
        Auto,
        Flexible
    }

    public enum VerticalAlignment
    {
        Top,
        Center,
        Bottom
    }

    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right
    }

    public class InvalidLayoutException : Exception
    {
        public InvalidLayoutException(string message):base(message) { }
    }
}