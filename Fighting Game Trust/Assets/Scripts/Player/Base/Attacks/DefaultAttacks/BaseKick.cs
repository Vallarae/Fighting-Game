using Player.Base.Attacks.Base;
using Player.Base.Attacks.Base.Validator.Base;
using Player.Base.Controller;
using Player.Base.Utils;
using UnityEngine;
using Input = Player.Base.InputHandling.Input;

namespace Player.Base.Attacks.DefaultAttacks {
    public class BaseKick : Attack {
        private readonly PlayerController _player;

        public BaseKick(PlayerController player) {
            _player = player;

            FramesToImpact = 5;
            FramesToEnd = 25;
            HitboxLifetime = 5;

            RequiredStance = AttackStance.Any;
            LowBlockable = true;
            
            directionInputs.Add(10);
            button = ButtonType.Kick;
        }

        public override void Enter() {
            int kick = 5;

            Input input = _player.InputReader.GetLastInput();
            Vector2Int dir = DirectionUtils.NumpadToVector(input.direction);

            if (dir.y < 0) kick = 2;
            if (dir.y > 0) kick = 7;

            if (!Physics.Raycast(_player.transform.position, Vector3.down, 1.2f, LayerMask.GetMask("Ground"))) kick = 7;

            if (kick == 2) {
                FramesToImpact = 8;
                FramesToEnd = 24;
                LowBlockable = true;
            }
            else {
                LowBlockable = false;
            }

            if (kick == 5) {
                FramesToImpact = 6;
                FramesToEnd = 24;
            }
            
            _player.PlayerAnimationController.UpdateValue("Kick", kick);
        }

        public override void Exit() {
            _player.PlayerAnimationController.UpdateValue("Kick", 0);
        }

        public override void OnAttack() {
            Vector3 spawnPoint = _player.transform.position - new Vector3(_player.DirectionToOtherPlayer(), 0, 0);
            Vector3 size = new Vector3(1, 1, 1);
            
            _player.SpawnHitbox(spawnPoint, size, this);
        }
    }
}