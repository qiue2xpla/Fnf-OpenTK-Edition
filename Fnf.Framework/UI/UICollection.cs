using System.Collections.Generic;
using Fnf.Framework.Graphics;

namespace Fnf.Framework
{
    public class UICollection : UI, IRenderable
    {
        private Dictionary<UI, bool> isAutoPositioned = new Dictionary<UI, bool>();
        private List<UI> guis = new List<UI>();

        public Color color;
        public bool isRenderable { get; set; } = true;
        public bool movable = false;
        public float Padding = 5;
        public float Spacing = 5;

        public void Add(UI gui, bool autoPos)
        {
            guis.Add(gui);
            isAutoPositioned.Add(gui, autoPos);
        }

        public void Remove(UI gui)
        {
            guis.Remove(gui);
            isAutoPositioned.Remove(gui);
        }

        public void Clear()
        {
            foreach (var item in guis)
            {
                item.parent = null;
            }

            isAutoPositioned.Clear();
            guis.Clear();
        }

        public void Up(UI gui)
        {
            int index = guis.IndexOf(gui) - 1;
            if (index >= 0)
            {
                guis.Remove(gui);
                guis.Insert(index, gui);
            }
        }

        public void Down(UI gui)
        {
            int index = guis.IndexOf(gui) + 1;
            if (index < guis.Count)
            {
                guis.Remove(gui);
                guis.Insert(index, gui);
            }
        }

        public void Update()
        {
            foreach (UI gui in guis)
            {
                if (gui is IUpdatable updatable)
                {
                    updatable.Update();
                }
            }
        }

        public void Render()
        {
            if (!isRenderable) return;
            if(IsInside()) SetTopControl();

            Gizmos.DrawRoundQuad(globalPosition, globalScale, width, height, globalRotation, 5, 1, color);

            Vector2 cursor = new Vector2(0, height) / 2;
            cursor.y -= Padding;

            foreach(UI gui in guis)
            {
                gui.parent = this;

                if (isAutoPositioned[gui])
                {
                    gui.width = width - 2 * Padding;
                    gui.localPosition = new Vector2(gui.localPosition.x, -gui.height / 2) + cursor;
                    cursor.y -= Spacing + gui.height;
                }
                
                if(gui is IRenderable renderable)
                {
                    renderable.Render();
                }
            }
        }

        protected override void OnMouseDrag(MouseButton button)
        {
            if(movable) globalPosition += Input.GetGridMouseDelta();
        }
    }
}