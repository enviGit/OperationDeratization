using Tools.TimelineSubtitles;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace HF.IntroCutscene {
    [CustomTimelineEditor(typeof(SubtitlesLine))]
    public class SubtitleClipNamesUpdater : ClipEditor {
        public override void OnClipChanged(TimelineClip abstractClip) {
            var clip = abstractClip.asset as SubtitlesLine;
            abstractClip.displayName = $"\"{clip.text}\"";
        }
    }
}