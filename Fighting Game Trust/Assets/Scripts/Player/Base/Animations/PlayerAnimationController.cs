using Player.Base.Controller;
using UnityEngine;

namespace Player.Base.Animations {
    public class PlayerAnimationController : MonoBehaviour {
        [SerializeField] private Animator _animator;

        private PlayerController _player;
        
        private void Start() {
            _animator = GetComponentInChildren<Animator>();
            _player = GetComponent<PlayerController>();
        }

        public void Tick() {
            _player.PlayerAnimationController.UpdateValue("Velocity", _player.Rigidbody.linearVelocity.x * _player.Rigidbody.linearVelocity.x);
            
            int directionToOther = _player.DirectionToOtherPlayer();

            _animator.gameObject.transform.rotation = Quaternion.Euler(0, directionToOther == 1 ? -150 : 150, 0);
            _animator.gameObject.transform.localScale = new(directionToOther == 1 ? 0.64645f : -0.64645f, 0.64645f, 0.64645f);
        }
        
        public void UpdateValue(string key, float value) => _animator.SetFloat(key, value);
        public void UpdateValue(string key, int value) => _animator.SetInteger(key, value);
        public void UpdateValue(string key, bool value) => _animator.SetBool(key, value);
    }
}