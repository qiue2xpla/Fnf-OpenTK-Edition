namespace Fnf.Framework
{
    public static class Anchor
    {
        public static void PositionUILocaly(UI ui , AnchorType anchor, Vector2 padding)
        {
            ui.localPosition = GetPos(ui, anchor, padding);
        }

        private static Vector2 GetPos(UI ui, AnchorType anchor, Vector2 padding)
        {
            Vector2 pos = Vector2.Zero;

            switch (anchor)
            {
                case AnchorType.TopLeft:
                case AnchorType.TopRight:
                case AnchorType.TopCenter:
                    pos.y = (Window.GridSize.height - ui.height) / 2 - padding.y;
                    break;

                case AnchorType.MiddleLeft:
                case AnchorType.MiddleRight:
                case AnchorType.MiddleCenter:
                    pos.y = 0;
                    break;

                case AnchorType.DownLeft:
                case AnchorType.DownRight:
                case AnchorType.DownCenter:
                    pos.y = (ui.height - Window.GridSize.height) / 2 + padding.y;
                    break;
            }

            switch (anchor)
            {
                case AnchorType.TopLeft:
                case AnchorType.MiddleLeft:
                case AnchorType.DownLeft:
                    pos.x = (ui.width - Window.GridSize.width) / 2 + padding.x;
                    break;

                case AnchorType.TopRight:
                case AnchorType.MiddleRight:
                case AnchorType.DownRight:
                    pos.x = (Window.GridSize.width - ui.width) / 2 - padding.x;
                    break;

                case AnchorType.TopCenter:
                case AnchorType.MiddleCenter:
                case AnchorType.DownCenter:
                    pos.x = 0;
                    break;
            }

            return pos;
        }
    }

    public enum AnchorType
    {
        TopRight, TopLeft, TopCenter,
        MiddleRight, MiddleLeft, MiddleCenter,
        DownRight, DownLeft, DownCenter,
    }
}