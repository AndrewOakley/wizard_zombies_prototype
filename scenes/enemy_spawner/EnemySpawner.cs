using System;
using System.Threading.Tasks;
using Godot;
using Game.Abstracts;

public partial class EnemySpawner : Node2D {
    [Export]
    private PackedScene _enemyCharacterScene;
        
    private Marker2D _spawnPoint;
    private AnimationPlayer _animationPlayer;

    public override void _Ready() {
        _spawnPoint = GetNode<Marker2D>("SpawnPoint");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
    
    public async Task SpawnEnemy() {
        _animationPlayer.Play("spawning");
        await ToSignal(_animationPlayer, AnimationMixer.SignalName.AnimationFinished);
        
        var enemyCharacter = _enemyCharacterScene.Instantiate<CombatantCharacter>();
        enemyCharacter.GlobalPosition = _spawnPoint.GlobalPosition;
        GetParent().AddChild(enemyCharacter);
    }
}
