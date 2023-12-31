using DG.Tweening;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI.Menu
{
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
}