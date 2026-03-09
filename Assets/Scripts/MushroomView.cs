using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MushroomView : MonoBehaviour
{
    [SerializeField] private Transform _head;

    private Mushroom _mushroom;
    private SimulationController _controller;
    private SpriteRenderer _headRenderer;
    private float _deathFade;

    public void Awake()
    {
        if (_head == null) throw new InvalidOperationException($"{nameof(_head)} not assigned.");

        _headRenderer = _head.GetComponent<SpriteRenderer>();
    }

    public void Bind(Mushroom mushroom, SimulationController controller)
    {
        _mushroom = mushroom;
        _controller = controller;

        _headRenderer.color = mushroom.Color;
    }

    void Update()
    {
        if (_mushroom.IsDead)
        {
            _deathFade += Time.deltaTime / 5f;
            var c = _headRenderer.color;
            c.a = Mathf.Clamp01(1f - _deathFade);
            _headRenderer.color = c;

            if (_deathFade > 1f)
                Destroy(gameObject);

            return;
        }

        transform.position = _controller.PixelToWorld(_mushroom.Position);

        var diameterWorld = _controller.PixelToWorld(_mushroom.Radius * 2f);
        _head.localScale = new Vector3(diameterWorld, diameterWorld, 1f);

        var brightness = Mathf.Clamp01(_mushroom.Energy / 100f);
        _headRenderer.color = new Color(
            brightness * _mushroom.Color.r,
            brightness * _mushroom.Color.g,
            brightness * _mushroom.Color.b,
            1f);
    }

}
