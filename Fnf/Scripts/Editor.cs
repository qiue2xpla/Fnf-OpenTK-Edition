using Fnf.Framework;
using Fnf.Framework.TrueType;
using Fnf.Framework.TrueType.Rasterization;
using Fnf.Game;
using System;

public class Editor : Script
{
    StageContext stage;
    Text text;
    Button button;

    int count;

    void Start()
    {
        Font font = new Font("arial");
        FontAtlas atlas = new FontAtlas(font, 120, 2, 2, 0, FontAtlas.Space+FontAtlas.UpperCase+FontAtlas.LowerCase+FontAtlas.Ponctuals+FontAtlas.Numbers);
        text = new Text(atlas);

        text.width = 1000;
        text.height = 600;
        text.textAlignment = TextAlignment.Top;

        button = new Button(atlas);
        button.overlayText.text = "Click me!";
        button.width = 200;
        button.height = 70;
        button.cornerRadius = 7;
        button.borderSmoothness = 3;
        button.smoothColor = true;
        button.OnClick += delegate { count++; };

        stage = new StageContext("idk mane", "hard", new string[] { "Markov" });
    }

    void Update()
    {
        text.text = $"Pressed the button '{count}' times";
        text.fontSize += Input.GetScrollWheelDelta();

        stage.Update();
    }

    void Render()
    {
        stage.Render();
        text.Render();
        button.Render();
    }
}