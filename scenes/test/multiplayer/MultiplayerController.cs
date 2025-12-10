using Godot;
using System;
using System.Xml.Schema;
using System.Linq;


// TEMP: multiplayer controller for connecting clients to P2P server
public partial class MultiplayerController : Node2D {
	[Export] private int _port = 8910;
	[Export] private string _address = "127.0.0.1";

	private ENetMultiplayerPeer _peer;
	private const int MAX_PLAYERS = 2;
	
	public override void _Ready() {
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
		if(OS.GetCmdlineArgs().Contains("--server")){
			HostGame();
		}
	}

	/// <summary>
	/// runs when the connection fails, and it runs only on the client
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	private void ConnectionFailed() {
		GD.Print("CONNECTION FAILED");
	}

	/// <summary>
	/// runs when the connection is successful and only runs on the clients
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	private void ConnectedToServer() {
		GD.Print("Connected To Server");
		RpcId(1, nameof(SendPlayerInformation), GetNode<LineEdit>("LineEdit").Text, Multiplayer.GetUniqueId());
	}

	/// <summary>
	/// Runs when a player disconnects and runs on all peers
	/// </summary>
	/// <param name="id">id of the player that disconnected</param>
	/// <exception cref="NotImplementedException"></exception>
	private void PeerDisconnected(long id) {
		GD.Print("Player Disconnected: " + id.ToString());
		GameManager.Players.Remove(GameManager.Players.First(i => i.Id == id));
		var players = GetTree().GetNodesInGroup("Player");
		
		foreach (var player in players) {
			if(player.Name == id.ToString()) {
				player.QueueFree();
			}
		}
	}

	/// <summary>
	/// Runs when a player connects and runs on all peers
	/// </summary>
	/// <param name="id">id of the player that connected</param>
	/// <exception cref="NotImplementedException"></exception>
	private void PeerConnected(long id) {
		GD.Print("Player Connected! " + id);
	}
	
	private void HostGame() {
		_peer = new ENetMultiplayerPeer();
		var error = _peer.CreateServer(_port, MAX_PLAYERS);
		if(error != Error.Ok) {
			GD.Print("error cannot host! :" + error);
			return;
		}
		_peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);

		Multiplayer.MultiplayerPeer = _peer;
		GD.Print("Waiting For Players!");
	}

	public void _on_host_button_down(){
		HostGame();
		SendPlayerInformation(GetNode<LineEdit>("LineEdit").Text, 1);
	}
	
	public void _on_host_headless_button_down(){
		HostGame();
		SendPlayerInformation("skip", 1);
	}
	
	public void _on_join_button_down(){
		_peer = new ENetMultiplayerPeer();
		_peer.CreateClient(_address, _port);

		_peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = _peer;
		GD.Print("Joining Game!");
	}
	
	public void _on_start_game_button_down(){
		Rpc(nameof(StartGame));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void StartGame() {
		var scene = ResourceLoader.Load<PackedScene>("res://scenes/test/test_map.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);
		this.Hide();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SendPlayerInformation(string name, int id) {
		var playerInfo = new PlayerInfo(){
			Name = name,
			Id = id
		};
		
		if (!GameManager.Players.Contains(playerInfo)){
			GameManager.Players.Add(playerInfo);
		}

		if (Multiplayer.IsServer()){
			foreach (var player in GameManager.Players) {
				Rpc(nameof(SendPlayerInformation), player.Name, player.Id);
			}
		}
	}
}
