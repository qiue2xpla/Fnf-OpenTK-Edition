using Fnf.Framework.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System;
using OpenTK.Platform.Windows;

namespace Fnf.Framework
{
    public static class Window
    {
        public static Point Position
        {
            get => new Point(_window.Location.X, _window.Location.Y);
            set => _window.Location = new System.Drawing.Point(value.x, value.y);
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

        public static bool IsFullscreen
        {
            get => _window.WindowState == WindowState.Fullscreen;
            set
            {
                _window.WindowBorder = value ? WindowBorder.Hidden : WindowBorder.Resizable;
                _window.WindowState = value ? WindowState.Fullscreen : WindowState.Normal;
            }
        }

        public static int Fps
        {
            get => (int)_window.TargetUpdateFrequency;
            set
            {
                _window.TargetUpdateFrequency = value;
                _window.TargetRenderFrequency = value;
            }
        }

        // Converts pixel space [-GridWidth/2, GridWidth/2] to viewport space [-1, 1] for opengl
        // TODO: Make it a seperate class
        public static float PixelToViewportHorizontal(float x) => 2 * x / GridSize.width;
        public static float PixelToViewportVertical(float y) => 2 * y / GridSize.height;
        public static Vector2 PixelToViewport(Vector2 vector) => PixelToViewport(vector.x, vector.y);
        public static Vector2 PixelToViewport(float x, float y) =>
            new Vector2(
                PixelToViewportHorizontal(x),
                PixelToViewportVertical(y));

        internal static WindowObject _window;
        private static bool _isGridFixed = false;
        private static Size _gridSize = new Size(1366, 768);

        public static void Initiate() { if (_window == null) _window = new WindowObject(); }
        public static void Run()      { _window?.Run(); }
        public static void Close()    { _window?.Close(); }
    }

    // Inhirating is needed because it doesn't have event based calls
    class WindowObject : GameWindow
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe");

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
           // GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

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
            if (Window.IsGridFixed)
            {
                // Free screen aspect ratio is not supported because there is not need for it
                if (Window.WindowSize.slobe >= Window.GridSize.slobe)
                {
                    float GridWidthInPixels = Width * Window.GridSize.slobe;
                    GL.Viewport(0, (int)(Height - GridWidthInPixels) / 2, Width, (int)GridWidthInPixels);
                }
                else
                {
                    float GridHeightInPixels = Height / Window.GridSize.slobe;
                    GL.Viewport((int)(Width - GridHeightInPixels) / 2, 0, (int)GridHeightInPixels, Height);
                }
            }
            else
            {
                Window.GridSize = Window.WindowSize;
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
                GUI.InvokeEvents();
            }

            if (Input.GetKeyDown(Key.F11))
            {
                Window.IsFullscreen = !Window.IsFullscreen;
            }

            if (Input.GetAnyKeys(Key.AltLeft, Key.AltRight) && Input.GetAnyKeysDown(Key.Enter, Key.KeypadEnter))
            {
                Window.IsFullscreen = !Window.IsFullscreen;
            }

            if (Window.IsFullscreen && Input.GetAnyKeysDown(Key.WinLeft, Key.WinRight))
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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Script.RenderScript();
            Context.SwapBuffers();
        }
    }
}