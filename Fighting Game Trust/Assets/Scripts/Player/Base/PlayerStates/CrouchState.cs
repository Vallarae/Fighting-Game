using Player.Base.Attacks.Base;
using Player.Base.Controller;
using Player.Base.StateMachineSystem;
using UnityEngine;
using Input = Player.Base.InputHandling.Input;

namespace Player.Base.PlayerStates {
    public class CrouchState : IState {
        private readonly PlayerController _player;

        public CrouchState(PlayerController player) {
            _player = player;
        }

        public void Enter() {
            GameObject playerObject = _player.gameObject;
            playerObject.transform.localScale = new Vector3(1, 0.5f, 1);
            playerObject.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y - 0.5f, _player.transform.position.z);
            _player.rigidbody.linearVelocity = Vector2.zero;
        }

        public void Tick() {
            IAttack attack = _player.attackResolver.Resolve();
            if (attack != null) {
                _player.fms.ChangeState(new AttackState(_player, attack));
                return;
            }
            
            Input input = _player.inputReader.GetLastInput();
            Vector2Int dir = NumpadToVector(input.direction);
            
            if (dir.y > -1) _player.fms.ChangeState(_player.movement);
        }

        public void Exit() {
            GameObject playerObject = _player.gameObject;
            playerObject.transform.localScale = new Vector3(1, 1f, 1);
            playerObject.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y + 0.5f, _player.transform.position.z);
        }
        
        private static Vector2Int NumpadToVector(int direction) => direction switch {
            1 => new(-1, -1),
            2 => new(0, -1),
            3 => new(1, -1),
            4 => new(-1, 0),
            5 => new(0, 0),
            6 => new(1, 0),
            7 => new(-1, 1),
            8 => new(0, 1),
            9 => new(1, 1),
            _ => Vector2Int.zero
        };
    }
}