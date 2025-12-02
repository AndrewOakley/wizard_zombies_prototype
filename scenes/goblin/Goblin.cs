using Game.Abstracts;
using Godot;

public partial class Goblin : CombatantCharacter {
	public const float Speed = 200.0f;
	private Godot.Collections.Array<Wizard> Wizards;

    public override void _Ready() {
		base._Ready();

		var nodes = GetTree().GetNodesInGroup("Player");
		Wizards = new Godot.Collections.Array<Wizard>();
		foreach (Node node in nodes) {
			if (node is Wizard wizard) {
				Wizards.Add(wizard);
			}
		}
	}

	public override void _PhysicsProcess(double delta) {
		Wizard nearestWizard = FindNearestWizard();
		
		if (nearestWizard != null) {
			Vector2 direction = (nearestWizard.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * Speed;
		} else {
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();
	}

	private Wizard FindNearestWizard() {
		if (Wizards == null || Wizards.Count == 0) {
			return null;
		}

		Wizard nearest = null;
		float minDistance = float.MaxValue;

		foreach (Wizard wizard in Wizards) {
			if (wizard == null || !IsInstanceValid(wizard)) {
				continue;
			}

			float distance = GlobalPosition.DistanceSquaredTo(wizard.GlobalPosition);
			if (distance < minDistance) {
				minDistance = distance;
				nearest = wizard;
			}
		}

		return nearest;
	}
}
