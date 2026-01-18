using Player.Base.Attacks.Base;
using Player.Base.Controller;
using Player.Base.StateMachineSystem;
using Player.Base.Utils;
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
        private bool _canDash;

        public void Enter() {
            _frame = 0;
            _hasDoubleJump = false;
            _canDoubleJump = false;
            
            Input input = _player.inputReader.GetLastInput();

            Vector2Int dir = DirectionUtils.NumpadToVector(input.direction);
            
            Vector2 direction = dir;
            direction.x /= 2.5f;
            
            _player.rigidbody.AddForce(direction * _player.jumpForce, ForceMode.Impulse);
            _player.rigidbody.linearVelocity = new Vector2(0, _player.rigidbody.linearVelocity.y);

            _canDash = true;
            _dashCooldown = _player.dashCooldown;
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

            _dashCooldown--;
            
            Input input = _player.inputReader.GetLastInput(); //gets the most recent input from the player
            Vector2Int dir = DirectionUtils.NumpadToVector(input.direction);

            if (isGrounded) {
                _player.fms.ChangeState(_player.movement);
                return;
            }

            if (dir.y < 1) _canDoubleJump = true;
            
            if (_dashCooldown <= 0) DoDash(input, dir);

            if (_canDoubleJump && !_hasDoubleJump && dir.y > 0) {
                _player.rigidbody.linearVelocity = Vector2.zero;

                Vector2 direction = dir;
                direction.x /= 2.5f;
                
                _player.rigidbody.AddForce(direction * _player.jumpForce, ForceMode.Impulse);
                _hasDoubleJump = true;
                _canDoubleJump = false;
                _canDash = false;
            }
        }
        
        private void DoDash(Input input, Vector2Int dir) {
            if (!input.dashButtonDown) return;
            if (!_canDash) return;

            bool isTowards = dir.x != _player.DirectionToOtherPlayer();
            float dashForceToUse = isTowards ? _player.dashTowards : _player.dashAway;

            Vector2 direction = dir;
            direction.y = 0;
            
            _player.rigidbody.AddForce(direction * dashForceToUse, ForceMode.Impulse);
            _dashCooldown = _player.dashCooldown;
            _canDash = false;
            _hasDoubleJump = true;
        }

        private bool isGrounded => Physics.Raycast(_player.gameObject.transform.position, Vector3.down, 1.2f, LayerMask.GetMask("Ground"));
    }
}