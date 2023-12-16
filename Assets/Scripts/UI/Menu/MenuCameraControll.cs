using UnityEngine;
using DG.Tweening;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class MenuCameraControll : MonoBehaviour
{
    [SerializeField] private float duration;

	private async void Start()
	{
		await UnityServices.InitializeAsync();
		StartCollectingData();
	}
	private void StartCollectingData()
	{
		AnalyticsService.Instance.StartDataCollection();
	}
	public void LookAt(Transform target)
    {
        transform.DOLookAt(target.position, duration);
    }
}
