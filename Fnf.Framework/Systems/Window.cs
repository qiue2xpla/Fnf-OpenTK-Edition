using Fnf.Framework.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System;

namespace Fnf.Framework
{
    public static class Window
    {
        public static Point Position
        {
            get => new Point(CurrentWindow.Location.X, CurrentWindow.Location.Y);
            set => CurrentWindow.Location = new System.Drawing.Point(value.x, value.y);
        }
        public static bool IsGridFixed
        {
            get => _isGridFixed;
            set
            {
                if (_isGridFixed == value) return;
                _isGridFixed = value;
                _window.ResizeBase();
            }
        }
        public static Size WindowSize
        {
            get => new Size(_window.Width, _window.Height);
            set
            {
                _window.Width = value.width;
                _window.Height = value.height;
            }
        }
        public static Size GridSize
        {
            get => _gridSize;
            set
            {
                if (_isGridFixed) _gridSize = value;
            }
        }
        // TODO: This is broken on muliple displays
        public static Size ScreenSize => new Size(DisplayDevice.Default.Width, DisplayDevice.Default.Height);
        public static string Title
        {
            get => _window.Title;
            set => _window.Title = value;
        }

        internal static WindowObject _window;
        private static bool _isGridFixed = false;
        private static Size _gridSize = new Size(1366, 768);

        public static void Initiate() { if (_window == null) _window = new WindowObject(); }
        public static void Run()      { if (_window != null) _window.Run(); }
        public static void Exit()     { if (_window != null) _window.Close(); }
    }

    // Inhirating is needed because it doesn't have event based calls
    class WindowObject : GameWindow
    {
        #region Window Shit

        

        

        public static bool isFullscreen
        {
            get => CurrentWindow.WindowState == WindowState.Fullscreen;
            set
            {
                CurrentWindow.WindowBorder = value ? WindowBorder.Hidden : WindowBorder.Resizable;
                CurrentWindow.WindowState = value ? WindowState.Fullscreen : WindowState.Normal;
            }
        }

        #endregion

        #region Viewport

        // Converts pixel space [-GridWidth/2, GridWidth/2] to viewport space [-1, 1] for opengl
        public static float PixelToViewportHorizontal(float x) => 2 * x / GridSize.width;
        public static float PixelToViewportVertical(float y) => 2 * y / GridSize.height;
        public static Vector2 PixelToViewport(Vector2 vector) => PixelToViewport(vector.x, vector.y);
        public static Vector2 PixelToViewport(float x, float y) =>
            new Vector2(
                PixelToViewportHorizontal(x),
                PixelToViewportVertical(y));

        #endregion

        #region Events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe");

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Gizmos.LoadGizmoz();
            Script.StartScript();
            Time.Start();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            ResizeBase();
        }

        public void ResizeBase()
        {
            if (_isGridFixed)
            {
                // Free screen aspect ratio is not supported because there is not need for it
                if (WindowSize.slobe >= GridSize.slobe)
                {
                    float GridWidthInPixels = Width * GridSize.slobe;
                    GL.Viewport(0, (int)(Height - GridWidthInPixels) / 2, Width, (int)GridWidthInPixels);
                }
                else
                {
                    float GridHeightInPixels = Height / GridSize.slobe;
                    GL.Viewport((int)(Width - GridHeightInPixels) / 2, 0, (int)GridHeightInPixels, Height);
                }
            }
            else
            {
                GridSize = WindowSize;
                GL.Viewport(0, 0, Width, Height);
            }

            Script.ResizeScript();
        }



        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            Input.CharTyped(e.KeyChar);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            Time.Update();

            if (Input.AllowUnfocusedInput || Focused)
            {
                Input.Update();
                UI.InvokeEvents();
            }

            if (Input.GetKeyDown(Key.F11))
            {
                isFullscreen = !isFullscreen;
            }

            if (Input.GetAnyKeys(Key.AltLeft, Key.AltRight) && Input.GetAnyKeysDown(Key.Enter, Key.KeypadEnter))
            {
                isFullscreen = !isFullscreen;
            }

            if (isFullscreen && Input.GetAnyKeysDown(Key.WinLeft, Key.WinRight))
            {
                WindowState = OpenTK.WindowState.Minimized;
            }

            if (Input.GetAnyKeys(Key.AltLeft, Key.AltRight) && Input.GetKeyDown(Key.F4))
            {
                Exit();
            }

            Script.UpdateScript();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            UI.Raycast();
            Script.RenderScript();
            Context.SwapBuffers();
        }

        #endregion
    }

}