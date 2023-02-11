using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace MergeUI.Editor
{
    [InitializeOnLoad]
    public static class MergeUIEditorHelper
    {
        static MergeUIEditorHelper()
        {
            PrefabStage.prefabSaved -= OnPrefabStageSaved;
            PrefabStage.prefabSaved += OnPrefabStageSaved;
        }

        private static void OnPrefabStageSaved(GameObject obj)
        {
            var renders = obj.GetComponentsInChildren<MergeUIRender>();
            if (renders.Length == 0)
                return;

            var mergeUis = obj.GetComponentsInChildren<IMerge>();
            if(mergeUis.Length == 0)
                return;

            var changes = false;
            foreach (var merge in mergeUis)
            {
                var result = ResetMergeData(merge);
                if (result)
                    changes = true;
            }

            if (!changes)
                return;

            foreach (var render in renders)
            {
                render.ReSortingInEditor();
            }

            PrefabUtility.SavePrefabAsset(obj);
        }

        private static bool ResetMergeData(IMerge merge)
        {
            var trans = merge.GetTransform();
            var root = trans;
            MergeUIRender render = null;
            while (render != null || root == null)
            {
                render = root.GetComponent<MergeUIRender>();
                root = root.parent;
            }

            if (render == null)
                return false;

            var newPath = MergeUIMgr.GetPath(trans, root);
            var oldPath = merge.GetPath();
            var oldRender = merge.GetMergeRender();
            var pathResult = string.Equals(trans, root);
            var renderResult = oldRender == render;
            if (pathResult && renderResult)
            {
                // no changes
                return false;
            }

            var graphic = trans.GetComponent<Graphic>();
            if (oldRender != null)
                oldRender.UnRegister(graphic, merge);
            render.Register(graphic, merge);
            merge.SetPath(newPath);
            merge.SetMergeRender(render);
            return true;
        }



        private static StringBuilder _sb = new StringBuilder();
    }
}