using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Creature
{
    private const float MaxPopulation = 20f;
    private const int MinRadius = 2;
    private const int MaxRadius = 20;
    private const float startEnergy = 10f;

    private const float GrowthRate = 0.01f;     // tiny, because Update() is every frame
    private const float EnergyPerRadius = 12f;      // tuning knob
    private const int SpawnRadius = 100;      // how far away new mushrooms spawn when this one reproduces

    /// <summary>How much energy each cycle takes</summary>
    public float Fatigue { get; set; } = 0.0001f;

    /// <summary>How much energy is gained by eating</summary>
    public float Metabolism { get; set; } = 100f;

    public int Radius =>
        Mathf.Clamp(
            MinRadius + Mathf.FloorToInt(Energy / EnergyPerRadius),
            MinRadius,
            MaxRadius);

    public override float AvoidanceRadius => Radius;
    public override string Name => $"Mushroom at {Position}";

    public Mushroom(Vector2Int position, Color color)
        : base(position, startEnergy, color)
    {
    }

    public override Creature[] Reproduce(SimulationController controller, Vector2Int position, float energy)
    {
        // Spawn some kids nearby
        var numberOfSpawn = Random.Range(1, 6);
        var results = new List<Creature>();
        for (var n = 0; n < numberOfSpawn; n++)
        {
            var x = Position.x + Random.Range(Radius, SpawnRadius) * (Random.value < 0.5 ? -1 : 1);
            if (x < 0 || x > controller.Width) continue;

            var y = Position.y - Random.Range(Radius, SpawnRadius) * (Random.value < 0.5 ? -1 : 1);
            if (y < 0 || y > controller.Height) continue;

            results.Add(new Mushroom(new Vector2Int(x, y), Color));
        }

        IsDead = true;

        return results.ToArray();
    }

    public override void CreateView(SimulationController controller)
    {
        var view = Object.Instantiate(controller.MushroomViewPrefab, controller.CreatureViewsParent);
        view.Bind(this, controller);
    }

    protected override void OnUpdate(SimulationController controller)
    {
        var competition = Mathf.Max(0.1f, 1f - Population / MaxPopulation);
        Energy += controller.Eat(Position, Position, Radius, Color, GrowthRate * Time.deltaTime, Metabolism) * Metabolism * competition - Fatigue;
    }

    protected override void OnDeath(SimulationController controller)
    {
        // On death, consume once for a visible "decomposition / collapse" effect.
        //controller.Eat(Position, Position, Radius, Color, 100f, false);
    }

}
