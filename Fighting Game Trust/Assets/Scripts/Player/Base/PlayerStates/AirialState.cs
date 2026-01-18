using Player.Base.Attacks.Base;
using Player.Base.Controller;
using Player.Base.StateMachineSystem;
using UnityEngine;
using Input = Player.Base.InputHandling.Input;

namespace Player.Base.PlayerStates {
    public class AerialState : IState {
        private readonly PlayerController _player;
        private int _dashCooldown;

        public AerialState(PlayerController player) {
            _player = player;
        }

        private int _frame;
        private bool _hasDoubleJump;
        private bool _canDoubleJump;

        public void Enter() {
            _frame = 0;
            _hasDoubleJump = false;
            _canDoubleJump = false;
            
            Input input = _player.inputReader.GetLastInput();

            Vector2Int dir = NumpadToVector(input.direction);
            
            Vector2 direction = dir;
            direction.x /= 2.5f;
            
            _player.rigidbody.AddForce(direction * _player.jumpForce, ForceMode.Impulse);
            _player.rigidbody.linearVelocity = new Vector2(0, _player.rigidbody.linearVelocity.y);
        }

        public void Tick() {
            IAttack attack = _player.attackResolver.Resolve();
            if (attack != null) {
                _player.fms.ChangeState(new AttackState(_player, attack));
                return;
            }
            
            HandleAerial();
        }
        
        public void Exit() { }

        private void HandleAerial() {
            _frame++;

            if (_frame < 15) return;
            
            Input input = _player.inputReader.GetLastInput(); //gets the most recent input from the player
            Vector2Int dir = NumpadToVector(input.direction);

            if (isGrounded) {
                _player.fms.ChangeState(_player.movement);
                return;
            }

            if (dir.y < 1) _canDoubleJump = true;

            if (_canDoubleJump && !_hasDoubleJump && dir.y > 0) {
                _player.rigidbody.linearVelocity = Vector2.zero;

                Vector2 direction = dir;
                direction.x /= 2.5f;
                
                _player.rigidbody.AddForce(direction * _player.jumpForce, ForceMode.Impulse);
                _hasDoubleJump = true;
            }
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

        private bool isGrounded => Physics.Raycast(_player.gameObject.transform.position, Vector3.down, 1.2f, LayerMask.GetMask("Ground"));
    }
}