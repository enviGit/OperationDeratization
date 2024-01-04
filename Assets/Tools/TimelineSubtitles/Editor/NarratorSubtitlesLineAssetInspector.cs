using Tools.TimelineSubtitles;
using UnityEditor;
using UnityEngine;

    [CustomEditor(typeof(SubtitlesLine))]
    public class NarratorSubtitlesLineAssetInspector : UnityEditor.Editor {
        const int MIN_CPS = 10;
        const int AVERAGE_CPS = 20;
        const int MAX_CPS = 25;
        
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            ShowDurationFeedback();
        }

        void ShowDurationFeedback() {
            var clip = (target as SubtitlesLine);
            var txt = clip.text;

            GUILayout.Label(
                $"Sądząc po znakach, to powino trwać {CalculateTimeUsingCPS(txt, AVERAGE_CPS):0.0}-{CalculateTimeUsingCPS(txt, MIN_CPS):0.0}s, co jakiś czas może być {CalculateTimeUsingCPS(txt, MAX_CPS):0.0}s");
        }

        static float CalculateTimeUsingCPS(string text, float cps) {
            if (string.IsNullOrEmpty(text))
                return 0;
            var cpsBasedTime = text.Length / cps;
            if (cpsBasedTime < 1.5f)
                cpsBasedTime = 1.5f;
            return cpsBasedTime;
        }
    }
