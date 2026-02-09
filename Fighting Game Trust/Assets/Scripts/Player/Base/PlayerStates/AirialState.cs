using Player.Base.Attacks.Base;
using Player.Base.Controller;
using Player.Base.StateMachineSystem;
using Player.Base.Utils;
using UnityEngine;
using Input = Player.Base.InputHandling.Input;

namespace Player.Base.PlayerStates {
    public class AerialState : IState {
        private readonly PlayerController _player;

        private int _frame;
        private int _dashCooldown;
        private int _dashDuration;

        private bool _hasDoubleJump;
        private bool _canDoubleJump;
        private bool _canDash;
        private bool _isTowards;

        private float _dashForceToUse;
        private Vector2 _direction;

        public AerialState(PlayerController player) {
            _player = player;
        }

        public void Enter() {
            _frame = 0;
            _hasDoubleJump = false;
            _canDoubleJump = false;

            _canDash = true;
            _dashCooldown = _player.dashCooldown;
            _dashDuration = 0;

            Input input = _player.InputReader.GetLastInput();
            Vector2Int dir = DirectionUtils.NumpadToVector(input.direction);

            Vector2 direction = dir;
            direction.x /= 2.5f;

            if (IsGrounded) {
                _player.Rigidbody.linearVelocity = new Vector2(0, _player.Rigidbody.linearVelocity.y);
                _player.Rigidbody.AddForce(direction * _player.jumpForce, ForceMode.Impulse);
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
            _player.Rigidbody.useGravity = true;
        }

        private void HandleAerial() {
            _frame++;
            if (_frame < 15) return;

            _dashCooldown--;

            Input input = _player.InputReader.GetLastInput();
            Vector2Int dir = DirectionUtils.NumpadToVector(input.direction);

            if (IsGrounded) {
                _player.Fms.ChangeState(_player.movement);
                return;
            }

            if (OnTopOfPlayer()) {
                int pushDir = _player.DirectionToOtherPlayer();
                _player.Rigidbody.AddForce(new Vector2(pushDir, 0) * 0.5f, ForceMode.Acceleration);
            }

            if (dir.y < 1) _canDoubleJump = true;

            if (!_canDash) _dashDuration++;

            if (_isTowards && _dashDuration == 5 ||
                !_isTowards && _dashDuration == 1) {
                DashVelocity();
            }

            if (!_canDash && _dashDuration > _player.maxAirDashFrames) {
                _player.Rigidbody.linearVelocity = new Vector2(0, _player.Rigidbody.linearVelocity.y);
                _player.Rigidbody.useGravity = true;
            }

            if (_dashCooldown <= 0) DoDash(input, dir);

            if (_canDoubleJump && !_hasDoubleJump && dir.y > 0) {
                _player.Rigidbody.linearVelocity = Vector2.zero;

                Vector2 jumpDir = dir;
                jumpDir.x /= 2.5f;

                _player.Rigidbody.AddForce(jumpDir * _player.jumpForce, ForceMode.Impulse);
                _hasDoubleJump = true;
                _canDoubleJump = false;
                _canDash = false;
            }
        }

        private void DoDash(Input input, Vector2Int dir) {
            if (!input.dashButtonDown || !_canDash) return;

            if (dir.x == 0)
                dir.x = -_player.DirectionToOtherPlayer();

            _isTowards = dir.x == -_player.DirectionToOtherPlayer();
            _dashForceToUse = _isTowards ? _player.dashTowards : _player.dashAway;

            _direction = new Vector2(dir.x, 0);

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

        private bool IsGrounded =>
            Physics.Raycast(_player.transform.position + Vector3.up * 0.1f,
                            Vector3.down, 1.3f, LayerMask.GetMask("Ground"));

        private bool OnTopOfPlayer() {
            Vector3 pos = _player.transform.position;
            return Physics.Raycast(pos, Vector3.down, 1.2f, LayerMask.GetMask("Player")) ||
                   Physics.Raycast(pos + Vector3.right * 0.5f, Vector3.down, 1.2f, LayerMask.GetMask("Player")) ||
                   Physics.Raycast(pos - Vector3.right * 0.5f, Vector3.down, 1.2f, LayerMask.GetMask("Player"));
        }
    }
}
