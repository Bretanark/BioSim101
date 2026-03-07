using UnityEngine;

public class RedWorm : WormBase
{
    public static int InstanceCount { get; private set; }
    private static int _nextId = 1;


    private int Id { get; } = _nextId++;
    public override Color Eats => Color.red;

    public RedWorm(Vector2Int startPosition, float startEnergy)
        : base(startPosition, startEnergy)
    {
        InstanceCount++;
    }

    public override string Name => $"RedWorm {Id} at {Position} with energy {Energy}";

    public override WormBase Reproduce(Vector2Int position, float energy) => new RedWorm(position, energy);

    public override void CreateView(SimulationController simulation)
    {
        var view = Object.Instantiate(simulation.RedWormViewPrefab, simulation.WormViewsParent);
        view.Bind(this, simulation);
    }

}
