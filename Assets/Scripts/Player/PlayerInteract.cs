using TMPro;
using UnityEngine;
using System.Collections;

public class PlayerInteract : MonoBehaviour
{
    [Header("References")]
    private Camera cam;
    private PlayerUI playerUI;

    [Header("Raycast")]
    public float distance = 2f;
    public Ray ray;
    public RaycastHit hitInfo;

    private void Start()
    {
        cam = GetComponent<PlayerShoot>().cam;
        playerUI = GetComponent<PlayerUI>();
    }
    private void Update()
    {
        playerUI.UpdateText(string.Empty);
        ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.prompt);
                playerUI.promptText.color = Color.white;

                if (Input.GetKeyDown(KeyCode.F))
                    interactable.BaseInteract();
            }
            else if (hitInfo.collider.GetComponent<EnemyHealth>() != null)
            {
                EnemyHealth enemy = hitInfo.collider.GetComponent<EnemyHealth>();
                Tracker tracker = FindObjectOfType<Tracker>();

                if(!enemy.isAlive && !enemy.isMarkedAsDead)
                {
                    playerUI.UpdateText("Mark opponent as dead");
                    playerUI.promptText.color = Color.red;

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        tracker.MarkOpponentAsDead(hitInfo.collider.gameObject);
                        playerUI.UpdateText(string.Empty);

                        if (!enemy.isMarkedAsDead)
                            StartCoroutine(FadeOutPromptText());

                        enemy.isMarkedAsDead = true;
                    }
                }
            }
        }
    }
    private IEnumerator FadeOutPromptText()
    {
        float fadeDuration = 2f;
        float elapsedTime = 0f;
        Color initialColor = Color.green;
        GameObject promptTextClone = Instantiate(playerUI.promptText.gameObject, playerUI.promptText.transform.parent);
        TextMeshProUGUI promptText = promptTextClone.GetComponent<TextMeshProUGUI>();
        promptText.text = "Marked";

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            promptText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(promptTextClone);
    }
}
