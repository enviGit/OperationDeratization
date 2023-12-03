using UnityEngine;

public class PlayerLeaning : MonoBehaviour
{
    [Header("Leaning")]
    private float currentLean;
    private float targetLean;
    public float leanAngle = 30f;
    public float leanSmoothing = 0.15f;
    private float leanVelocity;
    private bool isLeaningLeft = false;
    private bool isLeaningRight = false;

    private void Update()
    {
        CheckLeaningInput();
        CalculateLeaning();
    }
    private void CheckLeaningInput()
    {
        isLeaningLeft = Input.GetKey(KeyCode.Q);
        isLeaningRight = Input.GetKey(KeyCode.E);
    }
    private void CalculateLeaning()
    {
        if (isLeaningLeft)
            targetLean = Mathf.Clamp(targetLean + leanAngle * Time.deltaTime * 2f, -leanAngle, leanAngle);
        else if (isLeaningRight)
            targetLean = Mathf.Clamp(targetLean - leanAngle * Time.deltaTime * 2f, -leanAngle, leanAngle);
        else
            targetLean = 0f;

        currentLean = Mathf.SmoothDamp(currentLean, targetLean, ref leanVelocity, leanSmoothing);
        transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, currentLean));
    }
}
