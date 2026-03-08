using UnityEngine;

public class Worm : Creature
{
    public static int InstanceCount { get; private set; }
    private static int _nextId = 1;


    private int Id { get; } = _nextId++;
    public override Color Color { get; }

    public Worm(Vector2Int startPosition, float startEnergy, Color color)
        : base(startPosition, startEnergy)
    {
        InstanceCount++;
        Color = color;
    }

    public override string Name => $"{Color} worm {Id} at {Position} with energy {Energy}";

    public override Creature Reproduce(Vector2Int position, float energy) => new Worm(position, energy, Color);

    public override void CreateView(SimulationController controller)
    {
        var view = Object.Instantiate(controller.WormViewPrefab, controller.WormViewsParent);
        view.Bind(this, controller, Color);
    }

}
