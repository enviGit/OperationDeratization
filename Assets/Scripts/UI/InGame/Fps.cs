using System.Collections;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI.InGame
{
    public class Fps : MonoBehaviour
    {
        private float count;
        [SerializeField] private bool isTutorialActive = false;

        private IEnumerator Start()
        {
            GUI.depth = 2;

            while (true)
            {
                count = 1f / Time.unscaledDeltaTime;

                yield return new WaitForSeconds(1f);
            }
        }
        private void OnGUI()
        {
            GUIStyle headStyle = new GUIStyle
            {
                fontSize = 30
            };
            headStyle.normal.textColor = Color.white;

            if (!isTutorialActive)
                GUI.Label(new Rect(5, 5, 100, 25), "FPS: " + Mathf.Round(count), headStyle);
            else
            {
                headStyle.alignment = TextAnchor.MiddleRight;
                GUI.Label(new Rect(Screen.width - 105, 5, 100, 25), "FPS: " + Mathf.Round(count), headStyle);
            }
        }
    }
}