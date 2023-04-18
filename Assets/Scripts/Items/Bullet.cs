using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifespan = 3f;
    private bool hasCollided = false;

    private void Start()
    {
        Destroy(gameObject, lifespan);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!hasCollided)
        {
            hasCollided = true;
            Destroy(gameObject);
        }
    }
}
