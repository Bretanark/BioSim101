using UnityEngine;

public abstract class WormBase
{
    public Vector2Int Position { get; protected set; }
    public float Energy { get; protected set; }
    public bool IsDead { get; protected set; }

    protected WormBase(Vector2Int startPosition, float startEnergy)
    {
        Position = startPosition;
        Energy = startEnergy;
    }

    public abstract void Update(SimulationController simulation);
}
