using UnityEngine;

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
        }
    }
}
