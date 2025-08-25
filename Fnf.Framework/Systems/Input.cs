using OpenTK.Input;
using OpenTK;
using System;

namespace Fnf.Framework
{
    /// <summary>
    /// Gets mouse and keyboard inputs from the user
    /// </summary>
    public static class Input
    {
        public static bool AllowUnfocusedInput = false;

        public static event Action<char> OnCharTyped;
        public static bool IsCursorVisible
        {
            get => Window._window.CursorVisible;
            set => Window._window.CursorVisible = value;
        }

        private static KeyboardState currentKeyboard;
        private static KeyboardState previousKeyboard;
        private static MouseState currentMouse;
        private static MouseState previousMouse;
        //private static bool _lockCursore = false;

        /// <summary>
        /// Gets and sets the cursor locked status
        /// </summary>
        /*public static bool IsCursorLocked
        {
            get => _lockCursore;
            set
            {
                //brb
                //if (value && !_lockMouse) SetMousePosition(0, 0);
                _lockCursore = value;
            }
        }*/

        #region Key

        /// <summary>
        /// Check if a key is held down
        /// </summary>
        public static bool GetKey(Key key)
        {
            return currentKeyboard.IsKeyDown(Convert(key));
        }

        /// <summary>
        /// Checks if any of the given keys is held down
        /// </summary>
        public static bool GetAnyKeys(params Key[] keys)
        {
            for (int i = 0; i < keys.Length; i++) if (GetKey(keys[i])) return true;
            return false;
        }

        /// <summary>
        /// Checks if all of the given keys are held down
        /// </summary>
        public static bool GetAllKeys(params Key[] keys)
        {
            for (int i = 0; i < keys.Length; i++) if (!GetKey(keys[i])) return false;
            return true;
        }

        /// <summary>
        /// Checks if any key is held down
        /// </summary>
        public static bool IsAnyKeyDown()
        {
            return currentKeyboard.IsAnyKeyDown;
        }

        #endregion

        #region Key Down

        /// <summary>
        /// Checks if a key has been pressed
        /// </summary>
        public static bool GetKeyDown(Key key)
        {
            var convert = Convert(key);
            return currentKeyboard[convert] && !previousKeyboard[convert];
        }

        /// <summary>
        /// Checks if any of the given keys has been pressed
        /// </summary>
        public static bool GetAnyKeysDown(params Key[] keys)
        {
            for (int i = 0; i < keys.Length; i++) if (GetKeyDown(keys[i])) return true;
            return false;
        }

        /// <summary>
        /// Checks if all the given keys has been pressed
        /// </summary>
        public static bool GetAllKeysDown(params Key[] keys)
        {
            for (int i = 0; i < keys.Length; i++) if (!GetKeyDown(keys[i])) return false;
            return true;
        }

        #endregion

        #region Key Up

        /// <summary>
        /// Checks if a key has been released
        /// </summary>
        public static bool GetKeyUp(Key key)
        {
            var conver = Convert(key);
            return !currentKeyboard[conver] && previousKeyboard[conver];
        }

        /// <summary>
        /// Checks if any of the given keys has been released
        /// </summary>
        public static bool GetAnyKeysUp(params Key[] keys)
        {
            for (int i = 0; i < keys.Length; i++) if (GetKeyUp(keys[i])) return true;
            return false;
        }

        /// <summary>
        /// Checks if all of the given keys has been released
        /// </summary>>
        public static bool GetAllKeysUp(params Key[] keys)
        {
            for (int i = 0; i < keys.Length; i++) if (!GetKeyUp(keys[i])) return false;
            return true;
        }

        #endregion

        #region Scroll Wheel

        /// <summary>
        /// Gets the current mouse scroll wheel position
        /// </summary>
        public static int GetScrollWheel()
        {
            return (int)currentMouse.WheelPrecise;
        }

        /// <summary>
        /// Gets the change in the current mouse scroll wheel position
        /// </summary>
        public static int GetScrollWheelDelta()
        {
            return (int)currentMouse.WheelPrecise - (int)previousMouse.WheelPrecise;
        }

        #endregion

        #region Mouse Position

        public static bool GridMouseMoved()
        {
            Vector2 delta = GetGridMouseDelta();
            return delta.x + delta.y != 0;
        }

        /// <summary>
        /// Gets mouse position in desktop space
        /// </summary>
        public static Point GetDesktopMousePosition()
        {
            return ToDesktop(currentMouse);
        }

        /// <summary>
        /// Gets mouse position in window space
        /// </summary>
        public static Point GetWindowMousePosition()
        {
            return ToWindow(currentMouse);
        }

        /// <summary>
        /// Gets mouse position in grid space
        /// </summary>
        public static Vector2 GetGridMousePosition()
        {
            return ToGrid(currentMouse);
        }

        /// <summary>
        /// Sets mouse position in desktop space
        /// </summary>
        public static void SetDesktopMousePosition(int x, int y)
        {
            Mouse.SetPosition(x, Window.ScreenSize.height - y - 1);
        }

        /// <summary>
        /// Sets mouse position in window space
        /// </summary>
        public static void SetWindowMousePosition(int x, int y)
        {
            var desktop = Window._window.PointToScreen(
                new System.Drawing.Point(x, Window.WindowSize.height - y - 1));
            Mouse.SetPosition(desktop.X, desktop.Y);
        }

        /// <summary>
        /// Sets mouse position in grid space
        /// </summary>
        public static void SetGridMousePosition(int x, int y)
        {
            y = Window.GridSize.height - y - 1;

            float outX = 0;
            float outY = 0;

            if (Window.WindowSize.slobe > Window.GridSize.slobe)
            {
                //window is thin
                float gridHeightInPixels = Window.WindowSize.width * Window.GridSize.slobe;
                float offset = (Window.WindowSize.height - gridHeightInPixels) / 2;
                outY = (float)y / Window.GridSize.height * gridHeightInPixels + offset;
                outX = (float)x / Window.GridSize.width * Window.WindowSize.width;
            }
            else
            {
                //window is thick
                float gridWidthInPixels = Window.WindowSize.height / Window.GridSize.slobe;
                float offset = (Window.WindowSize.width - gridWidthInPixels) / 2;
                outX = (float)x / Window.GridSize.width * gridWidthInPixels + offset;
                outY = (float)y / Window.GridSize.height * Window.WindowSize.height;
            }

            var desktop = Window._window.PointToScreen(new System.Drawing.Point((int)outX, (int)outY));
            Mouse.SetPosition(desktop.X, desktop.Y);
        }

        /// <summary>
        /// Gets the change in mouse position in desktop space
        /// </summary>
        public static Point GetDesktopMouseDelta()
        {
            if (!Window._window.Focused) return new Point(0, 0);
            //if (isCursorLocked) return ToDesktop(currentMouse); 
            return ToDesktop(currentMouse) - ToDesktop(previousMouse);
        }

        /// <summary>
        /// Gets the change in mouse position in window space
        /// </summary>
        public static Point GetWindowMouseDelta()
        {
            if (!Window._window.Focused) return new Point(0, 0);
            //if (isCursorLocked) return ToWindow(currentMouse);
            return ToWindow(currentMouse) - ToWindow(previousMouse);
        }

        /// <summary>
        /// Gets the change in mouse position in grid space
        /// </summary>
        public static Vector2 GetGridMouseDelta()
        {
            if (!Window._window.Focused) return new Vector2(0, 0);
            //if (isCursorLocked) return ToGrid(currentMouse);
            return ToGrid(currentMouse) - ToGrid(previousMouse);
        }

        private static Point ToDesktop(MouseState mouse)
        {
            return new Point(mouse.X, DisplayDevice.Default.Height - mouse.Y - 1);
        }

        private static Point ToWindow(MouseState mouse)
        {
            var desktop = new System.Drawing.Point(mouse.X, mouse.Y);
            var window = Window._window.PointToClient(desktop);
            window.Y = Window._window.Height - window.Y - 1;
            return new Point(window.X, window.Y);
        }

        private static Vector2 ToGrid(MouseState mouse)
        {
            var desktop = new System.Drawing.Point(mouse.X, mouse.Y);
            var window = Window._window.PointToClient(desktop);
            float x = 0;
            float y = 0;

            if (Window.WindowSize.slobe > Window.GridSize.slobe)
            {
                //window is thin
                float gridHeightInPixels = Window.WindowSize.width * Window.GridSize.slobe;
                float offset = (Window.WindowSize.height - gridHeightInPixels) / 2;
                y = (window.Y - offset) / gridHeightInPixels * Window.GridSize.height;
                x = (float)window.X / Window.WindowSize.width * Window.GridSize.width;
            }
            else
            {
                //window is thick
                float gridWidthInPixels = Window.WindowSize.height / Window.GridSize.slobe;
                float offset = (Window.WindowSize.width - gridWidthInPixels) / 2;
                x = (window.X - offset) / gridWidthInPixels * Window.GridSize.width;
                y = (float)window.Y / Window.WindowSize.height * Window.GridSize.height;
            }

            //invert y
            y = Window.GridSize.height - y - 1;

            // TODO: Fix the root of the offset issue
            x -= Window.GridSize.width / 2;
            y -= Window.GridSize.height / 2;


            return new Vector2((int)x, (int)y);
        }

        #endregion

        #region Button

        /// <summary>
        /// Checks if any mouse button is held down
        /// </summary>
        public static bool IsAnyButtonDown()
        {
            return currentMouse.IsAnyButtonDown;
        }
        
        /// <summary>
        /// Checks if a mouse button is held down
        /// </summary>
        public static bool GetButton(MouseButton button)
        {
            return currentMouse.IsButtonDown(ConvertButton(button));
        }

        /// <summary>
        /// Checks if any of the given keys is held down
        /// </summary>
        public static bool GetAnyButtons(params MouseButton[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++) if (GetButton(buttons[i])) return true;
            return false;
        }
        
        /// <summary>
        /// Checks if all of the given buttons are held down
        /// </summary>
        public static bool GetAllButtons(params MouseButton[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++) if (!GetButton(buttons[i])) return false;
            return true;
        }

        #endregion

        #region Button Down

        /// <summary>
        /// Checks if a button has been pressed
        /// </summary>
        public static bool GetButtonDown(MouseButton button)
        {
            var con = ConvertButton(button);
            return currentMouse[con] && !previousMouse[con];
        }

        /// <summary>
        /// Checks if any of the given buttons has been pressed
        /// </summary>
        public static bool GetAnyButtonsDown(params MouseButton[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++) if (GetButtonDown(buttons[i])) return true;
            return false;
        }

        /// <summary>
        /// Checks if all of the given buttons has been pressed
        /// </summary>>
        public static bool GetAllButtonsDown(params MouseButton[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++) if (!GetButtonDown(buttons[i])) return false;
            return true;
        }

        #endregion

        #region Button Up

        /// <summary>
        /// Checks if a button has been released
        /// </summary>
        public static bool GetButtonUp(MouseButton button)
        {
            var con = ConvertButton(button);
            return !currentMouse[con] && previousMouse[con];
        }

        /// <summary>
        /// Checks if any of the given buttons has been released
        /// </summary>
        public static bool GetAnyButtonsUp(params MouseButton[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++) if (GetButtonUp(buttons[i])) return true;
            return false;
        }

        /// <summary>
        /// Checks if all of the given buttons has been released
        /// </summary>>
        public static bool GetAllButtonsUp(params MouseButton[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++) if (!GetButtonUp(buttons[i])) return false;
            return true;
        }

        #endregion

        internal static void Update()
        {
            previousKeyboard = currentKeyboard;
            previousMouse = currentMouse;
            currentKeyboard = Keyboard.GetState();
            currentMouse = Mouse.GetCursorState();
            //if (IsCursorLocked) SetGridMousePosition(Screen.GridSize.width/2, Screen.GridSize.height / 2);
        }

        internal static void CharTyped(char c)
        {
            OnCharTyped?.Invoke(c);
        }

        private static OpenTK.Input.Key Convert(Key key)
        {
            return (OpenTK.Input.Key)key;
        }

        private static OpenTK.Input.MouseButton ConvertButton(MouseButton button)
        {
            return (OpenTK.Input.MouseButton)button;
        }
    }
}