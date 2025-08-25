namespace Fnf.Framework
{
    public class MovableObject
    {
        public MovableObject parent;

        public Vector2 globalPosition
        {
            get
            {
                if(parent == null) return localPosition;

                //default position
                Vector2 _globalPosition = localPosition;

                //apply scale
                _globalPosition *= parent.globalScale;

                //apply rotation
                _globalPosition = _globalPosition.Rotate(parent.globalRotation);

                //apply position
                _globalPosition += parent.globalPosition;

                return _globalPosition;
            }
            set
            {
                if (parent == null)
                {
                    localPosition = value;
                    return;
                }

                Vector2 _localPosition = value;
                _localPosition -= parent.globalPosition;
                _localPosition = _localPosition.Rotate(-parent.globalRotation);
                _localPosition = _localPosition / parent.globalScale;
                localPosition = _localPosition;
            }
        }

        public Vector2 globalScale
        {
            get => localScale * (parent != null ? parent.globalScale : Vector2.One);
            set => localScale = value / (parent != null ? parent.globalScale : Vector2.One);
        }

        public float globalRotation
        {
            get => localRotation + (parent != null ? parent.globalRotation : 0);
            set => localRotation = value - (parent != null ? parent.globalRotation : 0);
        }

        public Vector2 localPosition;
        public Vector2 localScale = new Vector2(1);
        public float localRotation;
    }
}