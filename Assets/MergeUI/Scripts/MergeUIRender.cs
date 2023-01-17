using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeUI
{
    public class MergeUIRender : MonoBehaviour
    {
        #region Init

        protected void Start()
        {
            _transform = transform;
        }

        #endregion

        #region UpdateMesh

        protected void LateUpdate()
        {
            
        }

        #endregion


        #region Field

        private static Mesh _emptyMesh = new Mesh();

        private Transform _transform;

        private bool _dirty;

        #endregion
    }
}