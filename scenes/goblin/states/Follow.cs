using Godot;
using Game.Abstracts;

public partial class Follow : State {
    [Export] private Goblin _goblin;
    
    // predict the location of the goblin, server will have final authority
    public override void OnPhysicsProcess(double delta) {
        var nearestWizard = _goblin.FindNearestWizard();

        if (nearestWizard == null) {
            _goblin.Velocity = Vector2.Zero;
            return;
        }
		
        var direction = (nearestWizard.GlobalPosition - _goblin.GlobalPosition).Normalized();
        _goblin.LookAt(nearestWizard.GlobalPosition);

        // handle state change on server only
        if (Multiplayer.IsServer()) {
            var distanceToWizard = _goblin.GlobalPosition.DistanceTo(nearestWizard.GlobalPosition);
            if (distanceToWizard < Goblin.AttackRange) {
                StateMachine.ChangeState(nameof(Attack));
                return;
            }
        }
        
        _goblin.Velocity = direction * _goblin.Speed;
    }

    public override void Exit() {
        _goblin.Velocity = Vector2.Zero;
    }
}
