using System.Text;
using PlasticGui;
using UnityEngine;

namespace MergeUI
{
    public class MergeUIMgr
    {
        #region Interface

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

        private static StringBuilder _sb;

        #endregion
    }
}