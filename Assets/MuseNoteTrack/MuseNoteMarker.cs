using System;
using UnityEngine;

public abstract class MuseNoteMarker : UnityEngine.Timeline.Marker 
{
    [SerializeField]
    public string guid = Guid.NewGuid().ToString();

    public MuseNoteMarker()
    {
    }

}
