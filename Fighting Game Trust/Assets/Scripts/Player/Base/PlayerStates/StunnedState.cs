using Player.Base.Controller;
using Player.Base.StateMachineSystem;
using UnityEngine;

namespace Player.Base.PlayerStates {
    public class StunnedState : IState {
        private readonly PlayerController _player;

        public StunnedState(PlayerController player) {
            _player = player;
        }

        private int _frame;
        private int _combo;

        public void Enter() {
            _frame = 0;
            _combo = 1;
        }

        public void Tick() {
            _frame++;
            Debug.Log($"frame {_frame}");
            
            if (_frame >= _player.recoveryFrames && _combo < 3)
                UpdateState();
            
            if (_frame >= _player.recoveryFramesAfterCombo && _combo >= 3)
                UpdateState();
        }

        private void UpdateState() {
            _player.fms.ChangeState(isGrounded ? _player.movement : _player.aerial);
        }
        
        public void Exit() { }

        public void ExtraHit() {
            _combo++;
            _frame = 0;
        }
        
        private bool isGrounded => Physics.Raycast(_player.gameObject.transform.position, Vector3.down, 1.2f, LayerMask.GetMask("Ground"));
    }
}