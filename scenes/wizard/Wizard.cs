using Game.Abstracts;
using Godot;

public partial class Wizard : CombatantCharacter {
    [Export]
    public float Speed = 200.0f;
    [Export]
    public float SprintSpeed = 350.0f;
    [Export]
    public PackedScene Projectile;

    private Sprite2D _sprite2D;
    private Marker2D _projectileSrc;
    private Polygon2D _wand;

    public override void _Ready() {
        base._Ready();

        _sprite2D = GetNode<Sprite2D>("Sprite2D");
        _projectileSrc = GetNode<Marker2D>("%ProjectileSrc");
        _wand = GetNode<Polygon2D>("Wand");
    }

    public override void _PhysicsProcess(double delta) {
        PlayerMovement();
        PlayerShoot();
    }

    private void PlayerMovement() {
        // Get input direction
        var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        // Check if sprinting
        var isSprinting = Input.IsActionPressed("move_sprint");
        var currentSpeed = isSprinting ? SprintSpeed : Speed;

        // Apply movement
        var velocity = direction != Vector2.Zero ? direction * currentSpeed : Vector2.Zero;
        Velocity = velocity;

        MoveAndSlide();

        // Look at mouse
        var mousePos = GetGlobalMousePosition();
        _wand.LookAt(mousePos);
    }

    private void PlayerShoot() {
        if (!Input.IsActionJustPressed("move_fire"))
            return;

        var projectile = Projectile.Instantiate<CharacterBody2D>();
        projectile.GlobalPosition = _projectileSrc.GlobalPosition;
        projectile.Rotation = _wand.Rotation;
        GetTree().Root.AddChild(projectile);
    }
}
