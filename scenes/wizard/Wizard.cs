using Game.Abstracts;
using Godot;

public partial class Wizard : CombatantCharacter {
    [Export]
    public float Speed = 200.0f;
    [Export]
    public float SprintSpeed = 350.0f;
    [Export]
    public PackedScene Projectile;

    private Sprite2D sprite2D;
    private Marker2D projectileSrc;
    private Polygon2D wand;

    public override void _Ready() {
        base._Ready();

        sprite2D = GetNode<Sprite2D>("Sprite2D");
        projectileSrc = GetNode<Marker2D>("%ProjectileSrc");
        wand = GetNode<Polygon2D>("Wand");
    }

    public override void _PhysicsProcess(double delta) {
        PlayerMovement();
        PlayerShoot();
    }

    private void PlayerMovement() {
        // Get input direction
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        // Check if sprinting
        bool isSprinting = Input.IsActionPressed("move_sprint");
        float currentSpeed = isSprinting ? SprintSpeed : Speed;

        // Apply movement
        Vector2 velocity = direction != Vector2.Zero ? direction * currentSpeed : Vector2.Zero;
        Velocity = velocity;

        MoveAndSlide();

        // Look at mouse
        Vector2 mousePos = GetGlobalMousePosition();
        wand.LookAt(mousePos);
    }

    private void PlayerShoot() {
        if (!Input.IsActionJustPressed("move_fire"))
            return;

        CharacterBody2D projectile = Projectile.Instantiate<CharacterBody2D>();
        projectile.GlobalPosition = projectileSrc.GlobalPosition;
        projectile.Rotation = wand.Rotation;
        GetTree().Root.AddChild(projectile);
    }
}
