using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float distance = 3f;
    private PlayerUI playerUI;
    public Ray ray;
    public RaycastHit hitInfo;

    private void Start()
    {
        cam = GetComponent<PlayerMotor>().cam;
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

                if (Input.GetKeyDown(KeyCode.E))
                    interactable.BaseInteract();
            }
        }
    }
}
