using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SimulationController : MonoBehaviour
{
    [SerializeField] private Texture2D sourceImage;
    [SerializeField] private RedWormView redWormViewPrefab;
    [SerializeField] private BlueWormView blueWormViewPrefab;
    [SerializeField] private GreenWormView greenWormViewPrefab;
    [SerializeField] private Transform wormViewsParent;

    private Texture2D worldTexture;
    private Color[] pixels;
    public int Width { get; private set; }
    public int Height { get; private set; }

    private readonly List<WormBase> worms = new();


    public void Start()
    {
        Width = sourceImage.width;
        Height = sourceImage.height;

        pixels = sourceImage.GetPixels();

        worldTexture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
        worldTexture.SetPixels(pixels);
        worldTexture.Apply();

        GetComponent<Renderer>().material.mainTexture = worldTexture;


        // spawn a few of each worm type
        for (var i = 0; i < 3; i++)
        {
            var x = Random.Range(0, Width);
            var y = Random.Range(0, Height);
            var red = new RedWorm(new Vector2Int(x, y), 100f);
            worms.Add(red);
            var redView = Instantiate(redWormViewPrefab, wormViewsParent);
            redView.Bind(red, this);

            x = Random.Range(0, Width);
            y = Random.Range(0, Height);
            var blue = new BlueWorm(new Vector2Int(x, y), 100f);
            worms.Add(blue);
            var blueView = Instantiate(blueWormViewPrefab, wormViewsParent);
            blueView.Bind(blue, this);

            x = Random.Range(0, Width);
            y = Random.Range(0, Height);
            var green = new GreenWorm(new Vector2Int(x, y), 100f);
            worms.Add(green);
            var greenView = Instantiate(greenWormViewPrefab, wormViewsParent);
            greenView.Bind(green, this);
        }
    }

    public void Update()
    {
        foreach (var worm in worms)
            worm.Update(this);

        worldTexture.SetPixels(pixels);
        worldTexture.Apply();
    }

    public Vector3 PixelToWorld(Vector2Int pixelPosition)
    {
        var x = ((pixelPosition.x + 0.5f) / Width - 0.5f) * transform.localScale.x;
        var y = ((pixelPosition.y + 0.5f) / Height - 0.5f) * transform.localScale.y;

        return transform.position + new Vector3(x, y, 0);
    }

    public Color GetPixel(Vector2Int position)
    {
        return pixels[(position.y * Width) + position.x];
    }

    public float Eat(Vector2Int centre, int radius, Color amount)
    {
        const float poopScale = 0.3f;

        var radiusSquared = radius * radius;
        var totalEaten = 0f;

        for (var dy = -radius; dy <= radius; dy++)
        {
            for (var dx = -radius; dx <= radius; dx++)
            {
                if ((dx * dx) + (dy * dy) > radiusSquared) continue;

                var x = centre.x + dx;
                var y = centre.y + dy;

                if (x < 0 || x >= Width || y < 0 || y >= Height) continue;

                var index = (y * Width) + x;
                var pixel = pixels[index];

                var eatR = Mathf.Min(pixel.r, amount.r);
                var eatG = Mathf.Min(pixel.g, amount.g);
                var eatB = Mathf.Min(pixel.b, amount.b);

                pixel.r -= eatR;
                pixel.g -= eatG;
                pixel.b -= eatB;

                // convert eaten nutrients into opposite-color waste
                pixel.r = Mathf.Min(1f, pixel.r + (eatG + eatB) * poopScale);
                pixel.g = Mathf.Min(1f, pixel.g + (eatR + eatB) * poopScale);
                pixel.b = Mathf.Min(1f, pixel.b + (eatR + eatG) * poopScale);

                pixels[index] = pixel;

                totalEaten += eatR + eatG + eatB;
            }
        }

        return totalEaten;
    }

}
