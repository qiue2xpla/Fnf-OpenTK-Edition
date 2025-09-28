using Fnf.Framework;
using Fnf.Framework.TrueType;
using Fnf.Framework.TrueType.Rasterization;
using System;

public class Editor : Script
{
    Text text;
    Button button;

    void Start()
    {
        Font font = new Font("arial");
        FontAtlas atlas = new FontAtlas(font, 120, 2, 2, 0, FontAtlas.Space+FontAtlas.UpperCase+FontAtlas.LowerCase+FontAtlas.Ponctuals+FontAtlas.Numbers);
        text = new Text(atlas);

        text.width = 1000;
        text.height = 600;
        text.textAlignment = TextAlignment.Top;

        button = new Button();
        button.overlayText.atlas = atlas;
        button.overlayText.text = "Click me!";
        button.width = 200;
        button.height = 45;
        button.isUpdatable = true;

    }

    void Update()
    {
        text.text = $"'{(int)Math.Round(1 / Time.deltaTime)}' fps per second";
        text.fontSize += Input.GetScrollWheelDelta();

        button.Update();
    }

    void Render()
    {
        text.Render();
        button.Render();
    }
}