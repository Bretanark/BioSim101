using System.Collections.Generic;
using UnityEngine;

public class Worm : Creature
{
    private const float MaxPopulation = 20f;
    private static int _nextId = 1;


    private int Id { get; } = _nextId++;
    public List<Vector2Int> Trail { get; } = new();
    public Vector2Int Direction { get; protected set; } = Vector2Int.right;
    public int Radius { get; set; } = 2;
    public int SightRadius { get; set; } = 12;
    public float ForwardBias { get; set; } = 0.01f;
    public int MaxLength { get; set; } = 20;
    public float BiteStrength { get; set; } = 0.5f;

    /// <summary>How much energy each cycle takes</summary>
    public float Fatigue { get; set; } = 0.1f;

    /// <summary>How much energy is gained by eating</summary>
    public float Metabolism { get; set; } = 0.10f;



    public Worm(Vector2Int startPosition, float startEnergy, Color color)
        : base(startPosition, startEnergy, color)
    {
        Trail.Add(startPosition);
    }

    public override string Name => $"{Color} worm {Id} at {Position} with energy {Energy}";

    public override Creature Reproduce(Vector2Int position, float energy) => new Worm(position, energy, Color);

    public override void CreateView(SimulationController controller)
    {
        var view = Object.Instantiate(controller.WormViewPrefab, controller.WormViewsParent);
        view.Bind(this, controller);
    }

    public override void Update(SimulationController simulation)
    {
        //Debug.Log(Name);

        var newPosition = GetNewPosition(simulation);

        Direction = new Vector2Int(newPosition.x - Position.x, newPosition.y - Position.y);
        Position = newPosition;

        var competition = Mathf.Max(0.1f, 1f - Population / MaxPopulation);
        Energy += simulation.Eat(Position, Trail[0], Radius, Color, BiteStrength) * Metabolism * competition - Fatigue;

        Trail.Add(Position);
        if (Trail.Count > MaxLength) Trail.RemoveAt(0);

        base.Update(simulation);
    }

    private Vector2Int GetNewPosition(SimulationController simulation)
    {
        var best = Position;
        var bestScore = float.MinValue;

        for (var dy = -SightRadius; dy <= SightRadius; dy++)
        {
            for (var dx = -SightRadius; dx <= SightRadius; dx++)
            {
                if (dx == 0 && dy == 0) continue;
                if ((dx * dx) + (dy * dy) > SightRadius * SightRadius) continue;
                if (Mathf.Abs(dx) <= 1 && Mathf.Abs(dy) <= 1) continue;

                var x = Mathf.Clamp(Position.x + dx, 0, simulation.Width - 1);
                var y = Mathf.Clamp(Position.y + dy, 0, simulation.Height - 1);

                var p = new Vector2Int(x, y);
                var pixel = simulation.GetPixel(p);
                var food = pixel.r * Color.r + pixel.g * Color.g + pixel.b * Color.b;

                var dir = new Vector2(dx, dy).normalized;
                var forward = Vector2.Dot(dir, Direction);
                var shyness = simulation.GetShynessPenalty(p, this);
                var score = food + ForwardBias * forward + Random.value * 0.01f - shyness;

                if (score <= bestScore) continue;

                bestScore = score;
                best = p;
            }
        }

        var stepX = Mathf.Clamp(best.x - Position.x, -1, 1);
        var stepY = Mathf.Clamp(best.y - Position.y, -1, 1);

        var result = new Vector2Int(
            Mathf.Clamp(Position.x + stepX, 0, simulation.Width - 1),
            Mathf.Clamp(Position.y + stepY, 0, simulation.Height - 1));

        return result;
    }

}
