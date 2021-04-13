using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class LongNoteRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private GameObject origin;

    [SerializeField] private GameObject destination;

    private void _Awake()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

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

    private void _LateUpdate()
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