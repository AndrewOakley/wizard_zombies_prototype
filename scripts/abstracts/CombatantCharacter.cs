using Game.Component;
using Godot;

namespace Game.Abstracts {
    public abstract partial class CombatantCharacter : CharacterBody2D {
        [Export] protected HealthComponent healthComponent;
        
        protected HurtBoxComponent hurtBoxComponent;

        public override void _Ready() {
            if (healthComponent == null) {
                healthComponent = GetNodeOrNull<HealthComponent>("HealthComponent");
            }
            
            hurtBoxComponent = GetNodeOrNull<HurtBoxComponent>("HurtBoxComponent");
            
            if (healthComponent != null) {
                healthComponent.Died += OnDeath;
                healthComponent.HealthChanged += OnHealthChanged;
            }
            
            if (hurtBoxComponent != null) {
                hurtBoxComponent.AreaEntered += OnHurtBoxAreaEntered;
                hurtBoxComponent.BodyEntered += OnHurtBoxBodyEntered;
            }
        }

        public override void _ExitTree() {
            base._ExitTree();
            
            // Unsubscribe from events to prevent memory leaks
            if (healthComponent != null) {
                healthComponent.Died -= OnDeath;
                healthComponent.HealthChanged -= OnHealthChanged;
            }
            
            if (hurtBoxComponent != null) {
                hurtBoxComponent.AreaEntered -= OnHurtBoxAreaEntered;
                hurtBoxComponent.BodyEntered -= OnHurtBoxBodyEntered;
            }
        }

        /// <summary>
        /// Called when this character dies. Override to add custom death behavior.
        /// </summary>
        protected virtual void OnDeath() {
            GD.Print($"{Name} has died!");
            QueueFree();
        }

        /// <summary>
        /// Called when health changes. Override to add custom behavior like visual feedback.
        /// </summary>
        protected virtual void OnHealthChanged(int newHealth) {
            // Override in derived classes for custom behavior
        }

        /// <summary>
        /// Called when another Area2D enters this character's hurtbox.
        /// </summary>
        protected virtual void OnHurtBoxAreaEntered(Area2D area) {
            // Override in derived classes for custom behavior
        }

        /// <summary>
        /// Called when another body enters this character's hurtbox.
        /// </summary>
        protected virtual void OnHurtBoxBodyEntered(Node2D body) {
            // Override in derived classes for custom behavior
        }

        /// <summary>
        /// Applies damage to this character's health component.
        /// </summary>
        public virtual void TakeDamage(int amount) {
            if (healthComponent != null) {
                healthComponent.Damage(amount);
            }
        }

        /// <summary>
        /// Gets the current health of this character.
        /// </summary>
        public int GetCurrentHealth() {
            return healthComponent?.CurrentHealth ?? 0;
        }

        /// <summary>
        /// Gets the maximum health of this character.
        /// </summary>
        public int GetMaxHealth() {
            return healthComponent?.MaxHealth ?? 0;
        }
    }
}
