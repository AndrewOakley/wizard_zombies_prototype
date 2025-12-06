using Godot;

namespace Game.Component;

public partial class CurrencyComponent : Node2D {
    [Signal] public delegate void GoldChangedEventHandler(int newGoldAmount);
    
    private int _gold;

    public int Gold {
        get => _gold;
        set {
            _gold = Mathf.Max(0, value);
            EmitSignal(SignalName.GoldChanged, _gold);
        }
    }
}
