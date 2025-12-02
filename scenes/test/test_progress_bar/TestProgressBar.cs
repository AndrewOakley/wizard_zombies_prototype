using Game.Component;
using Godot;

public partial class TestProgressBar : ProgressBar {
	[Export] private HealthComponent healthComponent;

	public override void _Ready() {
        healthComponent.HealthChanged += OnHealthChanged;
		OnHealthChanged(healthComponent.CurrentHealth);
    }

	private void OnHealthChanged(int newHealth) {
        GD.Print($"Health changed: {newHealth}");
		Value = newHealth;
    }
}
