using Godot;

public partial class PlayerHud : Control {
	[Export] private Wizard _player;
	
	private Label _goldLabel;
	
	public override void _Ready() {
		_goldLabel = GetNode<Label>("GoldLabel");
		_goldLabel.Text = $"Gold: {_player.Gold}";
		
		_player.GoldChanged += OnGoldChanged;
	}
	
	private void OnGoldChanged(int newGoldAmount) {
		_goldLabel.Text = $"Gold: {newGoldAmount}";
	}
}
