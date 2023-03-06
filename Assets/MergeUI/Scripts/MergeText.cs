using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace MergeUI
{
    public class MergeText : Text, IMerge
    {
        
        #region MergeUI

        public Mesh GetMesh()
        {
            if (isActiveAndEnabled)
                return _mesh;

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
            if (dirty)
            {
                _lastPos = GetTransform().position;
                _lastRot = GetTransform().rotation;
                _lastScale = GetTransform().localScale;
                return true;
            }
            
            var tempPos = GetTransform().position;
            var tempRot = GetTransform().rotation;
            var tempScale = GetTransform().localScale;

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
            return GetTransform().localToWorldMatrix;
        }
        
        public Transform GetTransform()
        {
            if (_transform == null)
                _transform = transform;
            
            return _transform;
        }

#if UNITY_EDITOR

        public Material GetTempMaterial()
        {
            if (_tempMat == null)
            {
                _tempMat = new Material(Shader.Find( "Merge UI/Editor Font"));
            }

            Texture tex = null;
            if (font != null)
                tex = font.material.mainTexture;
            
            _tempMat.SetTexture("_MainTex", tex);
            return _tempMat;
        }

        public MeshFilter GetTempMeshFilter()
        {
            if (_tempMeshFilter == null)
            {
                _tempMeshFilter = gameObject.GetComponent<MeshFilter>();
                if(_tempMeshFilter == null)
                    _tempMeshFilter = gameObject.AddComponent<MeshFilter>();
                _tempMeshFilter.hideFlags = _meshHideflags;
            }
            
            return _tempMeshFilter;
        }

        public MeshRenderer GetTempMeshRenderer()
        {
            if (_tempMeshRenderer == null)
            {
                _tempMeshRenderer = gameObject.GetComponent<MeshRenderer>();
                if(_tempMeshRenderer == null)
                    _tempMeshRenderer = gameObject.AddComponent<MeshRenderer>();
                _tempMeshRenderer.hideFlags = _meshHideflags;
            }
            
            return _tempMeshRenderer;
        }
#endif
        #endregion

        #region Method

        protected override void Start()
        {
            base.Start();
            if (_uiRender != null)
            {
                _uiRender.SetDirty();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _transform = transform;
            if (_uiRender != null)
            {
                _uiRender.SetDirty();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_uiRender != null)
            {
                _uiRender.SetDirty();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_uiRender != null)
            {
                _uiRender.SetDirty();
            }
        }

        protected override void UpdateGeometry()
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.hideFlags = _meshHideflags;
            }
            
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
        
        internal static readonly HideFlags _meshHideflags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.HideInInspector;

        [SerializeField]
        private MergeUIRender _uiRender;
        
        [System.NonSerialized]
        private readonly VertexHelper _vertexHelper = new VertexHelper();

        [System.NonSerialized]
        private Mesh _mesh;

        private Transform _transform;

        private List<Vector4> _uv1s = new List<Vector4>();

        private static readonly Vector4 _defaultUV1 = new Vector4(1, 0, 0, 0);

        private string _path;

        private Vector3 _lastPos;

        private Quaternion _lastRot;

        private Vector3 _lastScale;
        
#if UNITY_EDITOR

        private Material _tempMat;

        private MeshRenderer _tempMeshRenderer;

        private MeshFilter _tempMeshFilter;
#endif

        #endregion
    }
}