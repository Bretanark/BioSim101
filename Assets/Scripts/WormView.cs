using System;
using System.Collections.Generic;
using UnityEngine;

public class WormView : MonoBehaviour
{
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _body;

    private float _fade = -1.1f;
    private Worm _worm;
    private SimulationController _controller;
    private SpriteRenderer _headRenderer;
    private SpriteRenderer _bodyRenderer;
    private readonly List<Transform> _segments = new();

    public Color Color { get; set; } = Color.white;


    public void Awake()
    {
        if (_head == null) throw new InvalidOperationException($"{nameof(_head)} not assigned.");
        if (_body == null) throw new InvalidOperationException($"{nameof(_body)} not assigned.");

        _headRenderer = _head.GetComponent<SpriteRenderer>();
        _bodyRenderer = _body.GetComponent<SpriteRenderer>();
    }

    public void Bind(Worm worm, SimulationController controller, Color color)
    {
        _worm = worm;
        _controller = controller;

        _segments.Clear();
        _segments.Add(_head);

        Color = color;
        _headRenderer.color = worm.Color;
        _bodyRenderer.color = worm.Color * 0.5f;
    }

    public void Update()
    {
        transform.position = _controller.PixelToWorld(_worm.Position);

        EnsureSegmentCount(_worm.Trail.Count);

        for (var i = 1; i < _worm.Trail.Count; i++)
        {
            _segments[i].localPosition = _controller.PixelToWorld(_worm.Trail[^(i + 1)]) - transform.position;
        }

        var headBrightness = Mathf.Clamp01(_worm.Energy / 100f);

        for (var i = 0; i < _segments.Count; i++)
        {
            var renderer = _segments[i].GetComponent<SpriteRenderer>();

            var t = i / (float)_segments.Count;
            var fade = Mathf.Exp(_fade * t);   // exponential falloff

            var brightness = headBrightness * fade;

            renderer.color = new Color(brightness * Color.r, brightness * Color.g, brightness * Color.b);
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
