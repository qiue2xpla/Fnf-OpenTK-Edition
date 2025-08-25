using Fnf.Framework.TrueType.Rasterization;
using Fnf.Framework.TrueType;
using Fnf.Framework.Graphics;
using Fnf.Framework.Audio;
using Fnf.Framework;
using System;

namespace Fnf.Game
{
    public class VolumeControl : UI, IRenderable, IUpdatable
    {
        public bool isUpdatable { get; set; } = true;
        public bool isRenderable { get; set; } = true;

        Text text;

        int volume
        {
            get => (int)Math.Round(ClipsManager.AppVolume * 10);
            set
            {
                float val = value / 10f;

                if (val > 1) val = 1;
                if (val < 0) val = 0;

                ClipsManager.AppVolume = val;
            }
        }

        float slide = 0;

        public VolumeControl()
        {
            Font font = new Font("Assets/Fonts/vcr");
            FontAtlas atlas = new FontAtlas(font, 70, 0, 3, 0, "Volume");

            text = new Text(atlas);
            text.parent = this;
            text.text = "Volume";
            text.fontSize = 28;
            text.textAlignment = TextAlignment.Center;
            text.localPosition = new Vector2(0, -12);
        }

        public void Update()
        {
            if (Input.GetKeyDown(Key.Minus))
            {
                slide = 5;
                volume--;
                if (volume < 0) volume = 0;

                ClipsManager.AppVolume = volume / 10f;
                new Clip("Assets/Shared/scrollMenu.ogg") { endAction = EndAction.Dispose}.play();
            }

            if (Input.GetKeyDown(Key.Plus))
            {
                slide = 5;
                volume++;
                if (volume > 10) volume = 10;

                ClipsManager.AppVolume = volume /10f;
                new Clip("Assets/Shared/scrollMenu.ogg") { endAction = EndAction.Dispose }.play();
            }

            if (slide < 0) return;

            slide -= Time.deltaTime*5;

            float slideOffset = slide;
            if (slideOffset > 1) slideOffset = 1;
            slideOffset = 1 - slideOffset;
            slideOffset *= height;

            height = 60;
            width = 160;
            localPosition = new Vector2(0, (Window.GridSize.height - height) / 2 + slideOffset);            
        }

        public void Render()
        {
            if (!isRenderable) return;
            if (slide < 0) return;

            Gizmos.DrawRoundQuad(globalPosition, globalScale, width, height, globalRotation, 0, 0, new Color(0, 0, 0, 140));
            text.Render();

            float vw = 116;

            float cursor = -vw / 2;

            float spacing = 4;
            float barWidth = 8;

            for (int i = 0; i < 10; i++)
            {
                float col = (i + 1) <= volume ? 1 : 0.6f;
                OpenGL.Color3(col, col, col);
                OpenGL.BeginDrawing(DrawMode.Quads);

                OpenGL.Pixel2(cursor + barWidth + globalPosition.x, (2 + 2 * i) + globalPosition.y);
                OpenGL.Pixel2(cursor            + globalPosition.x, (2 + 2 * i) + globalPosition.y);
                OpenGL.Pixel2(cursor            + globalPosition.x, 0 + globalPosition.y);
                OpenGL.Pixel2(cursor + barWidth + globalPosition.x, 0 + globalPosition.y);

                OpenGL.EndDrawing();

                cursor += barWidth + spacing;
            }

            OpenGL.Color3(1,1,1);
        }
    }
}