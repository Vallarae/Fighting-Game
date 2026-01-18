using Player.Base.Attacks.Base;
using Player.Base.Controller;
using Player.Base.StateMachineSystem;

namespace Player.Base.PlayerStates {
    public class AttackState : IState {
        private readonly PlayerController _player;
        private readonly IAttack _attack;
        private int _frame;

        public AttackState(PlayerController player, IAttack attack) {
            _player = player;
            _attack = attack;
        }

        public void Enter() {
            _frame = 0;
            _attack.Enter();
        }

        public void Tick() {
            _frame++;

            if (_frame >= _attack.framesToEnd()) {
                _attack.End();
                _player.fms.ChangeState(_player.movement);
            }
        }
        public void Exit() { }
    }
}