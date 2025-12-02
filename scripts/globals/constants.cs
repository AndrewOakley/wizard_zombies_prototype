using System;

public static class Constants {
    [Flags]
    public enum CollisionLayer {
        Environment  = 1 << 0, 
        Player       = 1 << 1, 
        Enemy        = 1 << 2, 
        PlayerAttack = 1 << 3, 
        EnemyAttack  = 1 << 4, 
    }
}