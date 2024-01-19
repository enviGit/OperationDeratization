using DG.Tweening;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization
{
    public class ArrowIndicator : MonoBehaviour
    {
        private Transform player;
        private Quaternion initialRotation;
        private Sequence moveSequence;
        public float moveDistance = 1f;
        public float moveAnimationDuration = 1f;
        public float rotateAnimationDuration = 0.5f;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            initialRotation = transform.rotation;
            PlayArrowAnimation();
        }
        private void PlayArrowAnimation()
        {
            // Stop and clear tweens if they exist
            DOTween.Kill(transform);
            // Move the arrow up and down alternately
            moveSequence = DOTween.Sequence();
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
        // Called when the parent object is destroyed
        private void OnDestroy()
        {
            // Check if tweens sequence is active and stop it
            if (moveSequence != null && moveSequence.IsActive())
                moveSequence.Kill();
        }
    }
}