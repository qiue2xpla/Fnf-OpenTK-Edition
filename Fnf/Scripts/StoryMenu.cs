using Fnf.Framework;
using Fnf.Framework.TrueType;
using Fnf.Framework.TrueType.Rasterization;

namespace Fnf
{
    public class StoryMenu : Script
    {
        Text weekScore;
        Text weekName;

        void Start()
        {
            Font font = new("Assets/Fonts/vcr");
            FontAtlas atlas = new FontAtlas(font, 100, 3, 2, 0, 
                FontAtlas.UpperCase + FontAtlas.LowerCase + FontAtlas.Numbers + FontAtlas.Ponctuals + FontAtlas.Space);



            weekScore = new Text(atlas);
            weekScore.text = "WEEK SCORE:0";
            weekScore.textAlignment = TextAlignment.Left;
            weekScore.height = 60;
            weekScore.width = 1000;
            weekScore.fontSize = 37;

            weekName = new(atlas);
            weekName.text = "CUTIE";
            weekName.textAlignment = TextAlignment.Right;
            weekName.height = 60;
            weekName.width = 1000;
            weekName.fontSize = 37;
            weekName.color = new Color(178, 178, 178);

            Anchor.PositionUILocaly(weekScore, AnchorType.TopLeft, new Vector2(15, 0));
            Anchor.PositionUILocaly(weekName, AnchorType.TopRight, new Vector2(15, 0));

        }

        void Update ()
        {
            //text.localPosition = Input.GetGridMousePosition();
        }

        void Render()
        {
            weekScore.Render();
            weekName.Render();
        }
    }
}