using System;
using System.Threading.Tasks;
using Godot;
using Game.Abstracts;

public partial class EnemySpawner : Node2D {
    [Export] private PackedScene _enemyCharacterScene;
    [Export] private Node _spawnPath;
        
    private Marker2D _spawnPoint;
    private AnimationPlayer _animationPlayer;
    private MultiplayerSpawner _multiplayerSpawner;

    public override void _Ready() {
        _spawnPoint = GetNode<Marker2D>("SpawnPoint");
        
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _multiplayerSpawner = GetNode<MultiplayerSpawner>("MultiplayerSpawner");
        
        _spawnPath ??= GetParent(); // If no spawn path is set, the parent will be the spawn path
        _multiplayerSpawner.SpawnPath = _spawnPath.GetPath();
    }

    public async Task PlaySpawnAnimation() {
        _animationPlayer.Play("spawning");
        await ToSignal(_animationPlayer, AnimationMixer.SignalName.AnimationFinished); 
    }
    
    public void SpawnEnemy() {
        var enemyCharacter = _enemyCharacterScene.Instantiate<CombatantCharacter>();
        enemyCharacter.GlobalPosition = _spawnPoint.GlobalPosition;
        _spawnPath.AddChild(enemyCharacter, true);
    }
}
