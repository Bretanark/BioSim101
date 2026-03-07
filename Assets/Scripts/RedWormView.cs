using System;
using System.Collections.Generic;
using UnityEngine;

public class RedWormView : MonoBehaviour
{
    [SerializeField] private Transform headSegment;
    [SerializeField] private Transform bodySegmentPrefab;

    private RedWorm worm;
    private SimulationController simulation;
    private readonly List<Transform> segments = new();

    public void Bind(RedWorm redWorm, SimulationController simulationController)
    {
        worm = redWorm ?? throw new ArgumentNullException(nameof(redWorm));
        simulation = simulationController ?? throw new ArgumentNullException(nameof(simulationController));

        segments.Clear();
        segments.Add(headSegment);
    }

    void Awake()
    {
        if (headSegment == null) throw new InvalidOperationException($"{nameof(headSegment)} not assigned.");
        if (bodySegmentPrefab == null) throw new InvalidOperationException($"{nameof(bodySegmentPrefab)} not assigned.");
    }

    void Update()
    {
        transform.position = simulation.PixelToWorld(worm.Position);

        EnsureSegmentCount(worm.Trail.Count);

        for (var i = 1; i < worm.Trail.Count; i++)
        {
            segments[i].localPosition = simulation.PixelToWorld(worm.Trail[^(i + 1)]) - transform.position;
        }
    }

    private void EnsureSegmentCount(int count)
    {
        while (segments.Count < count)
        {
            var seg = Instantiate(bodySegmentPrefab, transform);
            seg.gameObject.SetActive(true);
            segments.Add(seg);
        }
    }

}
