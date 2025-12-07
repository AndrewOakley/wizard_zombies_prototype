using Godot;

namespace Game.Abstracts;

public abstract partial class State : Node {
    [Export] public StateMachine StateMachine;
    
    public virtual void Enter() {}

    public virtual void Exit() {}

    public virtual void OnReady() {}

    public virtual void OnProcess(double delta) {}
    
    public virtual void OnPhysicsProcess(double delta) {}

    public virtual void OnInput(InputEvent @event) {}
}