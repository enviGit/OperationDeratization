using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Tools.TimelineSubtitles {
    public class SubtitlesLine : PlayableAsset, ITimelineClipAsset {
        public string text;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            var playable = ScriptPlayable<SubtitlesLineBehaviour>.Create(graph);
            var subsLineBehaviour = playable.GetBehaviour();
            subsLineBehaviour.text = text;
            return playable;
        }

        public ClipCaps clipCaps => ClipCaps.None;

    }
}
