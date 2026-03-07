using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class WormViewBase<TWorm> : MonoBehaviour where TWorm : WormBase
{
    [SerializeField] private Transform headSegment;
    [SerializeField] private Transform bodySegmentPrefab;

    private float Fade { get; set; } = -1.1f;
    private TWorm Worm;
    private SimulationController simulation;
    private readonly List<Transform> segments = new();

    public abstract Color Color { get; }

    public void Bind(TWorm worm, SimulationController simulationController)
    {
        Worm = worm ?? throw new ArgumentNullException(nameof(TWorm));
        simulation = simulationController ?? throw new ArgumentNullException(nameof(simulationController));

        segments.Clear();
        segments.Add(headSegment);
    }

    public void Awake()
    {
        if (headSegment == null) throw new InvalidOperationException($"{nameof(headSegment)} not assigned.");
        if (bodySegmentPrefab == null) throw new InvalidOperationException($"{nameof(bodySegmentPrefab)} not assigned.");
    }

    public void Update()
    {
        transform.position = simulation.PixelToWorld(Worm.Position);

        EnsureSegmentCount(Worm.Trail.Count);

        for (var i = 1; i < Worm.Trail.Count; i++)
        {
            segments[i].localPosition = simulation.PixelToWorld(Worm.Trail[^(i + 1)]) - transform.position;
        }

        var headBrightness = Mathf.Clamp01(Worm.Energy / 100f);

        for (var i = 0; i < segments.Count; i++)
        {
            var renderer = segments[i].GetComponent<SpriteRenderer>();

            var t = i / (float)segments.Count;
            var fade = Mathf.Exp(Fade * t);   // exponential falloff

            var brightness = headBrightness * fade;

            renderer.color = new Color(brightness * Color.r, brightness * Color.g, brightness * Color.b);
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
