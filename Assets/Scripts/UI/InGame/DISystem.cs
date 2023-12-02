using System;
using System.Collections.Generic;
using UnityEngine;

public class DISystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DamageIndicator indicatorPrefab;
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

        DamageIndicator newIndicator = Instantiate(indicatorPrefab, holder);
        newIndicator.Register(target, player, new Action(() => { Indicators.Remove(target); }));
        Indicators.Add(target, newIndicator);
    }
}
