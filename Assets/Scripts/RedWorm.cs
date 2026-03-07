using System.Collections.Generic;
using UnityEngine;

public class RedWorm : WormBase
{
    public static int InstanceCount { get; private set; }
    public readonly List<Vector2Int> Trail = new();

    public RedWorm(Vector2Int startPosition, float startEnergy)
        : base(startPosition, startEnergy)
    {
        InstanceCount++;
        Trail.Add(startPosition);
    }

    public override void Update(SimulationController simulation)
    {
        var dx = Random.Range(-1, 2);
        var dy = Random.Range(-1, 2);

        var x = Mathf.Clamp(Position.x + dx, 0, simulation.Width - 1);
        var y = Mathf.Clamp(Position.y + dy, 0, simulation.Height - 1);

        Position = new Vector2Int(x, y);

        Energy += simulation.Eat(Position, 2, new Color(0.05f, 0, 0));

        Trail.Add(Position);

        if (Trail.Count > 20)
            Trail.RemoveAt(0);
    }

}
