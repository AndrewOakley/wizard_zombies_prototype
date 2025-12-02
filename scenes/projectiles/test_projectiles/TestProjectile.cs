using Godot;

public partial class TestProjectile : CharacterBody2D {
	private const float Speed = 300.0f;
	
	private Vector2 _direction;

    public override void _Ready() {
        base._Ready();
		_direction = new Vector2(1, 0).Rotated(Rotation);
    }

	public override void _PhysicsProcess(double delta)	{
		var velocity = Speed * _direction;

		Velocity = velocity;
		MoveAndSlide();
	}
}
