using RatGamesStudios.OperationDeratization.Optimization.ObjectPooling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI.InGame
{
    public class DISystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject indicatorPrefab;
        [SerializeField] private RectTransform holder;
        [SerializeField] private new Camera camera;
        [SerializeField] private Transform player;
        private Dictionary<Transform, DamageIndicator> Indicators = new Dictionary<Transform, DamageIndicator>();
        public static Action<Transform> CreateIndicator = delegate { };

        private void OnEnable()
        {
            CreateIndicator += Create;
        }
        private void OnDisable()
        {
            CreateIndicator -= Create;
        }
        private void Create(Transform target)
        {
            if (Indicators.ContainsKey(target))
            {
                Indicators[target].Restart();

                return;
            }

            GameObject indicator = ObjectPoolManager.SpawnObject(indicatorPrefab, Vector3.zero, Quaternion.identity, holder);
            DamageIndicator newIndicator = indicator.GetComponent<DamageIndicator>();
            indicator.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            indicator.transform.localScale = Vector3.one;
            newIndicator.Register(target, player, new Action(() => { Indicators.Remove(target); }));
            Indicators.Add(target, newIndicator);
        }
    }
}