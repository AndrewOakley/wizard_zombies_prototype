using Godot;

public partial class TestProjectile : CharacterBody2D {
	public const float Speed = 300.0f;
	
	private Vector2 Direction;

    public override void _Ready() {
        base._Ready();
		Direction = new Vector2(1, 0).Rotated(Rotation);
    }

	public override void _PhysicsProcess(double _delta)	{
		Vector2 velocity;
		velocity = Speed * Direction;

		Velocity = velocity;
		MoveAndSlide();
	}
}
