using System;
using Player.Base.Attacks.Base;
using Player.Base.Controller;
using Player.Base.StateMachineSystem;
using Player.Base.Utils;
using UnityEngine;
using Input = Player.Base.InputHandling.Input;

namespace Player.Base.PlayerStates {
    public class MovementState : IState {
        private readonly PlayerController _player;
        
        private int _dashCooldown;
        private float _speedToUse;
        
        private bool _canDash = true;

        private int _lastHorizontalDir;

        public MovementState(PlayerController player) {
            _player = player;
        }

        public void Enter() {
            _dashCooldown = _player.dashCooldown;
            _speedToUse = _player.walkSpeed;
        }

        public void Tick() {
            IAttack attack = _player.attackResolver.Resolve();
            if (attack != null) {
                _player.Fms.ChangeState(new AttackState(_player, attack));
                return;
            }

            _dashCooldown--;
            
            HandleMovement();
        }
        public void Exit() { }

        private void HandleMovement() {
            Input input = _player.InputReader.GetLastInput(); //gets the most recent input from the player
            Vector2Int dir = DirectionUtils.NumpadToVector(input.direction);

            if (dir.y > 0) {
                _player.Fms.ChangeState(_player.aerial);
                return;
            }

            if (dir.y < 0) {
                _player.Fms.ChangeState(_player.crouch);
                return;
            }
            
            if (dir.x != 0 && dir.x != _lastHorizontalDir) _speedToUse = _player.walkSpeed;
            
            _lastHorizontalDir = dir.x;

            if (_dashCooldown <= 0) 
                DoDash(input, dir);
            
            if (!input.dashButtonDown) {
                _speedToUse = _player.walkSpeed;
                _canDash = true;
            }
            
            //maybe stop the movement for a few frames after the dash ?

            float targetSpeed = dir.x * _speedToUse;
            float speedDifference = targetSpeed - _player.Rigidbody.linearVelocity.x;

            float accelerationRate = Mathf.Abs(targetSpeed) > 0.01f ? _player.acceleration : _player.deceleration;

            float movement = Mathf.Pow(Math.Abs(speedDifference) * accelerationRate, 1) * Mathf.Sign(speedDifference);

            _player.Rigidbody.AddForce(movement * Vector3.right);
        }

        private void DoDash(Input input, Vector2Int dir) {
            if (!input.dashButtonDown) return;
            if (!_canDash) return;

            //WHY THE FUCK DOES THIS WORK ???... but not at the same time...
            //I gave up and tried using AI... AND IT STILL DOESN'T WORK
            //THE AI IS SPOUTING SOME RANDOM BULLSHIT
            //wait I have an idea...
            //nvm it never broke, I'm just stupid ;-;
            bool isTowards = dir.x != _player.DirectionToOtherPlayer();
            float dashForceToUse = isTowards ? _player.dashTowards : _player.dashAway;

            if (isTowards) _speedToUse = _player.runSpeed;

            Vector2 direction = dir;
            direction.y = 0;
            
            _player.Rigidbody.AddForce(direction * dashForceToUse, ForceMode.Impulse);
            _dashCooldown = _player.dashCooldown;
            _canDash = false;
        }
    }
}