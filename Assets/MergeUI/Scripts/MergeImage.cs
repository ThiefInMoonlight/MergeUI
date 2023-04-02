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
            var mtrans = GetTransform();
            if (dirty)
            {
                _lastPos = mtrans.localPosition;
                _lastRot = mtrans.localRotation;
                _lastScale = mtrans.lossyScale;
                mtrans.hasChanged = false;
                return true;
            }

            if (!mtrans.hasChanged)
            {
                return false;
            }
            
            var tempPos = mtrans.localPosition;
            var tempRot = mtrans.localRotation;
            var tempScale = mtrans.lossyScale;

            if (_lastPos != tempPos || _lastRot != tempRot || _lastScale != tempScale)
            {
                _lastPos = tempPos;
                _lastRot = tempRot;
                _lastScale = tempScale;
                mtrans.hasChanged = false;
                return true;
            }

            mtrans.hasChanged = false;
            return false;
        }

        public Matrix4x4 GetMatrix()
        {
            return GetTransform().localToWorldMatrix;
        }
        
        public Transform GetTransform()
        {
            if (ReferenceEquals(_transform, null))
                _transform = transform;
            
            return _transform;
        }

#if UNITY_EDITOR

        public Material GetTempMaterial()
        {
            if (ReferenceEquals(_tempMat, null))
            {
                _tempMat = new Material( Shader.Find("Merge UI/Editor Image"));
            }

            Texture tex = null;
            if (sprite != null)
                tex = sprite.texture;
            
            _tempMat.SetTexture("_MainTex", tex);
            return _tempMat;
        }

        public MeshFilter GetTempMeshFilter()
        {
            if (ReferenceEquals(_tempMeshFilter, null))
            {
                _tempMeshFilter = gameObject.AddComponent<MeshFilter>();
                _tempMeshFilter.hideFlags = _meshHideflags;
            }
            
            return _tempMeshFilter;
        }

        public MeshRenderer GetTempMeshRenderer()
        {
            if (ReferenceEquals(_tempMeshRenderer, null))
            {
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

        #endregion

        #region MeshGenerate

        protected override void UpdateGeometry()
        {
            if (ReferenceEquals(_mesh, null))
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

            if (ReferenceEquals(_uiRender, null))
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

        public override Texture mainTexture
        {
            get
            {
                if(ReferenceEquals(sprite, null))
                    return s_WhiteTexture;

                if (ReferenceEquals(material, null))
                {
                    return s_WhiteTexture;
                }

                return material.mainTexture;
            }
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

        private static readonly Vector4 _defaultUV1 = new Vector4(0, 1, 0, 0);

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