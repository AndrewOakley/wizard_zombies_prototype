using System;
using Godot;

namespace Game.Component {
    public partial class HealthComponent : Node2D {
        [Signal] public delegate void HealthChangedEventHandler(int newHealth);
        [Signal] public delegate void DiedEventHandler();

        [Export] public int MaxHealth {
            get => _maxHealth;
            private set {
                _maxHealth = value;
                if (CurrentHealth > _maxHealth) {
                    CurrentHealth = _maxHealth;
                }
            }
        }
        [Export] public int CurrentHealth {
            get => _currentHealth;
            private set {
                _currentHealth = Math.Clamp(value, 0, MaxHealth);
                EmitSignal(SignalName.HealthChanged, _currentHealth);

                if (_currentHealth == 0) {
                    EmitSignal(SignalName.Died);
                }
            }
        }

        // backing fields
        private int _maxHealth = 10;
        private int _currentHealth = 10;

        public void Damage(int amount) {
            CurrentHealth -= amount;
        }
    }
}