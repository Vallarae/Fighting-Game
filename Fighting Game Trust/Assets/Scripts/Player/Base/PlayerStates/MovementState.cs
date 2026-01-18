using Player.Base.Attacks.Base;
using Player.Base.Controller;
using Player.Base.StateMachineSystem;
using UnityEngine;
using Input = Player.Base.InputHandling.Input;

namespace Player.Base.PlayerStates {
    public class MovementState : IState {
        private readonly PlayerController _player;
        
        private int _dashCooldown;

        public MovementState(PlayerController player) {
            _player = player;
        }

        public void Enter() {
            _dashCooldown = _player.dashCooldown;
        }

        public void Tick() {
            IAttack attack = _player.attackResolver.Resolve();
            if (attack != null) {
                _player.fms.ChangeState(new AttackState(_player, attack));
                return;
            }
            
            HandleMovement();
        }
        public void Exit() { }

        private void HandleMovement() {
            Input input = _player.inputReader.GetLastInput(); //gets the most recent input from the player
            Vector2Int dir = NumpadToVector(input.direction);

            if (dir.y > 0) {
                _player.fms.ChangeState(_player.aerial);
                return;
            }

            if (dir.y < 0) {
                _player.fms.ChangeState(_player.crouch);
                return;
            }

            _player.rigidbody.linearVelocity = new Vector2(
                dir.x * _player.walkSpeed,
                _player.rigidbody.linearVelocity.y
            );
        }
        
        private static Vector2Int NumpadToVector(int direction) => direction switch {
            1 => new(-1, -1),
            2 => new(0, -1),
            3 => new(1, -1),
            4 => new(-1, 0),
            5 => new(0, 0),
            6 => new(1, 0),
            7 => new(-1, 1),
            8 => new(0, 1),
            9 => new(1, 1),
            _ => Vector2Int.zero
        };
    }
}