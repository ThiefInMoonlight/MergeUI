using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class AutoSet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!_init)
        {
            Init();
            _init = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!_init || !NeedUpdate)
            return;

        foreach (var graphic in _images)
        {
            bool flag = Random.value > 0.5f;
            graphic.enabled = flag;
        }

        foreach (var text in _texts)
        {
            bool flag = Random.value > 0.5f;
            text.enabled = flag;
        }
    }

    private void Init()
    {
        _images = GetComponentsInChildren<Image>();
        _texts = GetComponentsInChildren<Text>();
    }

    private bool _init = false;

    private Image[] _images;

    private Text[] _texts;

    public bool NeedUpdate = false;
}
