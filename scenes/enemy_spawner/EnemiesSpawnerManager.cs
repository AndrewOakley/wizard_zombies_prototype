using Godot;
using System;
using System.Threading.Tasks;
using Godot.Collections;

public partial class EnemiesSpawnerManager : Node {
    private Array<EnemySpawner> _enemySpawners = [];

    public override void _Ready() {
        var nodes = GetChildren();
        foreach (var node in nodes) {
            if (node is not EnemySpawner spawner) continue;

            _enemySpawners.Add(spawner);
        }

        SpawnEnemy();
    }
    
    private async Task SpawnEnemy() {
        while (true) {
            await _enemySpawners.PickRandom().SpawnEnemy();
            await ToSignal(GetTree().CreateTimer(5f), SceneTreeTimer.SignalName.Timeout);
        }
    }
}
