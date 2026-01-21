using Player.Base.Attacks.Base;
using Player.Base.Attacks.Base.Validator.Base;
using Player.Base.Controller;
using UnityEngine;

namespace Player.Base.Attacks.DefaultAttacks.TestAttacks {
    public class AdvancedAttackTest : Attack{
        private readonly PlayerController _player;

        public AdvancedAttackTest(PlayerController player) {
            _player = player;

            FramesToImpact = 5;
            FramesToEnd = 25;
            HitboxLifetime = 15;

            RequiredStance = AttackStance.Standing;
            LowBlockable = true;
            
            directionInputs.Add(6);
            button = ButtonType.Punch;
        }

        public override void OnAttack() {
            Vector3 spawnPoint = _player.transform.position - new Vector3(_player.DirectionToOtherPlayer(), 0, 0);
            Vector3 size = new Vector3(1, 1, 1);
            Debug.Log("Right");
            
            _player.SpawnHitbox(spawnPoint, size, this);
        }
    }
}