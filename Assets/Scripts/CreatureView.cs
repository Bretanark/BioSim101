using UnityEngine;

public abstract class CreatureView<TCreature> : MonoBehaviour
    where TCreature : Creature
{
    protected TCreature Creature { get; private set; }
    protected SimulationController Controller { get; private set; }

    private float _fade;

    public void Bind(TCreature creature, SimulationController controller)
    {
        Creature = creature;
        Controller = controller;
        OnBind();
    }

    protected virtual void OnBind() { }

    protected virtual void Update()
    {
        if (Creature.IsDead)
        {
            _fade += Time.deltaTime / 5f;
            UpdateDying(1f - Mathf.Clamp01(_fade));

            if (_fade >= 1f)
                Destroy(gameObject);

            return;
        }

        UpdateAlive();
    }

    protected abstract void UpdateAlive();

    protected virtual void UpdateDying(float alpha) { }

}
