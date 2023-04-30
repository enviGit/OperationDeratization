using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Flashbang : MonoBehaviour
{
    [Header("Flashbang")]
    public float delay = 1.5f;
    public float duration = 2f;
    public float intensity = 1.5f;
    public bool shouldFlash = false;
    bool hasFlashed = false;
    float countdown;

    private void Start()
    {
        countdown = delay;
    }
    private void Update()
    {
        if (shouldFlash)
        {
            countdown -= Time.deltaTime;

            if (countdown <= 0 && !hasFlashed)
            {
                Flash();
                hasFlashed = true;
            }
        }
    }
    private void Flash()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 grenadeDirection = transform.position - playerTransform.position;
        float angle = Vector3.Angle(grenadeDirection, playerTransform.forward);

        if (angle <= 90f) 
            StartCoroutine(FlashCoroutine());
    }
    IEnumerator FlashCoroutine()
    {
        Color originalColor = Color.white;
        Color flashColor = new Color(1f, 1f, 1f, 0f);
        float blinkTime = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime <= duration)
        {
            Camera.main.backgroundColor = originalColor;
            yield return new WaitForSeconds(blinkTime);
            Camera.main.backgroundColor = flashColor;
            yield return new WaitForSeconds(blinkTime);
            elapsedTime += blinkTime * 2f;
        }

        Camera.main.backgroundColor = originalColor;
        /*PlayerMotor playerMotor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        playerMotor.DisableControls(duration);
        Collider[] collidersToAffect = Physics.OverlapSphere(transform.position, 10f);

        foreach (Collider nearbyObject in collidersToAffect)
        {
            Enemy enemy = nearbyObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                float effect = Mathf.Clamp01((10f - distance) / 10f);
                enemy.ApplyStun(duration * effect, intensity * effect);
            }
        }*/

        Destroy(gameObject);
    }
}
