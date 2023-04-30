using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destroyedVersion;

    public void Destroy()
    {
        Instantiate(destroyedVersion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
