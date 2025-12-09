using Godot;
using System.Collections.Generic;


// TEMP: Game manager to store connected player information, must be refactored eventually
public partial class GameManager : Node {
    public static readonly List<PlayerInfo> Players = [];
}