using Player.Base.Controller;
using Player.Base.StateMachineSystem;
using UnityEngine;

namespace Player.Base.PlayerStates {
    public class BlockingState : IState {
        private PlayerController _player;

        public BlockingState(PlayerController player) {
            _player = player;
        }

        private int frames;
        
        public void Enter() {
            frames = 0;
        }
        public void Tick() {
            frames++;

            if (frames == 4) {
                _player.Fms.ChangeState(isGrounded ? _player.movement : _player.aerial);
            }
        }
        public void Exit() {
            
        }
        
        private bool isGrounded => Physics.Raycast(_player.gameObject.transform.position, Vector3.down, 1.2f, LayerMask.GetMask("Ground"));
    }
}