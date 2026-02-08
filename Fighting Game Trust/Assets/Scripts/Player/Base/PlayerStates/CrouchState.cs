using Player.Base.Attacks.Base;
using Player.Base.Controller;
using Player.Base.StateMachineSystem;
using Player.Base.Utils;
using UnityEngine;
using Input = Player.Base.InputHandling.Input;

namespace Player.Base.PlayerStates {
    public class CrouchState : IState {
        private readonly PlayerController _player;

        public CrouchState(PlayerController player) {
            _player = player;
        }

        public void Enter() {
            _player.PlayerAnimationController.UpdateValue("Crouched", true);
            _player.Rigidbody.linearVelocity = Vector2.zero;
        }

        public void Tick() {
            Attack attack = _player.attackResolver.Resolve();
            if (attack != null) {
                _player.Fms.ChangeState(new AttackState(_player, attack));
                return;
            }
            
            Input input = _player.InputReader.GetLastInput();
            Vector2Int dir = DirectionUtils.NumpadToVector(input.direction);
            
            if (dir.y > -1) _player.Fms.ChangeState(_player.movement);
        }

        public void Exit() {
            _player.PlayerAnimationController.UpdateValue("Crouched", false);
        }
    }
}