using Game.Abstracts;
using Godot;
using Constants = Game.Contstants.Constants;

public partial class Wizard : CombatantCharacter {
    [Signal] public delegate void GoldChangedEventHandler(int newGoldAmount);
    
    [Export] public float Speed = 200.0f;
    [Export] public float SprintSpeed = 350.0f;
    [Export] public PackedScene Projectile;
    [Export] public float RollDistance { get; set; } = 300f;
    [Export] public float RollDuration { get; set; } = 0.4f;
    
    private Sprite2D _sprite2D;
    private Marker2D _projectileSrc;
    private Polygon2D _wand;

    public int Gold {
        get => _gold;
        set {
            _gold = value;
            EmitSignal(SignalName.GoldChanged, _gold);
        }
    }

    private bool _isDodging = false;
    private uint _savedCollisionLayer;
    private uint _savedCollisionMask;
    private Vector2 _dodgeVelocity;
    private float _dodgeTimer;
    private int _gold;

    public override void _Ready() {
        base._Ready();

        _sprite2D = GetNode<Sprite2D>("Sprite2D");
        _projectileSrc = GetNode<Marker2D>("%ProjectileSrc");
        _wand = GetNode<Polygon2D>("Wand");
        
        // Save initial collision layers to easily change them back
        _savedCollisionLayer = CollisionLayer;
        _savedCollisionMask = CollisionMask;
    }

    public override void _PhysicsProcess(double delta) {
        PlayerMovement(delta);
    }

    private void PlayerMovement(double delta) {
        var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        
        // Get input direction
        if (Input.IsActionJustPressed("move_dodge_roll") && !_isDodging) {
            DodgeRoll(direction);
        } else if (_isDodging) {
            var motion = _dodgeVelocity * (float)delta;

            // This moves the character using physics and stops on collision
            var collision = MoveAndCollide(motion);

            _dodgeTimer -= (float)delta;
            if (_dodgeTimer <= 0 || collision != null) {
                EndDodgeRoll();
            }
        } else {
            // Check if sprinting
            var isSprinting = Input.IsActionPressed("move_sprint");
            var currentSpeed = isSprinting ? SprintSpeed : Speed;
    
            // Apply movement
            var velocity = direction != Vector2.Zero ? direction * currentSpeed : Vector2.Zero;
            Velocity = velocity;
            MoveAndSlide();
            PlayerShoot();
        }

        // Look at mouse
        var mousePos = GetGlobalMousePosition();
        _wand.LookAt(mousePos);
    }

    private void PlayerShoot() {
        if (!Input.IsActionJustPressed("move_fire") || Input.IsActionPressed("move_sprint"))
            return;

        var projectile = Projectile.Instantiate<Projectile>();
        projectile.Initialize(_projectileSrc.GlobalPosition, _wand.Rotation, this);
        projectile.HitArea += OnProjectileHit;
        GetTree().Root.AddChild(projectile);
    }
    
    private void OnProjectileHit(Area2D area2D) {
        var isEnemy = (area2D.CollisionLayer & (uint)Constants.CollisionLayer.Enemy) != 0;
        if (isEnemy) {
            Gold += 10;
        }
    } 
    
    private void DodgeRoll(Vector2 direction) {
        var norm = direction.Normalized();
        if (norm == Vector2.Zero)
            norm = Vector2.Right;

        if (HurtBoxComponent != null)
            HurtBoxComponent.Monitoring = false;

        CollisionMask = (uint)Constants.CollisionLayer.Environment;

        _isDodging = true;
        _dodgeVelocity = norm * (RollDistance / RollDuration);
        _dodgeTimer = RollDuration;
    }
    
    private void EndDodgeRoll() {
        _isDodging = false;
        ResetCollisions();
    }

    private void ResetCollisions() {
        CollisionLayer = _savedCollisionLayer;
        CollisionMask = _savedCollisionMask;
        
        if (HurtBoxComponent != null) {
            HurtBoxComponent.Monitoring = true;
        }
    }
}
