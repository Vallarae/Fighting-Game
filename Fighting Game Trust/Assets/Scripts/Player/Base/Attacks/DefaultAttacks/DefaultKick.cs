using System.Collections.Generic;
using Player.Base.Attacks.Base;
using Player.Base.InputHandling;

namespace Player.Base.Attacks.DefaultAttacks {
    public class DefaultKick : IAttack {
        public int framesToImpact() {
            return 15;
        }
        public int framesToEnd() {
            return 120;
        }

        public int framesHitDuration() {
            return 15;
        }

        public void Enter() {
            
        }
        public void End() {
            
        }

        public AttackStance RequiredStance() {
            return AttackStance.Standing;
        }

        public bool LowBlockable() {
            return false;
        }
        
        public List<Input> RequiredInputs() {
            Input input = new Input {
                direction = 10,
                kickButtonDown = true
            };
            
            List<Input> inputs = new List<Input>();
            inputs.Add(input);
            
            return inputs;
        }

        public int MaxInputGap() {
            return 1;
        }
        public int DirectionTolerance() {
            return 1;
        }
    }
}