using Godot;
using System.Collections.Generic;


// TEMP: Game manager to store connected player information, must be refactored eventually
public partial class GameManager : Node {
    public static readonly List<PlayerInfo> Players = [];
    private static readonly Dictionary<int, ConnectionInformation> Connections = new();

    public static void AddPlayer(PlayerInfo player) {
        Players.Add(player);
        Connections.Add(player.Id, new ConnectionInformation());
    }
    
    private double _pingTimer = 0.0;
    private const double PingInterval = 1.0;
    
    public override void _Process(double delta) {
        _pingTimer += delta;
        if (_pingTimer < PingInterval) return;
        _pingTimer = 0.0;
    
        foreach (var player in Players) {
            if (!Connections.TryGetValue(player.Id, out var value)) {
                value = new ConnectionInformation();
                Connections[player.Id] = value;
                GD.PushWarning("Player " + player.Id + " was not added properly, use AddPlayer method.");
            }
    
            // if the player is the current client, don't get the ping
            if (player.Id == Multiplayer.GetUniqueId()) {
                value.AddRttSample(0f);
                continue;
            }
    
            RpcId(player.Id, nameof(PingRequest), Time.GetUnixTimeFromSystem());
        }
    }

    public static ConnectionInformation GetConnectionInformation(int playerId) {
        if (Connections.TryGetValue(playerId, out var information)) return information;
        
        GD.PushError("Player " + playerId + " does not exist");
        return null;

    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void PingRequest(double clientSendTime) {
        // client receives clientSendTime and replies with it + external client time
        var senderId = Multiplayer.GetRemoteSenderId();
        RpcId(senderId, nameof(PingReply), clientSendTime, Time.GetUnixTimeFromSystem());
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void PingReply(double clientSendTime, double serverTime) {
        var playerId = Multiplayer.GetRemoteSenderId();
        
        // client computes RTT and stores half (seconds) for prediction
        var currentTime = Time.GetUnixTimeFromSystem();
        var rttSeconds = currentTime - clientSendTime;
        
        var playerConnection = Connections[playerId];
        playerConnection.AddRttSample(rttSeconds);
        GD.Print(
            "Connection from: ", Multiplayer.GetUniqueId() == 1 ? "Server " : "Client ",
            "Last RTT: " + (playerConnection.LastRttSeconds * 1000).ToString("F2") + " ms," +
            "Average RTT: " + (playerConnection.AverageRttSeconds * 1000).ToString("F2") + " ms"
        );
    }

    public class ConnectionInformation {
        private readonly Queue<double> _rttHistory = new();
        private const int MaxHistorySize = 10;
        
        public double LastRttSeconds = 0f;
        public double AverageRttSeconds = 0f;

        public double LastPingSeconds => LastRttSeconds / 2.0;
        public double AveragePingSeconds => AverageRttSeconds / 2.0;
        
        public void AddRttSample(double rtt) {
            var totalRtt = AverageRttSeconds * _rttHistory.Count + rtt;
            
            _rttHistory.Enqueue(rtt);
            if (_rttHistory.Count > MaxHistorySize) {
                var removedRtt = _rttHistory.Dequeue();
                totalRtt -= removedRtt;
            }
            
            AverageRttSeconds = totalRtt / _rttHistory.Count;
            LastRttSeconds = rtt;
        }
    }
}