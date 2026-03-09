using UnityEngine;

public class Mushroom : Creature
{
    public int Radius { get; } = 5;

    public override float AvoidanceRadius => Radius;

    public override string Name => $"Mushroom at {Position}";

    public Mushroom(Vector2Int position, float energy, Color color)
        : base(position, energy, color)
    {
    }

    public override Creature Reproduce(Vector2Int position, float energy)
    {
        return new Mushroom(position, energy, Color);
    }

    public override void CreateView(SimulationController controller)
    {
        var view = Object.Instantiate(controller.MushroomViewPrefab, controller.CreatureViewsParent);
        view.Bind(this, controller);
    }

    public override void Update(SimulationController simulation)
    {
        // mushrooms do nothing for now
        base.Update(simulation);
    }

}
