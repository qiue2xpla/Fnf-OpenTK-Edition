using Fnf.Framework.Graphics;

namespace Fnf.Framework
{
    public class InputField : GUI, IRenderable, IUpdatable
    {
        public bool isRenderable { get; set; } = true;
        public bool isUpdatable { get; set; } = true;

        public Text textBox;

        public float borderSmoothness = 2;
        public float cornerRadius = 5;

        public Color backgroundColor = Color.White;
        public Color borderColor = new Color(190, 190, 190);

        float backSpaceTimer = 0;

        public InputField()
        {
            textBox = new Text();
            textBox.parent = this;
            textBox.textAlignment = TextAlignment.Left;
            textBox.fontSize = 30;
            textBox.color = Color.Black;
            textBox.isRaycastable = false;
            height = 40;

            Input.OnCharTyped += OnKeyPressed;
        }

        void OnKeyPressed(char c)
        {
            if (!isUpdatable) return;
            if (!isSelected) return; 
            textBox.text += c;
        }

        public void Update()
        {
            if (!isUpdatable) return;
            if (!isSelected) return;

            if(Input.GetKeyDown(Key.BackSpace))
            {
                // Delete Character
                if (textBox.text.Length != 0)
                    textBox.text = textBox.text.Substring(0, textBox.text.Length - 1);

                backSpaceTimer = -0.4f;
            }

            if(Input.GetKey(Key.BackSpace))
            {
                if (backSpaceTimer > 1 / 15f)
                {
                    backSpaceTimer -= 1 / 15f;

                    if (textBox.text.Length != 0)
                        textBox.text = textBox.text.Substring(0, textBox.text.Length - 1);
                }

                backSpaceTimer += Time.deltaTime;
            }
        }

        public void Render()
        {
            if (!isRenderable) return;
            if (IsOverGUI()) RaycastHit();

            globalRotation = 0;

            Gizmos.DrawRoundQuad(
                globalPosition,
                globalScale,
                width, height,
                globalRotation,
                cornerRadius,
                0,
                borderColor);

            Gizmos.DrawRoundQuad(
                globalPosition,
                globalScale,
                width - 2, height - 2,
                globalRotation,
                cornerRadius,
                borderSmoothness,
                backgroundColor);

            textBox.width = width - cornerRadius * 2;
            textBox.height = height - cornerRadius * 2;

            Scissor.SetActiveState(true);

            int scaledWidth = (int)(width * globalScale.x);
            int scaledHeight = (int)(height * globalScale.y);
            int x = (int)(globalPosition.x - scaledWidth / 2f);
            int y = (int)(globalPosition.y - scaledHeight / 2f);
            Scissor.SetBoundBoxOnGrid(x, y, scaledWidth, scaledHeight);

            textBox.Render();

            Scissor.SetActiveState(false);
        }
    }
}