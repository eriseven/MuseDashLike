using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBillboardRenderer : MonoBehaviour
{
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        // _camera = Camera.current;
    }


    private void LateUpdate()
    {
        if (_camera != null)
        {
            transform.LookAt(transform.position + _camera.transform.forward, _camera.transform.up);
        }
    }
}
