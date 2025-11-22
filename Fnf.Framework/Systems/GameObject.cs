using System.Collections.Generic;
using System;

namespace Fnf.Framework
{
    public class GameObject
    {
        #region Position

        Vector3 _localPosition;

        public Vector3 localPosition
        {
            get => _localPosition;
            set
            {
                if (_localPosition == value) return;
                _localPosition = value;
                PropertyChanged();
            }
        }

        public Vector3 worldPosition
        {
            get
            {
                if (parent == null) return _localPosition;
                return parent.worldMatrix * _localPosition;
            }
            set
            {
                if (parent == null) localPosition = value;
                else localPosition = parent.worldInverseMatrix * value;
            }
        }

        #endregion

        #region Rotation

        Quaternion _localQuaternion, _worldQuaternion; bool _worldQuaternionDirty;

        public Vector3 localRotation
        {
            get => Quaternion.ToEuler(_localQuaternion);
            set => localQuaternion = Quaternion.FromEuler(value);
        }

        public Vector3 worldRotation
        {
            get => Quaternion.ToEuler(worldQuaternion);
            set
            {
                if (parent == null) localRotation = value;
                else worldQuaternion = Quaternion.FromEuler(value);
            }
        }

        public Quaternion localQuaternion
        {
            get => _localQuaternion;
            set
            {
                Quaternion normalized = Quaternion.Normalize(value);
                if (_localQuaternion == normalized) return;
                _localQuaternion = normalized;
                PropertyChanged(true);
            }
        }

        public Quaternion worldQuaternion
        {
            get
            {
                if (parent == null) return _localQuaternion;

                if (_worldQuaternionDirty)
                {
                    _worldQuaternion = Quaternion.Normalize(parent.worldQuaternion * _localQuaternion);
                    _worldQuaternionDirty = false;
                }

                return _worldQuaternion;
            }
            set
            {
                if (parent == null) localQuaternion = value;
                else localQuaternion = Quaternion.Normalize(Quaternion.Inverse(parent.worldQuaternion) * value);
            }
        }

        #endregion

        #region Scale

        Vector3 _localScale;

        public Vector3 localScale
        {
            get => _localScale;
            set
            {
                if (_localScale == value) return;
                _localScale = value;
                PropertyChanged();
            }
        }

        public Vector3 worldScale
        {
            get
            {
                if (parent == null) return _localScale;
                return _localScale * parent.worldScale;
            }
            set
            {
                if (parent == null) localScale = value;
                else
                {
                    Vector3 parentScale = parent.worldScale;
                    if (parentScale.x == 0) parentScale.x = 0.0000001f;
                    if (parentScale.y == 0) parentScale.x = 0.0000001f;
                    if (parentScale.y == 0) parentScale.x = 0.0000001f;
                    localScale = value / parentScale;
                }
            }
        }

        #endregion

        #region Matrix

        Matrix4 _localMatrix, _localInverseMatrix; bool _localMatrixDirty, _localInverseMatrixDirty;
        Matrix4 _worldMatrix, _worldInverseMatrix; bool _worldMatrixDirty, _worldInverseMatrixDirty;

        public Matrix4 localMatrix
        {
            get
            {
                if (_localMatrixDirty)
                {
                    _localMatrix = Matrix4.TransformTRS(localPosition, localQuaternion, localScale);
                    _localMatrixDirty = false;
                }

                return _localMatrix;
            }
        }

        public Matrix4 worldMatrix
        {
            get
            {
                if (parent == null) return localMatrix;

                if (_worldMatrixDirty)
                {
                    _worldMatrix = parent.worldMatrix * localMatrix;
                    _worldMatrixDirty = false;
                }

                return _worldMatrix;
            }
        }

        public Matrix4 localInverseMatrix
        {
            get
            {
                if (_localInverseMatrixDirty)
                {
                    _localInverseMatrix = Matrix4.InverseTransformTRS(localPosition, localQuaternion, localScale);
                    _localInverseMatrixDirty = false;
                }

                return _localInverseMatrix;
            }
        }

        public Matrix4 worldInverseMatrix
        {
            get
            {
                if (parent == null) return localInverseMatrix;

                if (_worldInverseMatrixDirty)
                {
                    _worldInverseMatrix = parent.worldInverseMatrix * _localInverseMatrix;
                    _worldInverseMatrixDirty = false;
                }

                return _worldInverseMatrix;
            }
        }

        #endregion

        #region Parent and Child

        GameObject _parent;
        List<GameObject> _children;

        public GameObject parent
        {
            get => _parent;
            set
            {
                if (_parent == value) return;

                GameObject current = value;
                while (current != null)
                {
                    if (current == this) throw new InvalidOperationException("Cannot parent to a descendant");
                    current = current._parent;
                }

                if (_parent != null)
                {
                    if (_parent == value) return;
                    _parent._children.Remove(this);
                }

                if (value != null && !value._children.Contains(this))
                {
                    value._children.Add(this);
                }

                _parent = value;

                _worldMatrixDirty = true;
                _worldInverseMatrixDirty = true;
                _worldQuaternionDirty = true;

                ForeachNode(this, (GameObject child) =>
                {
                    child._worldMatrixDirty = true;
                    child._worldInverseMatrixDirty = true;
                    child._worldQuaternionDirty = true;
                });
            }
        }

        public IReadOnlyList<GameObject> children => _children;

        #endregion

        public GameObject()
        {
            _localScale = Vector3.One;

            _localMatrix = Matrix4.Identity;
            _worldMatrix = Matrix4.Identity;
            _localInverseMatrix = Matrix4.Identity;
            _worldInverseMatrix = Matrix4.Identity;

            _children = new List<GameObject>();
        }

        void PropertyChanged(bool rotationChanged = false)
        {
            _worldMatrixDirty = true;
            _localMatrixDirty = true;
            _localInverseMatrixDirty = true;
            _worldInverseMatrixDirty = true;
            if (rotationChanged) _worldQuaternionDirty = true;

            ForeachNode(this, (GameObject child) => {
                child._worldMatrixDirty = true;
                child._worldInverseMatrixDirty = true;
                if (rotationChanged) child._worldQuaternionDirty = true;
            });
        }

        void ForeachNode(GameObject gameObject, Action<GameObject> action)
        {
            for (int i = 0; i < gameObject.children.Count; i++)
            {
                GameObject child = gameObject.children[i];
                action.Invoke(child);
                ForeachNode(child, action);
            }
        }
    }
}