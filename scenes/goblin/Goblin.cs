using System;
using Game.Abstracts;
using Godot;
using Godot.Collections;

public partial class Goblin : CombatantCharacter {
	public AnimationPlayer AnimationPlayer;
	private MultiplayerSynchronizer _synchronizer;
	
	public float Speed = 200.0f;
	public const float AttackRange = 90.0f;
	public enum Animations { attack }
	// sync positions for MultiplayerSynchronizer
	[Export] private Vector2 _syncPos = Vector2.Zero;
	[Export] private float _syncRot = 0f;

	private Array<Wizard> _wizards;
	
	public override void _Ready() {
	    base._Ready(); // Do not remove this lol
		
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_synchronizer = GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer");
		var nodes = GetTree().GetNodesInGroup("Player");
		_wizards = [];
		foreach (var node in nodes) {
			if (node is Wizard wizard) {
				_wizards.Add(wizard);
			}
		}
		
		_syncPos = GlobalPosition;
		_synchronizer.Synchronized += Synchronize;
	}

	public override void _PhysicsProcess(double delta) {
		if (Multiplayer.IsServer()) {
			_syncPos = GlobalPosition;
			_syncRot = Rotation;
		}
		
		MoveAndSlide();
	}
	
	public void Synchronize() {
		GlobalPosition = GlobalPosition.Lerp(_syncPos, 0.1f);
		Rotation = Mathf.LerpAngle(Rotation, _syncRot, 0.1f);
	}
	
	// TODO: Move this to a utility class if needed elsewhere
	public Wizard FindNearestWizard() {
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
