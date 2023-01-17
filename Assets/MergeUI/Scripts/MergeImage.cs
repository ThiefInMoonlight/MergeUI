using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace MergeUI
{
    public class MergeImage : Image
    {
        #region Property

        

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
                
            }
        }
        
        /// <summary>
        /// you can override this func to implement more uvs operation 
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

        #endregion
    }
}