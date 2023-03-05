using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
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
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }

        #region Inerface

        public static MergeUIAtlasInfo ParseRuntimeData(SpriteAtlas atlas, List<MergeUIEditorSettingAtlasInfo> list)
        {
            var atlasInfo = new MergeUIAtlasInfo();
            foreach (var data in list)
            {
                var matInfo = new MergeUIMatInfo();
                var mat = data.Mat;
                var matPath = AssetDatabase.GetAssetPath(mat);
                matInfo.MaterialField = data.Field;
                matInfo.MaterialPath = matPath;
                atlasInfo.MatInfos.Add(matInfo);
            }

            atlasInfo.AtlasName = atlas.name;
            atlasInfo.AtlasPath = AssetDatabase.GetAssetPath(atlas);
            atlasInfo.FirstSpriteInAtlas = GetFirstImageInAtlas(atlas);
            
            return atlasInfo;
        }
        
        public static void ShowSaveFailedDialog(string str)
        {
            EditorUtility.DisplayDialog("Merge UI Setting Save Failed",
                str, "yes");
        }

        #endregion

        #region Method

        private static void OnPrefabStageSaved(GameObject obj)
        {
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if(stage == null)
                return;
            
            var renders = obj.GetComponentsInChildren<MergeUIRender>(true);
            if (renders.Length == 0)
                return;

            var mergeUis = obj.GetComponentsInChildren<IMerge>(true);
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

            PrefabUtility.SaveAsPrefabAsset(obj, stage.assetPath);
        }

        private static bool ResetMergeData(IMerge merge)
        {
            var trans = merge.GetTransform();
            var root = trans;
            MergeUIRender render = root.GetComponent<MergeUIRender>();
            while (render == null && root != null)
            {
                root = root.parent;
                render = root.GetComponent<MergeUIRender>();
            }

            if (render == null)
                return false;

            var newPath = MergeUIMgr.GetPath(trans, root);
            var oldPath = merge.GetPath();
            var oldRender = merge.GetMergeRender();
            var pathResult = string.Equals(newPath, oldPath);
            var renderResult = oldRender == render;
            if (pathResult && renderResult)
            {
                // no changes
                return false;
            }

            var graphic = trans.GetComponent<Graphic>();
            if (oldRender != null)
                oldRender.UnRegister(graphic, merge);
            merge.SetPath(newPath);
            render.Register(graphic, merge);
            merge.SetMergeRender(render);
            return true;
        }

        private static string GetFirstImageInAtlas(SpriteAtlas atlas)
        {
            var atlasPacks = atlas.GetPackables();
            if (atlasPacks.Length == 0)
            {
                return string.Empty;
            }

            // get first packed image in atlas, and get its name
            foreach (var pack in atlasPacks)
            {
                var path = AssetDatabase.GetAssetPath(pack);
                // folder : get first image in it
                if (Directory.Exists(path))
                {
                    var result = SearchPackableInFolder(path, out var image);
                    if (result)
                        return image;
                }
                // image : get its name
                else
                {
                    return GetPackableFileName(path);
                }
            }
            
            Debug.LogError($"[MergeUI] {atlas.name} cannot get valid packable Object data, pls check");
            // search failed
            return string.Empty;
        }

        private static bool SearchPackableInFolder(string folderPath, out string image)
        {
            var files = Directory.GetFiles(folderPath);
            foreach (var path in files)
            {
                if (!Directory.Exists(path))
                {
                    image = GetPackableFileName(path);
                    return true;
                }
                else
                {
                    var result = SearchPackableInFolder(path, out image);
                    if (result)
                        return true;
                }
            }

            image = string.Empty;
            return false;
        }

        private static string GetPackableFileName(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }
        
        /// <summary>
        /// when stop play, reset all material Texture to null
        /// </summary>
        /// <param name="state"></param>
        private static void OnPlayModeStateChange(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingPlayMode)
            {
                return;
            }

            var setting = MergeUIEditorSetting.GetSettings();
            if(setting == null)
                return;
            
            if(setting.AtlasInfos == null)
                return;

            foreach (var atlasInfo in setting.AtlasInfos)
            {
                if(atlasInfo.Mat == null || string.IsNullOrEmpty(atlasInfo.Field))
                    continue;
                
                atlasInfo.Mat.SetTexture(atlasInfo.Field, null);
            }
        }

        #endregion


        private static StringBuilder _sb = new StringBuilder();
    }
}