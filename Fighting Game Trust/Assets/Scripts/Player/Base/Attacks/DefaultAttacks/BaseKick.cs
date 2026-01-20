using Player.Base.Attacks.Base;
using Player.Base.Attacks.Base.Validator.Base;
using Player.Base.Controller;
using UnityEngine;

namespace Player.Base.Attacks.DefaultAttacks {
    public class BaseKick : Attack {
        private readonly PlayerController _player;

        public BaseKick(PlayerController player) {
            _player = player;

            FramesToImpact = 5;
            FramesToEnd = 25;
            HitboxLifetime = 15;

            RequiredStance = AttackStance.Standing;
            LowBlockable = true;
            
            directionInputs.Add(10);
            button = ButtonType.Kick;
        }

        public override void OnAttack() {
            Vector3 spawnPoint = _player.transform.position - new Vector3(_player.DirectionToOtherPlayer(), 0, 0);
            Vector3 size = new Vector3(1, 1, 1);
            
            _player.SpawnHitbox(spawnPoint, size, this);
        }
    }
}