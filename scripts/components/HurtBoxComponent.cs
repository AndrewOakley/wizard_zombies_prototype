using Godot;

namespace Game.Component;

public partial class HurtBoxComponent : Area2D {
    [Export]
    private HealthComponent _healthComponent;

    public const string EnemyHurtboxGroup = "enemy_hurt_box_group";

    public override void _Ready() {
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D area) {
        _healthComponent.Damage(1);
    }
}
