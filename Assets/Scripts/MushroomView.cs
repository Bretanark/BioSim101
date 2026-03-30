using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MushroomView : CreatureView<Mushroom>
{
    [SerializeField] private Transform _head;

    private SpriteRenderer _headRenderer;

    public void Awake()
    {
        if (_head == null) throw new InvalidOperationException($"{nameof(_head)} not assigned.");

        _headRenderer = _head.GetComponent<SpriteRenderer>();
    }

    protected override void OnBind()
    {
        var c = Creature.Color;
        c.a = 0.9f;
        _headRenderer.color = c;
    }

    protected override void UpdateAlive()
    {
        transform.position = Controller.PixelToWorld(Creature.Position);

        var diameter = Controller.PixelToWorld(Creature.Radius * 2f);
        _head.localScale = new Vector3(diameter, diameter, 1f);
    }

    protected override void UpdateDying(float alpha)
    {
        var c = _headRenderer.color;
        c.a = alpha * 0.9f;
        _headRenderer.color = c;
    }

}
