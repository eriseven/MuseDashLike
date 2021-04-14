using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
// [RequireComponent(typeof(LineRenderer))]
public class LongNoteRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private GameObject origin;

    [SerializeField] private GameObject destination;

    [SerializeField] private GameObject notePrefab;

    public void InitNote(Vector3 position, float duration)
    {
        // var note = GameObject.Instantiate(longClickNotePrefab, track);
        transform.localPosition = position;
        lineRenderer.transform.localScale = new Vector3(duration, 1, 1);
        var noteBegin = Instantiate(notePrefab, transform);
        noteBegin.transform.position = origin.transform.position;
        
        var noteEnd= Instantiate(notePrefab, transform);
        noteEnd.transform.position = destination.transform.position;
    }
    
    private void _Awake()
    {
        // if (lineRenderer == null)
        // {
        //     lineRenderer = GetComponent<LineRenderer>();
        // }
        //
        // if (lineRenderer == null)
        // {
        //     return;
        // }

        if (origin != null && destination != null)
        {
            origin.transform.localScale = Vector3.one;
            destination.transform.localScale = Vector3.one;
            // lineRenderer.SetPosition(0, origin.transform.position);
            // lineRenderer.SetPosition(1, destination.transform.position);
        }
    }

    // private void Start()
    // {
    //     // throw new NotImplementedException();
    // }

    private void _LateUpdate()
    {
        // if (lineRenderer == null)
        // {
        //     return;
        // }

        if (origin != null && destination != null)
        {
            origin.transform.localScale = Vector3.one;
            destination.transform.localScale = Vector3.one;
            // lineRenderer.SetPosition(0, origin.transform.position);
            // lineRenderer.SetPosition(1, destination.transform.position);
        }
    }

    [EasyButtons.Button]
    void UpdateLinePosition()
    {
        if (lineRenderer == null)
        {
            return;
        }

        if (origin != null && destination != null)
        {
            lineRenderer.SetPosition(0, origin.transform.position);
            lineRenderer.SetPosition(1, destination.transform.position);
        }
    }
}