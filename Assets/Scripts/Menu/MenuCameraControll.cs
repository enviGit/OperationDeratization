using UnityEngine;
using DG.Tweening;

public class MenuCameraControll : MonoBehaviour
{
    [SerializeField] private float duration;

    public void LookAt(Transform target)
    {
        transform.DOLookAt(target.position, duration);
    }
}
