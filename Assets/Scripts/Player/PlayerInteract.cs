using RatGamesStudios.OperationDeratization.Interactables;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerInteract : MonoBehaviour
    {
        [Header("References")]
        private Camera cam;
        private PlayerUI playerUI;

        [Header("Raycast")]
        public float distance = 2f;
        public Ray ray;
        public RaycastHit hitInfo;

        [Header("Cooldown")]
        public float interactCooldown = 0.5f;
        private float lastInteractTime = 0f;

        private void Start()
        {
            cam = GetComponent<PlayerShoot>().cam;
            playerUI = GetComponent<PlayerUI>();
        }
        private void Update()
        {
            playerUI.UpdateText(string.Empty);
            ray = new Ray(cam.transform.position, cam.transform.forward);
            LayerMask obstacleMask = ~(1 << LayerMask.NameToLayer("Player"));

            if (Physics.Raycast(ray, out hitInfo, distance, obstacleMask))
            {
                if (hitInfo.collider.GetComponent<Interactable>() != null)
                {
                    Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                    playerUI.UpdateText(interactable.prompt);
                    playerUI.promptText.color = Color.white;

                    if (Input.GetKeyDown(KeyCode.F) && Time.time - lastInteractTime >= interactCooldown)
                    {
                        interactable.BaseInteract();
                        lastInteractTime = Time.time;
                    }
                }
                if (hitInfo.collider.GetComponent<AmmoBox>() != null)
                    playerUI.ammoRefill = hitInfo.collider.gameObject.GetComponent<AmmoBox>();
            }
        }
    }
}