using OpenTK.Graphics.ES11;
using System;

namespace Fnf.Framework
{
    /// <summary>
    /// Basically a <seealso cref="GameObject"/> but with more features for advanced GUI behaviour
    /// </summary>
    public class GUI : GameObject
    {
        // Layout

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

        // Interaction

        /// <summary>
        /// Is the current <seealso cref="GUI"/> selected
        /// </summary>
        public bool isSelected => selectedGUI == this;

        /// <summary>
        /// Is the <seealso cref="GUI"/> interactable 
        /// </summary>
        public bool isRaycastable = true;

        // Functions 

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
                    // ---------------Handle width and height---------------
                    bool autoWidth = horizontalSizeMode == SizeMode.Auto;
                    bool autoHeight = verticalSizeMode == SizeMode.Auto;
                    bool horizontalFlow = layoutMode == LayoutMode.HorizontalFlow;

                    if (autoWidth || autoHeight)
                    {
                        // Reset width and height
                        if (autoWidth) width = 0;
                        if (autoHeight) height = 0;

                        for (int i = 0; i < children.Length; i++)
                        {
                            GUI child = children[i] as GUI;

                            if (autoWidth)
                            {
                                if (child.horizontalSizeMode == SizeMode.Flexible && horizontalFlow)
                                {
                                    throw new InvalidLayoutException("'Auto' parent cannot contain a 'Flexible' child");
                                }

                                if (child.horizontalSizeMode != SizeMode.Flexible)
                                {
                                    float childWidth = child.width + child.margin.left + child.margin.right;
                                    width = horizontalFlow ? width + childWidth : Math.Max(width, childWidth);
                                }
                            }

                            if (autoHeight)
                            {
                                if (child.verticalSizeMode == SizeMode.Flexible && !horizontalFlow)
                                {
                                    throw new InvalidLayoutException("'Auto' parent cannot contain a 'Flexible' child");
                                }

                                if (child.verticalSizeMode != SizeMode.Flexible)
                                {
                                    float childHeight = child.height + child.margin.top + child.margin.bottom;
                                    height = horizontalFlow ? Math.Max(height, childHeight) : height += childHeight;
                                }
                            }
                        }

                        // Add gap
                        if (layoutMode == LayoutMode.HorizontalFlow && autoWidth)
                        {
                            width += gap * Math.Max(children.Length - 1, 0);
                        }

                        if (layoutMode == LayoutMode.VerticalFlow && autoHeight)
                        {
                            height += gap * Math.Max(children.Length - 1, 0);
                        }

                        // Add padding
                        if (autoWidth) width += padding.left + padding.right;
                        if (autoHeight) height += padding.top + padding.bottom;
                    }

                    if (verticalSizeMode == SizeMode.Flexible && parent == null)
                    {
                        height = Window.GridSize.height;
                    }

                    if (horizontalSizeMode == SizeMode.Flexible && parent == null)
                    {
                        width = Window.GridSize.width;
                    }

                    // ---------------Handle SizeMode.Flexible children---------------
                    float totalHorizontalScalingUnits = 0;
                    float totalVerticalScalingUnits = 0;
                    float remainingWidth = width;
                    float remainingHeight = height;

                    // Remove the padding 
                    remainingWidth -= padding.left + padding.right;
                    remainingHeight -= padding.top + padding.bottom;

                    for (int i = 0; i < children.Length; i++)
                    {
                        GUI child = children[i] as GUI;

                        if (horizontalFlow)
                        {
                            if (child.horizontalSizeMode == SizeMode.Flexible)
                            {
                                totalHorizontalScalingUnits += child.scaleFactor;
                            }
                            else
                            {
                                remainingWidth -= child.width + child.margin.left + child.margin.right;
                            }
                        }
                        else
                        {
                            if (child.verticalSizeMode == SizeMode.Flexible)
                            {
                                totalVerticalScalingUnits += child.scaleFactor;
                            }
                            else
                            {
                                remainingHeight -= child.height + child.margin.top + child.margin.bottom;
                            }
                        }
                    }

                    // Remove gaps
                    float totalGap = gap * Math.Max(children.Length - 1, 0);
                    if (horizontalFlow)
                    {
                        remainingWidth -= totalGap;
                    }
                    else
                    {
                        remainingHeight -= totalGap;
                    }

                    float widthPerUnit = 0;
                    float heightPerUnit = 0;

                    if (totalHorizontalScalingUnits > 0) widthPerUnit = remainingWidth / totalHorizontalScalingUnits;
                    if (totalVerticalScalingUnits > 0) heightPerUnit = remainingHeight / totalVerticalScalingUnits;

                    for (int i = 0; i < children.Length; i++)
                    {
                        GUI child = children[i] as GUI;

                        if (child.horizontalSizeMode == SizeMode.Flexible)
                        {
                            if(horizontalFlow)
                            {
                                float available = child.scaleFactor * widthPerUnit;
                                child.width = Math.Max(0, available - child.margin.left - child.margin.right);
                            }
                            else
                            {
                                child.width = remainingWidth;
                            }
                        }

                        if (child.verticalSizeMode == SizeMode.Flexible)
                        {
                            if (horizontalFlow)
                            {
                                child.height = remainingHeight;
                            }
                            else
                            {
                                float available = child.scaleFactor * heightPerUnit;
                                child.height = Math.Max(0, available - child.margin.top - child.margin.bottom);
                            }
                        }
                    }

                    // ---------------Handle Position---------------
                    Vector2 pointer = new Vector2(padding.left, -padding.top); // TopLeft origin
                    Vector2 offset = new Vector2(-width/2, height/2);
                    for (int i = 0; i < children.Length; i++)
                    {
                        GUI child = children[i] as GUI;

                        float childWidth = child.width + child.margin.left + child.margin.right;
                        float childHeight = child.height + child.margin.top + child.margin.bottom;

                        if (horizontalFlow)
                        {
                            child.localPosition.x = offset.x + pointer.x + childWidth/2;

                            switch (child.verticalAlignment)
                            {
                                case VerticalAlignment.Top: child.localPosition.y = offset.y - childHeight/2 - padding.top; break;
                                case VerticalAlignment.Center: child.localPosition.y = 0; break;
                                case VerticalAlignment.Bottom: child.localPosition.y = -offset.y + childHeight/2 + padding.bottom; break;
                            }

                            pointer.x += childWidth + gap;
                        }
                        else
                        {
                            child.localPosition.y = offset.y + pointer.y - childHeight/2;

                            switch (child.horizontalAlignment)
                            {
                                case HorizontalAlignment.Left: child.localPosition.x = offset.x + padding.left + childWidth / 2; break;
                                case HorizontalAlignment.Center: child.localPosition.x = 0; break;
                                case HorizontalAlignment.Right: child.localPosition.x = -offset.x - padding.right - childWidth/ 2; break;
                            }

                            pointer.y -= childHeight + gap;
                        }
                    }

                    if(parent == null)
                    {
                        switch(horizontalAlignment)
                        {
                            case HorizontalAlignment.Left: localPosition.x = -Window.GridSize.width / 2 + width / 2 + margin.left; break;
                            case HorizontalAlignment.Center: localPosition.x = 0; break;
                            case HorizontalAlignment.Right: localPosition.x = Window.GridSize.width / 2 - width / 2 - margin.right; break;
                        }

                        switch (verticalAlignment)
                        {
                            case VerticalAlignment.Top: localPosition.y = Window.GridSize.height / 2 - height / 2 - margin.top; break;
                            case VerticalAlignment.Center: localPosition.x = 0; break;
                            case VerticalAlignment.Bottom: localPosition.x = -Window.GridSize.height / 2 + height / 2 + margin.bottom; break;
                        }
                    }

                    break;
                }
            }
        }

        public virtual void Raycast()
        {
            for (int i = 0; i < children.Length; i++) (children[i] as GUI).Raycast();
        }

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
        protected bool IsMouseOverGUI()
        {
            return IsMouseOverArea(globalPosition, globalScale, globalRotation, width, height);
        }

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
        /// Returns if the mouse is over the given area
        /// </summary>
        public static bool IsMouseOverArea(Vector2 pos, Vector2 size, float rotation, float width, float height)
        {
            Vector2 mp = Matrix3.InverseTransform(pos, -MathUtility.ToRadian(rotation), size) * Input.GetGridMousePosition();
            return Math.Abs(mp.x) <= width / 2 && Math.Abs(mp.y) <= height / 2;
        }

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