using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WormView : CreatureView<Worm>
{
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _body;

    private float _fade = -1.1f;
    private SpriteRenderer _headRenderer;
    private SpriteRenderer _bodyRenderer;
    private readonly List<Transform> _segments = new();



    public void Awake()
    {
        if (_head == null) throw new InvalidOperationException($"{nameof(_head)} not assigned.");
        if (_body == null) throw new InvalidOperationException($"{nameof(_body)} not assigned.");

        _headRenderer = _head.GetComponent<SpriteRenderer>();
        _bodyRenderer = _body.GetComponent<SpriteRenderer>();
    }

    protected override void OnBind()
    {
        _segments.Clear();
        _segments.Add(_head);

        _headRenderer.color = Creature.Color;
        _bodyRenderer.color = Creature.Color * 0.5f;
    }

    protected override void UpdateAlive()
    {
        transform.position = Controller.PixelToWorld(Creature.Position);

        EnsureSegmentCount(Creature.Trail.Count);

        for (var i = 1; i < Creature.Trail.Count; i++)
        {
            _segments[i].localPosition = Controller.PixelToWorld(Creature.Trail[^(i + 1)]) - transform.position;
        }

        var headBrightness = Mathf.Clamp01(Creature.Energy / 100f);

        for (var i = 0; i < _segments.Count; i++)
        {
            var renderer = _segments[i].GetComponent<SpriteRenderer>();

            var t = i / (float)_segments.Count;
            var fade = Mathf.Exp(_fade * t);   // exponential falloff

            var brightness = headBrightness * fade;

            renderer.color = new Color(brightness * Creature.Color.r, brightness * Creature.Color.g, brightness * Creature.Color.b);
        }
    }

    protected override void UpdateDying(float alpha)
    {
        foreach (var segment in _segments)
        {
            var renderer = segment.GetComponent<SpriteRenderer>();
            var c = renderer.color;
            c.a = alpha;
            renderer.color = c;
        }
    }

    private void EnsureSegmentCount(int count)
    {
        while (_segments.Count < count)
        {
            var seg = Instantiate(_body, transform);
            seg.gameObject.SetActive(true);
            _segments.Add(seg);
        }
    }
}
