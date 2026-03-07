using UnityEngine;

public class GreenWorm : WormBase
{
    public static int InstanceCount { get; private set; }
    private static int _nextId = 1;


    private int Id { get; } = _nextId++;
    public override Color Eats => Color.green;

    public GreenWorm(Vector2Int startPosition, float startEnergy)
        : base(startPosition, startEnergy)
    {
        InstanceCount++;
    }

    public override string Name => $"GreenWorm {Id} at {Position} with energy {Energy}";

}
