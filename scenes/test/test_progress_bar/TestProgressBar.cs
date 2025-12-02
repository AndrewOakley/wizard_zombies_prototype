using Game.Component;
using Godot;

public partial class TestProgressBar : ProgressBar {
	[Export] private HealthComponent _healthComponent;

	public override void _Ready() {
        _healthComponent.HealthChanged += OnHealthChanged;
		OnHealthChanged(_healthComponent.CurrentHealth);
    }

	private void OnHealthChanged(int newHealth) {
        GD.Print($"Health changed: {newHealth}");
		Value = newHealth;
    }
}
