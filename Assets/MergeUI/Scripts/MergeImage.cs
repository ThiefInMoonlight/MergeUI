using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace MergeUI
{
    public class MergeImage : Image, IMerge
    {
        #region MergeUI

        public Mesh GetMesh()
        {
            if (isActiveAndEnabled)
                return m_CachedMesh;

            return null;
        }

        public void SetMergeRender(MergeUIRender obj)
        {
            _uiRender = obj;
        }

        public MergeUIRender GetMergeRender()
        {
            return _uiRender;
        }

        public string GetPath()
        {
            return _path;
        }

        public void SetPath(string path)
        {
            _path = path;
        }

        public bool PosCheck(bool dirty)
        {
            var tempPos = _transform.position;
            var tempRot = _transform.rotation;
            var tempScale = _transform.localScale;
            
            if (dirty)
            {
                _lastPos = tempPos;
                _lastRot = tempRot;
                _lastScale = tempScale;
                return true;
            }

            if (_lastPos != tempPos && _lastRot != tempRot && _lastScale != tempScale)
            {
                _lastPos = tempPos;
                _lastRot = tempRot;
                _lastScale = tempScale;
                return true;
            }
            
            return false;
        }

        public Matrix4x4 GetMatrix()
        {
            return _transform.localToWorldMatrix;
        }

#if UNITY_EDITOR

        public Material GetTempMaterial()
        {
            throw new System.NotImplementedException();
        }
        
        public Transform GetTransform()
        {
            return transform;
        }
        
#endif

        #endregion

        #region Method

        protected override void OnEnable()
        {
            base.OnEnable();
            _transform = transform;
        }

        #endregion

        #region MeshGenerate

        protected override void UpdateGeometry()
        {
            if (rectTransform != null && rectTransform.rect.width >= 0 && rectTransform.rect.height >= 0)
                OnPopulateMesh(_vertexHelper);
            else
                _vertexHelper.Clear(); // clear the vertex helper so invalid graphics dont draw.

            var components = ListPool<Component>.Get();
            GetComponents(typeof(IMeshModifier), components);

            for (var i = 0; i < components.Count; i++)
                ((IMeshModifier)components[i]).ModifyMesh(_vertexHelper);

            ListPool<Component>.Release(components);

            _vertexHelper.FillMesh(_mesh);
            AdjustMesh();

            if (_uiRender == null)
            {
                canvasRenderer.SetMesh(_mesh);
            }
            else
            {
                _uiRender.SetDirty();
            }
        }
        
        /// <summary>
        /// you can override this method to implement more uvs operation 
        /// </summary>
        protected virtual void AdjustMesh()
        {
            _mesh.GetUVs(1, _uv1s);
            var count = _uv1s.Count;
            if(_uv1s.Count == 0)
                return;

            for (int i = 0; i < count; i++)
            {
                _uv1s[i] = _defaultUV1;
            }
            
            _mesh.SetUVs(1, _uv1s);
        }

        #endregion
        
        #region Field

        [System.NonSerialized]
        private MergeUIRender _uiRender;
        
        [System.NonSerialized]
        private readonly VertexHelper _vertexHelper = new VertexHelper();

        [System.NonSerialized]
        private Mesh _mesh;

        private Transform _transform;

        private List<Vector4> _uv1s = new List<Vector4>();

        private static readonly Vector4 _defaultUV1 = new Vector4(0, 1, 0, 0);

        private string _path;

        private Vector3 _lastPos;

        private Quaternion _lastRot;

        private Vector3 _lastScale;

        #endregion
    }
}