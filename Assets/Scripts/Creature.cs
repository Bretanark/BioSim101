using System.Collections.Generic;
using UnityEngine;

public abstract class Creature
{
    public readonly static Dictionary<string, int> PopulationByKey = new();


    public Color Color { get; }
    public abstract string Name { get; }
    public abstract Creature Reproduce(Vector2Int position, float energy);
    public abstract void CreateView(SimulationController simulation);

    public Vector2Int Position { get; protected set; }
    public float Energy { get; protected set; }
    public bool IsDead { get; protected set; }

    private string GetPopulationKey() => $"{GetType().Name}_{Color}";

    public int Population => PopulationByKey[GetPopulationKey()];

    public virtual float AvoidanceRadius => 50f;


    protected Creature(Vector2Int startPosition, float startEnergy, Color color)
    {
        Position = startPosition;
        Energy = startEnergy;
        Color = color;

        var instanceCountKey = GetPopulationKey();
        PopulationByKey[instanceCountKey] = PopulationByKey.TryGetValue(instanceCountKey, out var instanceCount) ? instanceCount + 1 : 1;
    }

    public virtual void Update(SimulationController simulation)
    {
        if (Energy >= 200f)
        {
            Energy *= 0.5f;
            simulation.Reproduce(this);
        }
        else if (Energy <= 0f && !IsDead)
        {
            IsDead = true;
            PopulationByKey[GetPopulationKey()]--;
        }
    }

}
