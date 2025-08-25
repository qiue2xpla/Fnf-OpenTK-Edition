using Fnf.Framework.TrueType.Rasterization;
using Fnf.Framework.TrueType;
using Fnf.Framework;

public class GUITest : Script
{
    Font font;
    FontAtlas atlas;

    UICollection collection = new UICollection();
    Text text;
    Button button;
    InputField inputField;

    void Start()
    {
        font = new Font("arial");
        atlas = new FontAtlas(font, 120, 4, 3, 0, FontAtlas.UpperCase + FontAtlas.LowerCase + FontAtlas.Space);

        collection.localPosition = Window.GridSize.ToVector2() / 2;
        collection.width = 400;
        collection.height = 600;
        collection.movable = true;
        collection.color = new Color(150, 150, 150, 255);

        text = new Text(atlas);
        text.height = 40;
        text.fontSize = 40;
        text.text = "Window";
        text.raycast = false;
        text.textAlignment = TextAlignment.TopLeft;

        button = new Button();
        button.overlayText.atlas = atlas;
        button.overlayText.fontSize = 40;
        button.overlayText.text = "Test";
        button.overlayText.color = Color.Black;
        button.overlayText.textAlignment = TextAlignment.Center;
        button.height = 40;
        button.isUpdatable = true;

        inputField = new InputField();
        inputField.textBox.atlas = atlas;

        collection.Add(text, true);
        collection.Add(button, true);
        collection.Add(inputField, true);
    }

    void Update()
    {
        collection.Update();
    }

    void Render()
    {
        collection.Render();
    }
}