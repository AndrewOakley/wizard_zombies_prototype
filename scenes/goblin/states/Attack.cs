using Godot;
using Game.Abstracts;

public partial class Attack : State {
    [Export] private Goblin _goblin;
    
    public override void OnPhysicsProcess(double delta) {
        var nearestWizard = _goblin.FindNearestWizard();

        if (nearestWizard == null) return;

        if (_goblin.AnimationPlayer.GetCurrentAnimation() == nameof(Goblin.Animations.attack)) {
            return;
        }
        
        var distanceToWizard = _goblin.GlobalPosition.DistanceTo(nearestWizard.GlobalPosition);
        if (distanceToWizard >= Goblin.AttackRange) {
            StateMachine.ChangeState(nameof(Follow));
            return;
        } 
        
        _goblin.AnimationPlayer.Play(nameof(Goblin.Animations.attack));
    }
}
