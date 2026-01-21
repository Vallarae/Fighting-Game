using System.Collections.Generic;
using Player.Base.Attacks.Base.Validator.Base;

namespace Player.Base.Attacks.Base {
    public class Attack {
        
        public Attack() {
            directionInputs = new List<int>();
        }
        
        public int FramesToImpact = 5;
        public int FramesToEnd = 5;
        public int HitboxLifetime = 15;

        public bool LowBlockable = false;
        public bool dealsKnockback = true;

        public AttackStance RequiredStance = AttackStance.Any;

        public virtual void Enter() { }
        public virtual void Tick() { }
        public virtual void OnAttack() { }
        public virtual void Exit() { }

        public List<int> directionInputs;

        public ButtonType button = ButtonType.Punch;
    }
}