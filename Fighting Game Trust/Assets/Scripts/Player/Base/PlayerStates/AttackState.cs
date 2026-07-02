using Player.Base.Attacks.Base;
using Player.Base.Attacks.Base.Info;
using Player.Base.Controller;
using Player.Base.StateMachineSystem;

namespace Player.Base.PlayerStates {
    public class AttackState : IState {
        private PlayerController _controller;
        private Attack _currentAttack;
        private int _currentFrame;
        
        public AttackState(PlayerController playerController, Attack attack) {
            _controller = playerController;
            _currentAttack = attack;
        }
        
        public void Enter() {
            _currentFrame = 0;
        }

        public void Tick() {
            _currentFrame++;
            
            foreach (AttackEvent attackEvent in _currentAttack.events) {
                if (attackEvent.frame == _currentFrame) attackEvent.Execute(_controller);
            }

            foreach (HitboxDefinition hitbox in _currentAttack.hitboxes) {
                
            }
        }

        public void Exit() { }
    }
}