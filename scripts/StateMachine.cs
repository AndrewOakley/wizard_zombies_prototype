using System.Collections.Generic;
using Game.Abstracts;
using Godot;

public partial class StateMachine : Node {
    [Export] public State InitialState;
    
    private readonly Dictionary<string, State> _states = new();
    private State _currentState;

    public override void _Ready() {
        foreach (var node in GetChildren()) {
            if (node is not State state) continue;
            
            _states[state.Name] = state;
            state.StateMachine = this;
            state.OnReady();
            state.Exit(); // reset all states
        }

        _currentState = InitialState;
        _currentState.Enter();
    }

    public override void _Process(double delta) {
        _currentState.OnProcess(delta);
    }

    public override void _PhysicsProcess(double delta) {
        _currentState.OnPhysicsProcess(delta);
    }

    public override void _Input(InputEvent @event) {
        _currentState.OnInput(@event);
    }
    
    public void ChangeState(string stateName) {
        if (!_states.TryGetValue(stateName, out var value)) {
            GD.PrintErr($"State '{stateName}' not found in StateMachine.");
            return;
        }

        _currentState.Exit();
        _currentState = value;
        _currentState.Enter();
    }
}