using Godot;
using System.Threading.Tasks;
using Godot.Collections;

public partial class EnemiesSpawnerManager : Node {
    [Export] private bool _active = true;
    [Export] private MultiplayerSpawner _multiplayerSpawner;
    
    private Array<EnemySpawner> _enemySpawners = [];

    public override void _Ready() {
        // set the spawn function for both server and clients
        _multiplayerSpawner.SetSpawnFunction(new Callable(this, nameof(SpawnCallback)));
        
        var nodes = GetChildren();
        foreach (var node in nodes) {
            if (node is not EnemySpawner spawner) continue;

            _enemySpawners.Add(spawner);
            // TODO: dynamically set the spawnable nodes for the muiltiplayer spawner
        }
        
        if (!_active || !Multiplayer.IsServer()) return;
        
        // Only spawn enemies on server and MultiplayerSpawner will handle the node
        SpawnEnemyLoop();
    }
    
    private async void SpawnEnemyLoop() {
        while (true) {
            await ToSignal(GetTree().CreateTimer(5f), SceneTreeTimer.SignalName.Timeout);
            await SpawnEnemy();
        }
    }
    
    private async Task SpawnEnemy() {
        // get a random spawner index
        var rng = new RandomNumberGenerator();
        rng.Randomize();
        var spawnerIndex = rng.RandiRange(0, _enemySpawners.Count - 1);
        
        // rpc play spawn animation
        var spawner = _enemySpawners[spawnerIndex];
        await spawner.PlaySpawnAnimation();

        // prepare spawn context
        var spawnContext = new Dictionary() {
            ["spawnerIndex"] = spawnerIndex,
        };
        _multiplayerSpawner.Spawn(spawnContext);
    }
    
    // Called by MultiplayerSpawner to spawn the enemy
    private Node SpawnCallback(Variant data) {
        var spawnerContext = data.AsGodotDictionary();
        
        if (!spawnerContext.TryGetValue("spawnerIndex", out var spawnerIndexVariant)) {
            GD.PrintErr("Spawn context does not contain 'spawnerIndex' key.");
            return null;
        }
        
        var spawnerIndex = (int)spawnerIndexVariant;
        var enemy = _enemySpawners[spawnerIndex].GenerateEnemyForSpawn();
        return enemy;
    }
}
