using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SimulationController : MonoBehaviour
{
    [SerializeField] private Texture2D sourceImage;
    [SerializeField] private RedWormView redWormView;

    private Texture2D worldTexture;
    private Color[] pixels;
    public int Width { get; private set; }
    public int Height { get; private set; }

    private RedWorm redWorm;


    void Start()
    {
        Width = sourceImage.width;
        Height = sourceImage.height;

        pixels = sourceImage.GetPixels();

        worldTexture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
        worldTexture.SetPixels(pixels);
        worldTexture.Apply();

        GetComponent<Renderer>().material.mainTexture = worldTexture;

        redWorm = new RedWorm(new Vector2Int(Width / 2, Height / 2), 100f);
        redWormView.Bind(redWorm, this);
    }

    void Update()
    {
        redWorm.Update(this);

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
                Color pixel = pixels[index];

                var eatR = Mathf.Min(pixel.r, amount.r);
                var eatG = Mathf.Min(pixel.g, amount.g);
                var eatB = Mathf.Min(pixel.b, amount.b);

                pixel.r -= eatR;
                pixel.g -= eatG;
                pixel.b -= eatB;

                pixels[index] = pixel;

                totalEaten += eatR + eatG + eatB;
            }
        }

        return totalEaten;
    }

}
