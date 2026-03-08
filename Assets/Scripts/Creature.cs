using UnityEngine;

public abstract class Creature
{
    public Color Color { get; }
    public abstract string Name { get; }
    public abstract Creature Reproduce(Vector2Int position, float energy);
    public abstract void CreateView(SimulationController simulation);

    public Vector2Int Position { get; protected set; }
    public float Energy { get; protected set; }
    public bool IsDead { get; protected set; }



    protected Creature(Vector2Int startPosition, float startEnergy, Color color)
    {
        Position = startPosition;
        Energy = startEnergy;
        Color = color;
    }

    public abstract void Update(SimulationController simulation);

}
