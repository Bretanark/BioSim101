using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Renderer))]
public class SimulationController : MonoBehaviour
{
    [SerializeField] private Texture2D _sourceImage;
    [SerializeField] private WormView _wormViewPrefab;
    [SerializeField] private Transform _wormViewsParent;

    private Texture2D _worldTexture;
    private Color[] _pixels;
    public int Width { get; private set; }
    public int Height { get; private set; }

    private readonly List<Creature> _worms = new();
    private readonly List<Creature> _pendingWorms = new();

    public WormView WormViewPrefab => _wormViewPrefab;
    public Transform WormViewsParent => _wormViewsParent;


    public void Start()
    {
        Width = _sourceImage.width;
        Height = _sourceImage.height;

        _pixels = _sourceImage.GetPixels();

        _worldTexture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
        _worldTexture.SetPixels(_pixels);
        _worldTexture.Apply();

        GetComponent<Renderer>().material.mainTexture = _worldTexture;

        // ensure quad fills the camera view
        var cam = Camera.main;
        if (cam != null && cam.orthographic)
        {
            var height = cam.orthographicSize * 2f;
            var width = height * cam.aspect;
            transform.localScale = new Vector3(width, height, 1f);
        }

        // spawn a few of each worm type
        for (var i = 0; i < 3; i++)
        {
            foreach (var color in new[] { Color.red, Color.green, Color.blue })
            {
                var x = Random.Range(0, Width);
                var y = Random.Range(0, Height);
                var red = new Worm(new Vector2Int(x, y), 100f, color);
                _worms.Add(red);
                var view = Instantiate(WormViewPrefab, WormViewsParent);
                view.Bind(red, this, color);
            }
        }
    }

    public void Update()
    {
        _worms.AddRange(_pendingWorms);
        _pendingWorms.Clear();

        foreach (var worm in _worms)
        {
            worm.Update(this);
        }

        _worldTexture.SetPixels(_pixels);
        _worldTexture.Apply();
    }

    public Vector3 PixelToWorld(Vector2Int pixelPosition)
    {
        var x = ((pixelPosition.x + 0.5f) / Width - 0.5f) * transform.localScale.x;
        var y = ((pixelPosition.y + 0.5f) / Height - 0.5f) * transform.localScale.y;

        return transform.position + new Vector3(x, y, 0);
    }

    public Color GetPixel(Vector2Int position)
    {
        return _pixels[(position.y * Width) + position.x];
    }

    public float Eat(Vector2Int head, Vector2Int tail, int radius, Color amount, float biteStrength)
    {
        // Eat at the head
        var totalEaten = 0f;
        ForEachPixelInCircle(head, radius, (index, dx, dy) =>
        {
            var pixel = _pixels[index];

            var distance = Mathf.Sqrt(dx * dx + dy * dy);
            var falloff = Mathf.Lerp(0.4f, 1f, 1f - (distance / radius));

            var bite = biteStrength * falloff;

            var eatR = Mathf.Min(pixel.r, amount.r * bite);
            var eatG = Mathf.Min(pixel.g, amount.g * bite);
            var eatB = Mathf.Min(pixel.b, amount.b * bite);

            pixel.r -= eatR;
            pixel.g -= eatG;
            pixel.b -= eatB;

            _pixels[index] = pixel;

            totalEaten += eatR + eatG + eatB;
        });

        // Poop at the tail
        const float poopScale = 0.5f;
        var poop = new Color(1f - amount.r, 1f - amount.g, 1f - amount.b)
            * totalEaten * poopScale / (Mathf.PI * radius * radius);
        ForEachPixelInCircle(tail, radius, (index, _, _) =>
        {
            var pixel = _pixels[index];

            pixel.r = Mathf.Min(1f, pixel.r + poop.r);
            pixel.g = Mathf.Min(1f, pixel.g + poop.g);
            pixel.b = Mathf.Min(1f, pixel.b + poop.b);

            _pixels[index] = pixel;
        });

        return totalEaten;
    }

    private void ForEachPixelInCircle(Vector2Int centre, int radius, ForEachPixelInCircleAction action)
    {
        var radiusSquared = radius * radius;

        for (var dy = -radius; dy <= radius; dy++)
        {
            for (var dx = -radius; dx <= radius; dx++)
            {
                if ((dx * dx) + (dy * dy) > radiusSquared) continue;

                var x = centre.x + dx;
                var y = centre.y + dy;

                if (x < 0 || x >= Width || y < 0 || y >= Height) continue;

                action((y * Width) + x, dx, dy);
            }
        }
    }

    private delegate void ForEachPixelInCircleAction(int index, int dx, int dy);

    public void Reproduce(Creature parent)
    {
        var offset = new Vector2Int(Random.Range(-2, 3), Random.Range(-2, 3));
        var position = parent.Position + offset;

        position.x = Mathf.Clamp(position.x, 0, Width - 1);
        position.y = Mathf.Clamp(position.y, 0, Height - 1);

        var child = parent.Reproduce(position, parent.Energy);
        _pendingWorms.Add(child);
        child.CreateView(this);
    }

}
