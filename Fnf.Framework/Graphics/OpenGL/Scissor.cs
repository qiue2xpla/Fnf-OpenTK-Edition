using OpenTK.Graphics.OpenGL;

namespace Fnf.Framework.Graphics
{
    public static class Scissor
    {
        public static void SetActiveState(bool active)
        {
            if (active) GL.Enable(EnableCap.ScissorTest);
            else GL.Disable(EnableCap.ScissorTest);
        }

        public static bool GetActiveState()
        {
            return GL.IsEnabled(EnableCap.ScissorTest);
        }

        public static void SetBoundBoxOnWindow(int x, int y, int width, int height)
        {
            GL.Scissor(x, y, width, height);
        }

        public static void SetBoundBoxOnGrid(int x, int y, int width, int height)
        {
            if(Window.WindowSize.slobe > Window.GridSize.slobe) // tall
            {
                float gridHeightInWindow = Window.GridSize.slobe * Window.WindowSize.width;

                float newWidth = (float)width / Window.GridSize.width * Window.WindowSize.width;
                float newHeight = (float)height / Window.GridSize.height * gridHeightInWindow;

                float newY = (Window.WindowSize.height - gridHeightInWindow) / 2 + (float)y / Window.GridSize.height * gridHeightInWindow;
                float newX = (float)x / Window.GridSize.width * Window.WindowSize.width;

                GL.Scissor((int)newX, (int)newY, (int)newWidth, (int)newHeight);
            }
            else
            {
                float gridWidthInWindow = Window.WindowSize.height / Window.GridSize.slobe;

                float newWidth = (float)width / Window.GridSize.width * gridWidthInWindow;
                float newHeight = (float)height / Window.GridSize.height * Window.WindowSize.height;

                float newX = (Window.WindowSize.width - gridWidthInWindow) / 2 + (float)x / Window.GridSize.width * gridWidthInWindow;
                float newY = (float)y / Window.GridSize.height * Window.WindowSize.height; 

                GL.Scissor((int)newX, (int)newY, (int)newWidth, (int)newHeight);
            }
        }
    }
}