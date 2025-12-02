using System;
using Godot;

namespace Game.Component {
    public partial class HealthComponent : Node2D {
        [Signal] public delegate void HealthChangedEventHandler(int newHealth);
        [Signal] public delegate void DiedEventHandler();

        [Export] public int MaxHealth {
            get => maxHealth;
            private set {
                maxHealth = value;
                if (CurrentHealth > maxHealth) {
                    CurrentHealth = maxHealth;
                }
            }
        }
        [Export] public int CurrentHealth {
            get => currentHealth;
            private set {
                currentHealth = Math.Clamp(value, 0, MaxHealth);
                EmitSignal(SignalName.HealthChanged, currentHealth);

                if (currentHealth == 0) {
                    EmitSignal(SignalName.Died);
                }
            }
        }

        // backing fields
        private int maxHealth = 10;
        private int currentHealth = 10;

        public void damage(int amount) {
            CurrentHealth -= amount;
        }
    }
}