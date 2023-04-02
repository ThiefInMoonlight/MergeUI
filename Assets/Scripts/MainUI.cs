using System;
using System.Collections;
using System.Collections.Generic;
using MergeUI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainUI : MonoBehaviour
{
    public int ObjectCount = 3000;
    public Button mergeUI3d_button;
    public Button mergeUIui_button;
    public Button normalui_button;
    public bool enableUpdate = true;
    
    
    // Start is called before the first frame update
    void Start()
    {
        mergeUI3d_button.onClick.AddListener((() =>
        {
            ChangeUI(UIType.MegerUI_3D);
        }));
        mergeUIui_button.onClick.AddListener((() =>
        {
            ChangeUI(UIType.MergeUI_UI);
        }));
        normalui_button.onClick.AddListener((() =>
        {
            ChangeUI(UIType.NormalUI);
        }));
    }

    // Update is called once per frame
    void Update()
    {
        if(!enableUpdate)
            return;
        
        var result = _uiMenus.TryGetValue(currentUI, out var obj);
        if(!result)
            return;

        var objects = obj.Item2;
        var width = Screen.width / 2;
        var height = Screen.height / 2;
        foreach (var go in objects)
        {
            var posX = Random.Range(-1f, 1f) * width;
            var posY = Random.Range(-1f, 1f) * height;
            go.transform.localPosition = new Vector3(posX, posY, 0);
        }
    }

    #region Method

    private void Init()
    {
        if(init)
            return;

        InitUI("MergeUI_3d", UIType.MegerUI_3D);
        InitUI("MergeUI_ui", UIType.MergeUI_UI);
        InitUI("NormalUI", UIType.NormalUI);

        init = true;
    }

    private void InitUI(string path, UIType type)
    {
        var prefab = Resources.Load<GameObject>(path);
        var root = GameObject.Instantiate(prefab);
        var content = root.transform.GetChild(0).gameObject;
        content.AddComponent<AutoSet>();
        
        
        var objects = new List<GameObject>();
        for (int i = 0; i < ObjectCount; i++)
        {
            var obj = GameObject.Instantiate(content);
            obj.name = i.ToString();
            obj.transform.SetParent(root.transform);
            obj.transform.localPosition = Vector3.zero;
            objects.Add(obj);
        }
        objects.Add(content);
        root.SetActive(false);
        _uiMenus.Add(type, (root, objects));
        root.transform.SetAsFirstSibling();
    }

    private void ChangeUI(UIType type)
    {
        if(type == currentUI)
            return;
        
        if(!init)
            Init();
        
        currentUI = type;
        foreach (var kv in _uiMenus)
        {
            var key = kv.Key;
            var root = kv.Value.Item1;
            var objects = kv.Value.Item2;
            if (key != currentUI)
            {
                root.SetActive(false);
                continue;
            }

            root.SetActive(true);
        }
    }

    #endregion

    #region Field

    private Dictionary<UIType, (GameObject, List<GameObject>)> _uiMenus = new Dictionary<UIType, (GameObject, List<GameObject>)>();

    private bool init = false;
    
    private UIType currentUI = UIType.None;

    #endregion
}

public enum UIType
{
    None,
    NormalUI,
    MegerUI_3D,
    MergeUI_UI
}