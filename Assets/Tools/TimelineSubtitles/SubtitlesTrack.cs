using TMPro;
using Tools.TimelineSubtitles;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(SubtitlesLine))]
[TrackBindingType(typeof(TMP_Text))]
public class SubtitlesTrack : TrackAsset {
    PlayableTrack pt;

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
        return ScriptPlayable<SubtitlesLineMixer>.Create(graph, inputCount);
    }
}