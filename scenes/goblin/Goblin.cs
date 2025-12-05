using Game.Abstracts;
using Godot;

public partial class Goblin : CombatantCharacter {
	private const float Speed = 200.0f;
	private Godot.Collections.Array<Wizard> _wizards;

    public override void _Ready() {
		base._Ready();

		var nodes = GetTree().GetNodesInGroup("Player");
		_wizards = [];
		foreach (var node in nodes) {
			if (node is Wizard wizard) {
				_wizards.Add(wizard);
			}
		}
	}

	public override void _PhysicsProcess(double delta) {
		var nearestWizard = FindNearestWizard();
		
		if (nearestWizard != null) {
			var direction = (nearestWizard.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * Speed;
		} else {
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();
	}

	private Wizard FindNearestWizard() {
		if (_wizards == null || _wizards.Count == 0) {
			return null;
		}

		Wizard nearest = null;
		var minDistance = float.MaxValue;

		foreach (var wizard in _wizards) {
			if (wizard == null || !IsInstanceValid(wizard)) continue;

			var distance = GlobalPosition.DistanceSquaredTo(wizard.GlobalPosition);
			if (!(distance < minDistance)) continue;
			
			minDistance = distance;
			nearest = wizard;
		}

		return nearest;
	}
}
