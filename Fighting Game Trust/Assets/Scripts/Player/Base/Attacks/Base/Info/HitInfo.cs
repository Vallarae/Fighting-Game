using UnityEngine;

namespace Player.Base.Attacks.Base.Info {
    public struct HitInfo {
        [Header("Damage")]
        public int damage;
        public int chipDamage;
        public int guardDamage;

        [Space] [Header("Meter")] 
        public int attackerMeterGain;
        public int defenderMeterGain;
        public int attackerBurstGain;
        public int defenderBurstGain;

        [Space] [Header("Timing")] 
        public int hitStop;
        public int blockStop;
        public int hitStun;
        public int blockStun;
        public int groundBounceTime;
        public int wallStickTime;

        [Space] [Header("Launch Physics")] 
        public Vector2 initialVelocity;
        public float gravityScale;

        [Space] [Header("Position")] 
        public float attackerPushback;
        public float defenderPushback;
        public float cornerPushback;
        public float cameraShakeStrength;
        public float cameraShakeLength;

        [Space] [Header("Hit Stuff")] 
        public HitReaction hitReaction;

        [Space] [Header("Counter Hit")] 
        public bool counterHitOnly;
        public bool counterLaunch;
        public int counterHitStop;
        public int counterHitStun;
        
        [Space] [Header("Blocking")]
        public BlockType blockType;
    }

    public enum HitReaction {
        Standing,
        Crouching,
        Air,
        Launch,
        Blowback,
        HardKnockdown,
        SoftKnockdown,
        GroundBounce,
        WallBounce,
        WallStick,
        Crumple,
        Float,
        Spin,
        Reel,
        Stagger,
        Dizzy,
        GuardBreak,
        Capture,
        Vacuum,
        Custom
    }

    public enum BlockType {
        Mid,
        High,
        Low,
        Unblockable
    }
}