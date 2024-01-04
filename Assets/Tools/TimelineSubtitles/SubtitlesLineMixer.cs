using TMPro;
using UnityEngine.Playables;

namespace Tools.TimelineSubtitles {
    public class SubtitlesLineMixer : PlayableBehaviour {
        static TMP_Text previousSpeaker;

        public override void ProcessFrame(Playable playable, FrameData info, object labelObject) {
            var label = labelObject as TMP_Text;
            if (label == null)
                return;

            var allClipsOnTrackCount = playable.GetInputCount();

            for (var i = 0; i < allClipsOnTrackCount; i++) {
                var weight = playable.GetInputWeight(i);
                if (weight > 0.5f) {
                    var inputPlayable = (ScriptPlayable<SubtitlesLineBehaviour>) playable.GetInput(i);
                    var input = inputPlayable.GetBehaviour();
                    ShowText(label, input);
                    return;
                }
                ClearText(label);
            }
        }

        void ClearText(TMP_Text label) {
            if (label != null)
                label.text = "";
        }

        static void ShowText(TMP_Text label, SubtitlesLineBehaviour line) {
            if (label != null)
                label.text = line.text;
        }
    }
}
