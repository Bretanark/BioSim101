using UnityEngine;

public class BlueWorm : WormBase
{
    public static int InstanceCount { get; private set; }
    private static int _nextId = 1;


    private int Id { get; } = _nextId++;
    public override Color Eats => Color.blue;

    public BlueWorm(Vector2Int startPosition, float startEnergy)
        : base(startPosition, startEnergy)
    {
        InstanceCount++;
    }

    public override string Name => $"BlueWorm {Id} at {Position} with energy {Energy}";

    public override WormBase Reproduce(Vector2Int position, float energy) => new BlueWorm(position, energy);

    public override void CreateView(SimulationController simulation)
    {
        var view = Object.Instantiate(simulation.BlueWormViewPrefab, simulation.WormViewsParent);
        view.Bind(this, simulation);
    }
}
