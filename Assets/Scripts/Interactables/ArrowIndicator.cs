using DG.Tweening;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization
{
    public class ArrowIndicator : MonoBehaviour
    {
        private Transform player;
        private Quaternion initialRotation;
        private bool isDestroyed = false;
        public float moveDistance = 1f;
        public float moveAnimationDuration = 1f;
        public float rotateAnimationDuration = 0.5f;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            initialRotation = transform.rotation;
            Invoke("PlayArrowAnimation", 0.01f);
        }
        private void PlayArrowAnimation()
        {
            if (this != null && gameObject != null)
            {
                // Kill any existing tweens on this object
                DOTween.Kill(transform);
                // Move the arrow up and down alternately
                Sequence moveSequence = DOTween.Sequence();
                moveSequence.AppendCallback(() => initialRotation = transform.rotation);
                moveSequence.Append(transform.DOMoveY(transform.position.y + moveDistance, moveAnimationDuration)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() =>
                    {
                        // Rotate the arrow towards the player's position while moving
                        transform.LookAt(player.position);
                        // Apply the initial rotation to maintain the relative rotation
                        transform.rotation = Quaternion.Euler(90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                    }));

                // Set the infinite loops directly on the sequence
                moveSequence.SetLoops(-1, LoopType.Yoyo);
                moveSequence.Play();
            }
        }
        // Called when the parent object is destroyed
        private void OnDestroy()
        {
            // Kill any existing tweens on this object
            DOTween.Kill(transform);
        }
    }
}