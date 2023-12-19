using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Flashbang : MonoBehaviour
{
    [Header("Flashbang")]
    private Image whiteImage;
    private AudioSource whiteNoise;
    private AudioSource bang;
    public float delay = 2f;
    public float distance = 10f;
    public bool shouldFlash = false;
    bool hasFlashed = false;
    float countdown;

    private void Start()
    {
        countdown = delay;
        whiteImage = GameObject.FindGameObjectWithTag("WhiteImage").GetComponent<Image>();
        whiteNoise = GameObject.FindGameObjectWithTag("WhiteNoise").GetComponent<AudioSource>();
        bang = GameObject.FindGameObjectWithTag("Bang").GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (shouldFlash)
        {
            countdown -= Time.deltaTime;
            var weaponScript = transform.GetComponent<Weapon>();
            Destroy(weaponScript);

            if (countdown <= 0 && !hasFlashed)
            {
                Flash();
                hasFlashed = true;
            }
        }
    }
    public void Flash()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 grenadeDirection = transform.position - playerTransform.position;
        float angle = Vector3.Angle(grenadeDirection, playerTransform.forward);
        float distanceComparision = Vector3.Distance(transform.position, playerTransform.position);
        bang.PlayOneShot(bang.GetComponent<ProjectileSound>().audioClips[1]);
        whiteNoise.Play();

        if (angle < 60f && distanceComparision <= distance)
        {
            RaycastHit hit;

            if (Physics.Raycast(playerTransform.position, grenadeDirection, out hit, distanceComparision))
            {
                if (hit.transform.gameObject != gameObject)
                {
                    Destroy(gameObject);

                    return;
                }
            }

            StartCoroutine(FlashCoroutine());
        }
        else
            Destroy(gameObject);
    }
    private IEnumerator FlashCoroutine()
    {
        whiteImage.color = new Vector4(1, 1, 1, 1);

        /*Collider[] collidersToAffect = Physics.OverlapSphere(transform.position, distance);

        foreach (Collider nearbyObject in collidersToAffect)
        {
          Enemy enemy = nearbyObject.GetComponent<Enemy>();

          if (enemy != null)
          {
              float enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);
              float effect = Mathf.Clamp01((distance - enemyDistance) / distance);
              enemy.ApplyStun();
          }*/

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        float fadeSpeed = 1f;
        float modifier = 0.01f;
        float waitTime = 0;

        for (int i = 0; whiteImage.color.a > 0; i++)
        {
            whiteImage.color = new Vector4(1, 1, 1, fadeSpeed);
            fadeSpeed = fadeSpeed - 0.025f;
            modifier = modifier * 1.5f;
            waitTime = 0.5f - modifier;

            if (waitTime < 0.1f)
                waitTime = 0.1f;

            whiteNoise.volume -= 0.05f;

            yield return new WaitForSeconds(waitTime);
        }

        whiteNoise.Stop();
        whiteNoise.volume = 1;
        Destroy(gameObject);
    }
}