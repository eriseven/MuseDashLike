using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickNoteRenderer : MonoBehaviour
{
    private void Awake()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [EasyButtons.Button]
    void CheckQuadCenter()
    {
        var mesh = GetComponent<MeshFilter>().sharedMesh;
        Debug.Log(mesh.vertices);
    }
}
