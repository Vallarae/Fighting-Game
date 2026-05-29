using Player.Base.Attacks.Base;
using Player.Base.Controller;
using Player.Base.StateMachineSystem;
using UnityEngine;

namespace Player.Base.PlayerStates {
    public class StunnedState : IState {
        private readonly PlayerController _player;
        public Attack attackHitWith;

        public StunnedState(PlayerController player) {
            _player = player;
        }

        private int _minStunDuration;
        private int _maxStunDuration;
        private int _frame;
        private int _combo;

        public void Enter() {
            _frame = 0;
            _combo = 1;
            _player.PlayerAnimationController.UpdateValue("IsStunned", true);
            _minStunDuration = _player.recoveryFrames + attackHitWith.FramesToImpact;
            _maxStunDuration = _player.recoveryFramesAfterCombo + attackHitWith.FramesToImpact;
        }

        public void Tick() {
            _frame++;
            
            if (_frame >= _minStunDuration && _combo < 3)
                UpdateState();
            
            if (_frame >= _maxStunDuration && _combo >= 3)
                UpdateState();
        }

        private void UpdateState() {
            _player.Fms.ChangeState(IsGrounded ? _player.movement : _player.aerial);
        }

        public void Exit() {
            _player.PlayerAnimationController.UpdateValue("IsStunned", false);
        }

        public void ExtraHit() {
            _combo++;
            _frame = 0;
        }
        
        private bool IsGrounded => Physics.Raycast(_player.gameObject.transform.position, Vector3.down, 1.2f, LayerMask.GetMask("Ground"));
    }
}