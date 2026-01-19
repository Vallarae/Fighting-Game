using Player.Base.Attacks.Base;
using Player.Base.Controller;
using Player.Base.HitboxLookup;
using Player.Base.Interfaces;
using UnityEngine;

namespace Player.Base.Attacks.Damage {
    public class Damager : MonoBehaviour {
        private PlayerController _player;
        private IAttack _attack;

        public void Initialise(PlayerController player, IAttack attack) {
            _player = player;
            _attack = attack;
        }

        private int _frames;

        private void FixedUpdate() {
            _frames++;

            if (_frames >= _attack.FramesHitDuration()) {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other) {
            IDamageable damageable = other.GetComponent<IDamageable>();

            if (ReferenceEquals(damageable, null)) return;
            
            PlayerController player = PlayerRegistry.GetPlayer(other);
            if (ReferenceEquals(player, null)) return;
            if (ReferenceEquals(player, _player)) return;
            
            damageable.Damage(_player.damage);
        }
    }
}