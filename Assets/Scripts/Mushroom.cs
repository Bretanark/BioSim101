using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Creature
{
    private const float MaxPopulation = 15f;
    private const float MaxPopulationBoost = 1.5f;
    private const float MinSecondsToMaturity = 15f;
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

    public float Radius =>
        Mathf.Clamp(
            MinRadius + Energy / EnergyPerRadius,
            MinRadius,
            MaxRadius);

    private int PixelRadius => Mathf.Max(MinRadius, Mathf.RoundToInt(Radius));

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
            var x = Position.x + Random.Range(PixelRadius, SpawnRadius) * (Random.value < 0.5 ? -1 : 1);
            if (x < 0 || x > controller.Width) continue;

            var y = Position.y - Random.Range(PixelRadius, SpawnRadius) * (Random.value < 0.5 ? -1 : 1);
            if (y < 0 || y > controller.Height) continue;

            results.Add(new Mushroom(new Vector2Int(x, y), Color));
        }

        Die(controller);

        return results.ToArray();
    }

    public override void CreateView(SimulationController controller)
    {
        var view = Object.Instantiate(controller.MushroomViewPrefab, controller.CreatureViewsParent);
        view.Bind(this, controller);
    }

    protected override void OnUpdate(SimulationController controller)
    {
        var food = controller.Eat(Position, Position, PixelRadius, Color, GrowthRate * Time.deltaTime, Metabolism);
        var populationShare = Mathf.Min(1f, MaxPopulation / Mathf.Max(1f, Population));
        var maxGrowthThisFrame = (ReproductionEnergy - startEnergy) / MinSecondsToMaturity * Time.deltaTime;
        var expectedFood = Mathf.Max(
            Mathf.Epsilon,
            Mathf.PI * Radius * Radius * GrowthRate * Time.deltaTime);
        var localNutrition = Mathf.Clamp01(food / expectedFood);
        var overpopulation = Mathf.Max(0f, Population / MaxPopulation - 1f);
        var crowdingPenalty = overpopulation * (1f - localNutrition) * maxGrowthThisFrame;
        var netEnergy = food * Metabolism * populationShare - Fatigue - crowdingPenalty;

        Energy += netEnergy <= 0f
            ? netEnergy
            : Mathf.Min(netEnergy, maxGrowthThisFrame);
    }

    protected override void OnDeath(SimulationController controller)
    {
        // On death, consume once for a visible "decomposition / collapse" effect.
        //controller.Eat(Position, Position, Radius, Color, 100f, false);
    }

}
