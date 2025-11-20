using System.Collections.Generic;
using System;

namespace Fnf.Framework
{
    public class GameObject
    {
        public Vector3 localPosition
        {
            get => _localPosition;
            set
            {
                // Make sure the value changed
                if (_localPosition == value) return;

                // Set and mark as dirty
                _localPosition = value; MarkDirty();
            }
        }

        public Vector3 globalPosition
        {
            get
            {
                if (parent == null) return _localPosition;
                return (parent.worldMatrix * localPosition.ExtendW(1)).DropW();
            }
            set
            {
                if (parent == null) localPosition = value;
                else localPosition = (parent.worldInverse * value.ExtendW(1)).DropW();
            }
        }

        public Vector3 localScale
        {
            get => _localScale;
            set
            {
                if (_localScale == value) return;
                _localScale = value; 
                MarkDirty();
            }
        }

        public Vector3 globalScale
        {
            get
            {
                if (parent == null) return _localScale;
                return _localScale * parent.globalScale;
            }
            set
            {
                if (parent == null) localScale = value;
                else
                {
                    Vector3 parentScale = parent.globalScale;
                    localScale = new Vector3(
                        value.x / Math.Max(parentScale.x, 0.00001f),
                        value.y / Math.Max(parentScale.y, 0.00001f),
                        value.z / Math.Max(parentScale.z, 0.00001f));
                      
                }
            }
        }

        public Matrix4 localMatrix
        {
            get
            {
                if(localDirty)
                {
                    Vector3 radianRotation = localEuler * -MathUtility.ToRadian(1);
                    local = Matrix4.Transform(localPosition, radianRotation, localScale);
                    localInverse = Matrix4.InverseTransform(localPosition, radianRotation, localScale);
                    localDirty = false;
                }

                return local;
            }
        }

        public Matrix4 worldMatrix
        {
            get
            {
                if (parent == null) return localMatrix;

                if (worldDirty)
                {
                    world = parent.worldMatrix * localMatrix;
                    worldInverse = localInverse * parent.worldInverse;
                    worldDirty = false;
                }

                return world;
            }
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




        

        

        public float globalRotation
        {
            get => localRotation + (parent == null ? 0 : parent.globalRotation);
            set => localRotation = value - (parent == null ? 0 : parent.globalRotation);
        }

        public Vector3 localEuler;

        


        // Cache
        Vector3 _localPosition, _localScale;
        Matrix4 local, localInverse; bool localDirty;
        Matrix4 world, worldInverse; bool worldDirty;
        List<GameObject> _children;
        GameObject _parent;

        public GameObject()
        {
            local = Matrix4.Identity;
            world = Matrix4.Identity;
            localInverse = Matrix4.Identity;
            worldInverse = Matrix4.Identity;




            localScale = Vector3.One;
            localEuler = Vector3.Zero;

            _children = new List<GameObject>();
            children = new GameObject[0];
        }

        /*public Matrix3 LocalInverseTransformMatrix()
        {
            return Matrix3.InverseTransform(localPosition, -MathUtility.ToRadian(localRotation), localScale);
        }*/
        /*
        public Matrix3 WorldInverseTransformMatrix()
        {
            if (parent == null) return LocalInverseTransformMatrix();
            return parent.WorldInverseTransformMatrix() * LocalInverseTransformMatrix();
        }
        */

        void MarkDirty()
        {
            localDirty = true;
            worldDirty = true;
            Reqursive(this);

            void Reqursive(GameObject gameObject)
            {
                for (int i = 0; i < gameObject.children.Length; i++)
                {
                    gameObject.children[i].worldDirty = true;
                    Reqursive(gameObject.children[i]);
                }
            }
        }
    }
}