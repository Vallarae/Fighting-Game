using Player.Base.Attacks.Base;
using Player.Base.Controller;
using Player.Base.StateMachineSystem;
using UnityEngine;

namespace Player.Base.PlayerStates {
    public class AttackState : IState {
        private readonly PlayerController _player;
        private readonly Attack _attack;
        private int _frame;

        public AttackState(PlayerController player, Attack attack) {
            _player = player;
            _attack = attack;
        }

        public void Enter() {
            _frame = 0;
            _attack.Enter();
        }

        public void Tick() {
            _frame++;
            
            if (_frame == _attack.FramesToImpact) {
                _attack.OnAttack();
                return;
            }

            if (_frame >= _attack.FramesToEnd) {
                _attack.Exit();
                _player.Fms.ChangeState(IsGrounded ? _player.movement : _player.aerial);
            }
        }

        public void Exit() {
            _player.InputReader.Resume();
            _player.attackResolver.canAttack = true;
        }
        
        private bool IsGrounded => Physics.Raycast(_player.gameObject.transform.position, Vector3.down, 1.2f, LayerMask.GetMask("Ground"));
    }
}