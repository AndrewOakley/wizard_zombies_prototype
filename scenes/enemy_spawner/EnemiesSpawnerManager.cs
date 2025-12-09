using Godot;
using System.Threading.Tasks;
using Godot.Collections;

public partial class EnemiesSpawnerManager : Node {
    [Export] private bool _active = true;
    
    private Array<EnemySpawner> _enemySpawners = [];

    public override void _Ready() {
        var nodes = GetChildren();
        foreach (var node in nodes) {
            if (node is not EnemySpawner spawner) continue;

            _enemySpawners.Add(spawner);
        }

        if (!_active || !Multiplayer.IsServer()) return;
        
        // Only spawn enemies on server and MultiplayerSpawner will handle the node
        _ = SpawnEnemy();
    }
    
    private async Task SpawnEnemy() {
        while (true) {
            await ToSignal(GetTree().CreateTimer(5f), SceneTreeTimer.SignalName.Timeout);
            _enemySpawners.PickRandom().SpawnEnemy();
        }
    }
}
