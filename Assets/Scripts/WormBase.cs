using System.Collections.Generic;
using UnityEngine;

public abstract class WormBase
{
    public abstract Color Eats { get; }
    public abstract string Name { get; }


    public Vector2Int Position { get; protected set; }
    public float Energy { get; protected set; }
    public bool IsDead { get; protected set; }
    public List<Vector2Int> Trail { get; } = new();
    public Vector2Int Direction { get; protected set; } = Vector2Int.right;
    public int Radius { get; set; } = 2;
    public int SightRadius { get; set; } = 12;
    public float ForwardBias { get; set; } = 0.01f;
    public int MaxLength { get; set; } = 20;
    public float Fatigue { get; set; } = 0.1f;
    public float Metabolism { get; set; } = 0.1f;


    protected WormBase(Vector2Int startPosition, float startEnergy)
    {
        Position = startPosition;
        Energy = startEnergy;
        Trail.Add(startPosition);
    }

    public virtual void Update(SimulationController simulation)
    {
        var best = Position;
        var bestScore = float.MinValue;

        for (var dy = -SightRadius; dy <= SightRadius; dy++)
        {
            for (var dx = -SightRadius; dx <= SightRadius; dx++)
            {
                if (dx == 0 && dy == 0) continue;
                if ((dx * dx) + (dy * dy) > SightRadius * SightRadius) continue;

                var x = Mathf.Clamp(Position.x + dx, 0, simulation.Width - 1);
                var y = Mathf.Clamp(Position.y + dy, 0, simulation.Height - 1);

                var p = new Vector2Int(x, y);
                var pixel = simulation.GetPixel(p);
                var food = pixel.r * Eats.r + pixel.g * Eats.g + pixel.b * Eats.b;

                var dir = new Vector2(dx, dy).normalized;
                var forward = Vector2.Dot(dir, Direction);
                var score = food + ForwardBias * forward;

                if (score <= bestScore) continue;

                bestScore = score;
                best = p;
            }
        }

        var stepX = Mathf.Clamp(best.x - Position.x, -1, 1);
        var stepY = Mathf.Clamp(best.y - Position.y, -1, 1);

        var newPos = new Vector2Int(
            Mathf.Clamp(Position.x + stepX, 0, simulation.Width - 1),
            Mathf.Clamp(Position.y + stepY, 0, simulation.Height - 1));

        Direction = new Vector2Int(newPos.x - Position.x, newPos.y - Position.y);

        Position = newPos;

        Energy += simulation.Eat(Position, Radius, Eats) * Metabolism - Fatigue;

        Trail.Add(Position);
        if (Trail.Count > MaxLength) Trail.RemoveAt(0);

        Debug.Log(Name);
    }
}
