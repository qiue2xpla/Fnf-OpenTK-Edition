using Fnf.Framework.TrueType.Rendering;
using Fnf.Framework.TrueType;
using Fnf.Framework;
using Fnf.Game;
using System;

public class Editor : Script
{
    StageContext stage;
    Text text;
    Button button;

    TabView tabView;

    int count;

    void Start()
    {
        Font font = new Font("arial.ttf");
        FontAtlas atlas = new FontAtlas(font, 120, 2, 2, 0, FontAtlas.GetCustomCharset("PULNS"));

        text = new Text(atlas) { width = 1000, height = 600, textAlignment = TextAlignment.Top };

        button = new Button(atlas);
        button.overlayText.text = "Click me!";
        button.width = 200;
        button.height = 70;
        button.cornerRadius = 7;
        button.borderSmoothness = 3;
        button.smoothColor = true;
        button.OnClick += delegate { count++; };

        tabView = new TabView(atlas);
        tabView.width = 400 - 8;
        tabView.height = Window.GridSize.height - 8;
        tabView.localPosition.x = -Window.GridSize.width / 2 + 400 / 2;

        tabView.items.Add(new TabViewItem("Stage", new Panel() { color =  Color.Red }));
        tabView.items.Add(new TabViewItem("Chart", new Panel() { color = Color.Green }));
        tabView.items.Add(new TabViewItem("Offset", new Panel() { color = Color.Blue }));

        stage = new StageContext("idk mane", "hard", new string[] { "Markov" });
    }

    void Update()
    {
        text.text = $"Pressed the button '{count}' times";
        text.fontSize += Input.GetScrollWheelDelta();

        stage.Update();

        tabView.Update();
    }

    void Render()
    {
        stage.Render();
        text.Render();
        button.Render();

        tabView.Render();
    }
}