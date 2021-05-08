using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultImage : MonoBehaviour
{
    [SerializeField]
    Image img;

    float autoHideTime = 0;
    public Sprite sprite
    {
        set
        {
            img.sprite = value;
            if (value == null)
            {
                img.enabled = false;
            }
            else
            {
                img.SetNativeSize();
                autoHideTime = 4;
                img.enabled = true;
            }
        }

        get
        {
            return img?.sprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (autoHideTime <= 0) return;
        autoHideTime -= Time.deltaTime;
        if (autoHideTime <= 0)
        {
            img.enabled = false;
        }
    }
}
