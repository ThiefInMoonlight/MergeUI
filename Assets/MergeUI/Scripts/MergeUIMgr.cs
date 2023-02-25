﻿using System;
using System.Collections.Generic;
using System.Text;
using PlasticGui;
using UnityEngine;

namespace MergeUI
{
    public class MergeUIMgr
    {
        #region Interface

        public static MergeUIMgr I
        {
            get
            {
                if (_instance == null)
                    _instance = new MergeUIMgr();

                return _instance;
            }
        }

        public void Init(bool debugModule, MergeUISetting setting, Func<string, Material> getMatFunc)
        {
            _debug = debugModule;
            _getMatFunc = getMatFunc;

            if (getMatFunc == null && setting == null)
            {
                Debug.LogError($"[MergeUI] Init failed, MergeUISetting {setting} or GetMatFunc is Null, pls check ");
                return;
            }


            _init = true;
        }

        public void DeInit()
        {
            _init = false;
        }

        /// <summary>
        /// Get Path of graphic and target RenderObj
        /// for sorting order
        /// </summary>
        /// <param name="graphic"></param>
        /// <param name="renderObj"></param>
        /// <returns></returns>
        public static string GetPath(Transform graphic, Transform renderObj)
        {
            if (_sb == null)
                _sb = new StringBuilder();
            _sb.Clear();
            var root = graphic;
            while (!Equals(root, renderObj) && root != null)
            {
                var index = root.transform.GetSiblingIndex();
                if (index < 10)
                {
                    _sb.Insert(0, $"0{index}");
                }
                else
                {
                    _sb.Insert(0, index);
                }
            }

            if (root == null)
            {
                Debug.LogError($"[MergeUI] {graphic} is not a child of {renderObj}, pls check");
            }

            return _sb.ToString();
        }

        #endregion


        #region Field

        private static MergeUIMgr _instance;

        private static StringBuilder _sb;

        private bool _init = false;

        private bool _debug = false;

        private Func<string, Material> _getMatFunc;

        private List<string> _atlas;

        #endregion
    }
}