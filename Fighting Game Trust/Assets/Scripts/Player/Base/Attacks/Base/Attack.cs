using System.Collections.Generic;
using Player.Base.Attacks.Base.Validator.Base;
using Player.Base.InputHandling;

/*
 * Get fucked lmao
 */
namespace Player.Base.Attacks.Base {
    [System.Serializable]
    public abstract class Attack {
        public virtual int FramesToImpact { get; set; }
        public virtual int FramesToEnd { get; set; }
        
        public virtual AttackStance RequiredStance { get; set; }

        public virtual void Enter() { }
        public virtual void Tick() { }
        public virtual void OnAttack() { }
        public virtual void Exit() { }

        public virtual List<int> directionInputs { get; set; }

        public virtual ButtonType button { get; set; }
    }
}