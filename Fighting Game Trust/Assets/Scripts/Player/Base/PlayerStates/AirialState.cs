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
        private int _dashDuration;

        private float _dashForceToUse;
        private Vector2 _direction;
        private bool _isTowards;

        public void Enter() {
            _frame = 0;
            _hasDoubleJump = false;
            _canDoubleJump = false;
            
            Input input = _player.InputReader.GetLastInput();

            Vector2Int dir = DirectionUtils.NumpadToVector(input.direction);
            
            Vector2 direction = dir;
            direction.x /= 2.5f;

            if (IsGrounded) {
                _player.Rigidbody.AddForce(direction * _player.jumpForce, ForceMode.Impulse);
                _player.Rigidbody.linearVelocity = new Vector2(0, _player.Rigidbody.linearVelocity.y);
                
                _canDash = true;
                _dashCooldown = _player.dashCooldown;
                _dashDuration = 0;
            }
        }

        public void Tick() {
            Attack attack = _player.attackResolver.Resolve();
            if (attack != null) {
                _player.Fms.ChangeState(new AttackState(_player, attack));
                return;
            }
            
            HandleAerial();
        }

        public void Exit() {
            if (_canDash) return;
            
            _player.Rigidbody.linearVelocity = Vector2.zero;
        }

        private void HandleAerial() {
            _frame++;

            if (_frame < 15) return;

            _dashCooldown--;
            
            Input input = _player.InputReader.GetLastInput(); //gets the most recent input from the player
            Vector2Int dir = DirectionUtils.NumpadToVector(input.direction);

            if (IsGrounded) {
                _player.Fms.ChangeState(_player.movement);
                return;
            }

            if (OnTopOfPlayer()) {
                int direction =  _player.DirectionToOtherPlayer();
                
                _player.Rigidbody.AddForce(new Vector2(direction, 0) * 0.5f, ForceMode.Acceleration);
            }

            if (dir.y < 1) _canDoubleJump = true;

            if (!_canDash) _dashDuration++;

            if (_isTowards && _dashDuration == 5) {
                DashVelocity();
            } else if (!_isTowards && _dashDuration == 1) {
                DashVelocity();
            }

            if (!_canDash && _dashDuration > _player.maxAirDashFrames) {
                _player.Rigidbody.linearVelocity = new Vector2(0, _player.Rigidbody.linearVelocity.y);
                _player.Rigidbody.useGravity = true;
            }
            
            if (_dashCooldown <= 0) DoDash(input, dir);

            if (_canDoubleJump && !_hasDoubleJump && dir.y > 0) {
                _player.Rigidbody.linearVelocity = Vector2.zero;

                Vector2 direction = dir;
                direction.x /= 2.5f;
                
                _player.Rigidbody.AddForce(direction * _player.jumpForce, ForceMode.Impulse);
                _hasDoubleJump = true;
                _canDoubleJump = false;
                _canDash = false;
            }
        }
        
        private void DoDash(Input input, Vector2Int dir) {
            if (!input.dashButtonDown) return;
            if (!_canDash) return;

            _isTowards = dir.x != _player.DirectionToOtherPlayer();
            _dashForceToUse = _isTowards ? _player.dashTowards : _player.dashAway;

            _direction = dir;
            _direction.y = 0;
            
            _dashCooldown = _player.dashCooldown;
            _canDash = false;
            _hasDoubleJump = true;
            _dashDuration = 0;
            _player.Rigidbody.useGravity = false;
            _player.Rigidbody.linearVelocity = Vector2.zero;
        }

        private void DashVelocity() {
            _player.Rigidbody.AddForce(_direction * _dashForceToUse, ForceMode.Impulse);
        }

        private bool IsGrounded => Physics.Raycast(_player.gameObject.transform.position, Vector3.down, 1.2f, LayerMask.GetMask("Ground"));
        
        private bool OnTopOfPlayer() {
            
            bool one = Physics.Raycast(_player.gameObject.transform.position, Vector3.down, 1.2f, LayerMask.GetMask("Player"));
            bool two = Physics.Raycast(_player.gameObject.transform.position + new Vector3(0.5f, 0, 0), Vector3.down, 1.2f, LayerMask.GetMask("Player"));
            bool three = Physics.Raycast(_player.gameObject.transform.position - new Vector3(0.5f, 0, 0), Vector3.down, 1.2f, LayerMask.GetMask("Player"));

            return one || two || three;
        }
    }
}