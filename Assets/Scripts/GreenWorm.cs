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

    public override WormBase Reproduce(Vector2Int position, float energy) => new GreenWorm(position, energy);

    public override void CreateView(SimulationController simulation)
    {
        var view = Object.Instantiate(simulation.GreenWormViewPrefab, simulation.WormViewsParent);
        view.Bind(this, simulation);
    }

}
