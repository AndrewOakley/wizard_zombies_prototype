using Game.Abstracts;
using Godot;

namespace Game.Component;

public partial class HurtBoxComponent : Area2D {
    [Export] private HealthComponent _healthComponent;

    public override void _Ready() {
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D area) {
        if (area.Owner is Projectile projectile) {
            _healthComponent.Damage(projectile.Damage);
        }
    }
}
