using Game.Abstracts;
using Game.Contstants;
using Godot;

namespace Game.Component;

public partial class HurtBoxComponent : Area2D {
    [Export] private HealthComponent _healthComponent;

    public override void _Ready() {
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D area) {
        if (area is HitBoxComponent hitBoxComponent) {
            _healthComponent.Damage(hitBoxComponent.Damage);
        }
    }
}
