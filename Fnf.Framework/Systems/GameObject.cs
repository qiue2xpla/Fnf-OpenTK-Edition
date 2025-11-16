using System.Collections.Generic;

namespace Fnf.Framework
{
    public class GameObject
    {
        public Vector2 globalPosition
        {
            get => parent == null ? localPosition : parent.WorldlTransformMatrix() * localPosition;
            set
            {
                if (parent == null) localPosition = value;
                else localPosition = parent.WorldInverseTransformMatrix() * value;
            }
        }
        public Vector2 globalScale
        {
            get => localScale * (parent == null ? Vector2.One : parent.globalScale);
            set => localScale = value / (parent == null ? Vector2.One : parent.globalScale);
        }
        public float globalRotation
        {
            get => localRotation + (parent == null ? 0 : parent.globalRotation);
            set => localRotation = value - (parent == null ? 0 : parent.globalRotation);
        }

        public GameObject parent
        {
            get => _parent;
            set
            {
                if (_parent != null)
                {
                    _parent._children.Remove(this);
                    _parent.children = _parent._children.ToArray();
                }

                if (value != null && !value._children.Contains(this))
                {
                    value._children.Add(this);
                    value.children = value._children.ToArray();
                }

                _parent = value;
            }
        }
        public GameObject[] children { get; private set; }

        public Vector2 localPosition = Vector2.Zero;
        public Vector2 localScale = Vector2.One;
        public float localRotation = 0;
        private List<GameObject> _children;
        private GameObject _parent;

        public GameObject()
        {
            _children = new List<GameObject>();
            children = new GameObject[0];
        }

        public Matrix3 LocalTransformMatrix()
        {
            return Matrix3.Transform(localPosition, -MathUtility.ToRadian(localRotation), localScale);
        }

        public Matrix3 WorldlTransformMatrix()
        {
            if (parent == null) return LocalTransformMatrix();
            return parent.WorldlTransformMatrix() * LocalTransformMatrix();
        }

        public Matrix3 LocalInverseTransformMatrix()
        {
            return Matrix3.InverseTransform(localPosition, -MathUtility.ToRadian(localRotation), localScale);
        }

        public Matrix3 WorldInverseTransformMatrix()
        {
            if (parent == null) return LocalInverseTransformMatrix();
            return parent.WorldInverseTransformMatrix() * LocalInverseTransformMatrix();
        }
    }
}