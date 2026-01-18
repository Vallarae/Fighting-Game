using System.Collections.Generic;
using Player.Base.InputHandling;

/*
 * Damage is done though hitboxes, not this function
 */
namespace Player.Base.Attacks.Base {
    public interface IAttack {
        public int framesToImpact();
        public int framesToEnd();
        public int framesHitDuration(); //used to give the hitbox a life time

        public void Enter();
        public void End();

        public AttackStance RequiredStance();
        
        public bool LowBlockable(); //this value is used when an attack news a low block to be blocked

        //for this I will be adding extra directional things, for example 10 = no direction
        public List<Input> RequiredInputs();

        public int MaxInputGap(); //how many frames between steps are allowed
        public int DirectionTolerance(); //0 = strict. 1 = lenient
    }
}