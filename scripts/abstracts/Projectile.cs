using Godot;

namespace Game.Abstracts;

public abstract partial class Projectile : CharacterBody2D {
	[Signal] public delegate void HitAreaEventHandler(Area2D area2d);
    
    [Export] public int Damage { get; set; } = 1;
	[Export] public float Speed = 300.0f;
    [Export] public CombatantCharacter Sender { get; set; }
    
	private protected Area2D Area;	
	private Vector2 _direction;

	public void Initialize(Vector2 position, float rotation, CombatantCharacter sender) {
		GlobalPosition = position;
		Rotation = rotation;
		Sender = sender;
	}

    public override void _Ready() {
		Area = GetNode<Area2D>("Area2D");
		_direction = new Vector2(1, 0).Rotated(Rotation);
		Area.AreaEntered += OnAreaEntered;
		Area.BodyEntered += OnBodyEntered;
    }
    
    public override void _PhysicsProcess(double delta)	{
	    var velocity = Speed * _direction;

	    Velocity = velocity;
	    MoveAndSlide();
    }

    private protected virtual void OnAreaEntered(Area2D area2D) {
		EmitSignal(SignalName.HitArea, area2D);
		QueueFree();
    }

    private protected virtual void OnBodyEntered(Node2D node2d) {
		QueueFree();
    }
}