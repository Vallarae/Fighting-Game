using System.Collections.Generic;
using Player.Base.Attacks.Base;
using Player.Base.Controller;
using UnityEngine;
using Input = Player.Base.InputHandling.Input;

namespace Player.Base.Attacks.DefaultAttacks{
    public class TestComplexAttack : IAttack{
        private readonly PlayerController _player;

        public TestComplexAttack(PlayerController player) {
            _player = player;
        }
        
        public int FramesToImpact() {
            return 5;
        }
        public int FramesToEnd() {
            return 25;
        }

        public int FramesHitDuration() {
            return 15;
        }

        public void Enter() {
            
        }

        public void OnAttack() {
            SpawnHitbox();
        }
        
        public void End() {
            
        }

        private void SpawnHitbox() {
            Vector3 spawnPoint = _player.transform.position - new Vector3(_player.DirectionToOtherPlayer(), 0, 0);
            Vector3 size = new Vector3(1, 1, 1);
            
            _player.SpawnHitbox(spawnPoint, size, this);
        }

        public AttackStance RequiredStance() {
            return AttackStance.Any;
        }

        public bool LowBlockable() {
            return false;
        }
        
        public List<Input> RequiredInputs() {
            Input inputOne = new Input {
                direction = 6
            };

            Input inputTwo = new Input {
                direction = 3
            };

            Input inputThree = new Input {
                direction = 2
            };

            Input inputFour = new Input {
                punchButtonDown = true
            };
            
            List<Input> inputs = new List<Input>();
            inputs.Add(inputOne);
            inputs.Add(inputTwo);
            inputs.Add(inputThree);
            inputs.Add(inputFour);
            
            return inputs;
        }

        public int MaxInputGap() {
            return 3;
        }
        
        public int DirectionTolerance() {
            return 1;
        }
    }
}