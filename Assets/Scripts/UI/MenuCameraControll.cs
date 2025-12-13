using DG.Tweening;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI
{
	public class MenuCameraControll : MonoBehaviour
	{
		[SerializeField] private float duration;

		public void LookAt(Transform target)
		{
			transform.DOLookAt(target.position, duration);
		}
	}
}