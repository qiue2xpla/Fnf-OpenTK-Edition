namespace Fnf.Framework
{
    /// <summary>
    /// An object that has a powerfull coodinate system <br/>
    /// Global values are values that the object would have it a parent was not assigned <br/>
    /// Local values are the main used values and they are based on the parent if present
    /// </summary>
    public class GameObject
    {
        public Vector2 globalPosition
        {
            get
            {
                if(parent == null) return localPosition;
                return (parent.GetObjectWorldlTransformMatrix() * localPosition.ToHomogeneous()).ToEuclidean();
            }
            set
            {
                if (parent == null) localPosition = value;
                else localPosition = (parent.GetObjectWorldInverseTransformMatrix() * value.ToHomogeneous()).ToEuclidean();
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

        public Vector2 localPosition = Vector2.Zero;
        public Vector2 localScale = Vector2.One;
        public float localRotation = 0;

        public GameObject parent;

        public Matrix3x3 GetObjectLocalTransformMatrix()
        {
            return Matrix3x3.CreateTransformMatrix(localPosition, -MathUtility.ToRadian(localRotation), localScale);
        }

        public Matrix3x3 GetObjectWorldlTransformMatrix()
        {
            if (parent == null) return GetObjectLocalTransformMatrix();
            return parent.GetObjectWorldlTransformMatrix() * GetObjectLocalTransformMatrix();
        }

        public Matrix3x3 GetObjectLocalInverseTransformMatrix()
        {
            return Matrix3x3.CreateInverseTransformMatrix(localPosition, -MathUtility.ToRadian(localRotation), localScale);
        }

        public Matrix3x3 GetObjectWorldInverseTransformMatrix()
        {
            if (parent == null) return GetObjectLocalInverseTransformMatrix();
            return parent.GetObjectWorldInverseTransformMatrix() * GetObjectLocalInverseTransformMatrix();
        }
    }
}