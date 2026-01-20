using System.Collections.Generic;
using Player.Base.Attacks.Base;
using Player.Base.Controller;
using UnityEngine;
using Input = Player.Base.InputHandling.Input;

namespace Player.Base.Attacks.DefaultAttacks {
    public class DefaultKick : IAttack {
        private readonly PlayerController _player;

        public DefaultKick(PlayerController player) {
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
            
            //_player.SpawnHitbox(spawnPoint, size, this);
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