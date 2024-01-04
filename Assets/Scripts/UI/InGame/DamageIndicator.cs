using RatGamesStudios.OperationDeratization.Optimization.ObjectPooling;
using System;
using System.Collections;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI.InGame
{
    public class DamageIndicator : MonoBehaviour
    {
        private const float maxTimer = 4f;
        private float timer = maxTimer;
        private CanvasGroup canvasGroup;
        protected CanvasGroup CanvasGroup
        {
            get
            {
                if (canvasGroup == null)
                {
                    canvasGroup = GetComponent<CanvasGroup>();

                    if (canvasGroup == null)
                        canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }

                return canvasGroup;
            }
        }
        private RectTransform rect;
        protected RectTransform Rect
        {
            get
            {
                if (rect == null)
                {
                    rect = GetComponent<RectTransform>();

                    if (rect == null)
                        rect = gameObject.AddComponent<RectTransform>();
                }

                return rect;
            }
        }
        public Transform Target { get; protected set; }
        private Transform player;
        private IEnumerator countdown;
        private Action unRegister;
        private Quaternion tRot = Quaternion.identity;
        private Vector3 tPos = Vector3.zero;

        public void Register(Transform target, Transform player, Action unRegister)
        {
            this.Target = target;
            this.player = player;
            this.unRegister = unRegister;
            StartCoroutine(RotateToTheTarget());
            StartTimer();
        }
        public void Restart()
        {
            timer = maxTimer;
            StartTimer();
        }
        private void StartTimer()
        {
            if (countdown != null)
                StopCoroutine(countdown);

            countdown = Countdown();
            StartCoroutine(countdown);
        }
        IEnumerator RotateToTheTarget()
        {
            while (enabled)
            {
                if (Target)
                {
                    tPos = Target.position;
                    tRot = Target.rotation;
                }

                Vector3 direction = player.position - tPos;
                tRot = Quaternion.LookRotation(direction);
                tRot.z = -tRot.y;
                tRot.x = 0;
                tRot.y = 0;
                Vector3 northDirection = new Vector3(0, 0, player.eulerAngles.y + 180f);
                Rect.localRotation = tRot * Quaternion.Euler(northDirection);

                yield return null;
            }
        }
        private IEnumerator Countdown()
        {
            while (CanvasGroup.alpha < 1f)
            {
                CanvasGroup.alpha += 4 * Time.deltaTime;

                yield return null;
            }
            while (timer > 0)
            {
                timer--;

                yield return new WaitForSeconds(1f);
            }
            while (CanvasGroup.alpha > 0f)
            {
                CanvasGroup.alpha -= 2 * Time.deltaTime;

                yield return null;
            }

            unRegister();
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }
}