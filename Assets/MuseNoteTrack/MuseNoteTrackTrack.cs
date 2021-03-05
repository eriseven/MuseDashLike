using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.8784314f, 0.4117647f, 0.05882353f)]
[TrackClipType(typeof(MuseNoteTrackClip))]
public class MuseNoteTrackTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<MuseNoteTrackMixerBehaviour>.Create (graph, inputCount);
    }
}
