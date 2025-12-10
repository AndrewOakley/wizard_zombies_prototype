using System;
using System.Threading.Tasks;
using Godot;
using Game.Abstracts;

public partial class EnemySpawner : Node2D {
    [Export] private PackedScene _enemyCharacterScene;
        
    private Marker2D _spawnPoint;
    private AnimationPlayer _animationPlayer;

    public override void _Ready() {
        _spawnPoint = GetNode<Marker2D>("SpawnPoint");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
    
    // Generate an enemy node to be used by multiplayer spawner
    public Node GenerateEnemyForSpawn() {
        var enemy = _enemyCharacterScene.Instantiate<Goblin>();
        enemy.GlobalPosition = _spawnPoint.GlobalPosition;

        return enemy;
    }

    // play spawn animation on all clients
    public async Task PlaySpawnAnimation() {
        Rpc(nameof(RpcPlaySpawnAnimation));
        
        _animationPlayer.Play("spawning");
        await ToSignal(_animationPlayer, AnimationMixer.SignalName.AnimationFinished); 
    }
    
    // Rpc method for spawn animation on clients
    [Rpc]
    private void RpcPlaySpawnAnimation() {
        _animationPlayer.Play("spawning");
    }
}
