using Game.Abstracts;
using Game.Component;
using Godot;
using Constants = Game.Contstants.Constants;

public partial class Wizard : CombatantCharacter {
    [ExportGroup("Components")]
    [Export] public CurrencyComponent CurrencyComponent;
    
    [ExportGroup("Stats")]
    [Export] public float Speed = 200.0f;
    [Export] public float SprintSpeed = 350.0f;
    [Export] public float RollDistance { get; set; } = 200f;
    [Export] public float RollDuration { get; set; } = 0.3f;
    [Export] private PackedScene _projectileScene; // Temp: this will be in the specific spell eventually

    private Sprite2D _sprite2D;
    private Marker2D _projectileSrc;
    private Polygon2D _wand;
    private MultiplayerSynchronizer _multiplayerSynchronizer;
    private Sprite2D _predictor;
    private CollisionShape2D _collisionShape2D;

    private bool _isDodging = false;
    private uint _savedCollisionLayer;
    private uint _savedCollisionMask;
    private Vector2 _dodgeVelocity;
    private float _dodgeTimer;
    [Export] private Vector2 _syncPos = new Vector2(0,0); // exported for multiplayer synchronizer
    [Export] private Vector2 _syncVelocity = new Vector2(0,0);

    public override void _Ready() {
        base._Ready();

        _sprite2D = GetNode<Sprite2D>("Sprite2D");
        _projectileSrc = GetNode<Marker2D>("%ProjectileSrc");
        _wand = GetNode<Polygon2D>("Wand");
        _predictor = GetNode<Sprite2D>("Predictor");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        
        // Save initial collision layers to easily change them back
        _savedCollisionLayer = CollisionLayer;
        _savedCollisionMask = CollisionMask;
        
        _multiplayerSynchronizer = GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer");
        _multiplayerSynchronizer.SetMultiplayerAuthority(int.Parse(Name));

        if (_multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()) {
            _predictor.Hide();
        }
    }
    
    public override void _PhysicsProcess(double delta) {
        if (_multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()) {
            PlayerMovement(delta);
            _syncPos = GlobalPosition;
            _syncVelocity = Velocity;
        } else{
            _predictor.GlobalPosition = _syncPos;

            // send a ping request to the server with a timestamp (use RpcId to call remote)
            var authorityAverageRtt = GameManager.GetConnectionInformation(_multiplayerSynchronizer.GetMultiplayerAuthority()).AverageRttSeconds;
            if (authorityAverageRtt < 0.1f) {
                GlobalPosition = _syncPos;
            } else {
                var predictorGlobalPosition = _syncVelocity * (float)authorityAverageRtt + _syncPos;
                GlobalPosition = GlobalPosition.Lerp(predictorGlobalPosition, 0.1f);  
            }
        }
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
            
            if (Input.IsActionJustPressed("move_fire") && !Input.IsActionPressed("move_sprint"))
                Rpc(nameof(PlayerShoot));
        }

        // Look at mouse
        var mousePos = GetGlobalMousePosition();
        _wand.LookAt(mousePos);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void PlayerShoot() {
        var projectile = _projectileScene.Instantiate<Projectile>();
        projectile.Initialize(_projectileSrc.GlobalPosition, _wand.Rotation, this);
        projectile.HitArea += OnProjectileHit;
        GetTree().Root.AddChild(projectile);
    }
    
    private void OnProjectileHit(Area2D area2D) {
        var isEnemy = (area2D.CollisionLayer & (uint)Constants.CollisionLayer.Enemy) != 0;
        if (isEnemy) {
            CurrencyComponent.Gold += 10;
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
