using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MuseNoteTrackClip : PlayableAsset, ITimelineClipAsset
{
    public MuseNoteTrackBehaviour template = new MuseNoteTrackBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override double duration => 0.1;

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<MuseNoteTrackBehaviour>.Create (graph, template);
        MuseNoteTrackBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
