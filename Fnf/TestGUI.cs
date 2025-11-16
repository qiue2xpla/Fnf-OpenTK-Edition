using Fnf.Framework;
using Fnf.Framework.Graphics;
using Fnf.Framework.TrueType.Rendering;

public class TestGUI : GUI, IRenderable
{
    public bool isRenderable { get; set; } = true;

    Text text;

    public TestGUI(FontAtlas atlas)
    {
        width = 200;
        height = 200;
        globalPosition = new Vector2(0, -200);
        text= new Text(atlas);
        text.parent = this;
        text.color = Color.Black;

        Window.Fps = 5;
    }

    public void Render()
    {
        if (IsMouseOverGUI()) RaycastHit();
        Gizmos.DrawRoundQuad(this, Color.White, 5, 2);
        text.text = Input.GetGridMouseDelta().ToString();
        text.Render();
        OpenGL.Color3(1, 0, 0);
        Gizmos.DrawPoints(5, Input.GetGridMousePosition());
    }

    protected override void OnMouseHover()
    {
        text.text += "Hover\n";
    }

    protected override void OnMouseMove()
    {
        text.text += "Move\n";
    }

    protected override void OnMouseDown(MouseButton button)
    {
        text.text += "Down\n";
    }

    protected override void OnMouseUp(MouseButton button)
    {
        text.text += "Up\n";
    }

    protected override void OnMouseDrag(MouseButton button)
    {
        text.text += "Drag\n";
        localPosition += Input.GetGridMouseDelta();
    }

    protected override void OnMouse(MouseButton button)
    {
        text.text += "Hold\n";
    }
}