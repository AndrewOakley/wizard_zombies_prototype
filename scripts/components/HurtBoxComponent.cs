using Godot;

namespace Game.Component {
    public partial class HurtBoxComponent : Area2D {
        [Export]
        private HealthComponent healthComponent;

        public const string ENEMY_HURTBOX_GROUP = "enemy_hurt_box_group";

        public override void _Ready() {
            AreaEntered += OnAreaEntered;
        }

        private void OnAreaEntered(Area2D area) {
            healthComponent.damage(1);
        }
    }
}