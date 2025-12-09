using Godot;

// adds players to the current scene
public partial class SceneManager : Node2D {
    [Export] private PackedScene _playerScene;
    
    public override void _Ready() {
        var index = 0;
        foreach (var player in GameManager.Players) {
            var currentPlayer = _playerScene.Instantiate<Wizard>();
            currentPlayer.Name = player.Id.ToString();
            AddChild(currentPlayer);
            
            foreach (var node in GetTree().GetNodesInGroup("PlayerSpawnPoints")) {
                if (node is not Marker2D spawnPoint) continue;
                
                if(int.Parse(spawnPoint.Name) == index){
                    currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
                }

            }
            index ++;
        }
    }
}