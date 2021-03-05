using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRNodeClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("OnRNodeClick Pressed");
        }

        if (context.canceled)
        {
            Debug.Log("OnRNodeClick Released");
        }

    }
}
