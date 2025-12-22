using Godot;
using Game.Abstracts;

public partial class Attack : State {
    [Export] private Goblin _goblin;
    
    public override void OnPhysicsProcess(double delta) {
        // entire state handled by server only
        if (!Multiplayer.IsServer()) return;
        
        var nearestWizard = _goblin.FindNearestWizard();
        if (nearestWizard == null) return;
        var wizardPosition = nearestWizard.GlobalPosition;

        if (_goblin.AnimationPlayer.GetCurrentAnimation() == nameof(Goblin.Animations.attack)) {
            return;
        }
        
        var distanceToWizard = _goblin.GlobalPosition.DistanceTo(wizardPosition);
        if (distanceToWizard >= Goblin.AttackRange) {
            StateMachine.ChangeState(nameof(Follow));
            return;
        }

        _goblin.AnimationPlayer.Play(nameof(Goblin.Animations.attack));
    }
}
