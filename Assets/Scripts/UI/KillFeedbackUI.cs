using System.Collections;
using TMPro;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI
{
    public class KillFeedbackUI : MonoBehaviour
    {
        [SerializeField] private GameObject markTextObject;
        [SerializeField] private float displayDuration = 3f;

        private Coroutine hideCoroutine;

        private void Start()
        {
            if (markTextObject) markTextObject.SetActive(false);
        }

        public void ShowNeutralizedMessage()
        {
            if (markTextObject == null) return;

            markTextObject.SetActive(true);

            if (hideCoroutine != null) StopCoroutine(hideCoroutine);
            hideCoroutine = StartCoroutine(HideMessageAfterDelay());
        }

        private IEnumerator HideMessageAfterDelay()
        {
            yield return new WaitForSeconds(displayDuration);
            markTextObject.SetActive(false);
        }
    }
}