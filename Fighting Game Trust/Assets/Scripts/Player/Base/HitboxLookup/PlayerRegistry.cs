using System.Collections.Generic;
using Player.Base.Controller;
using UnityEngine;

namespace Player.Base.HitboxLookup {
    public static class PlayerRegistry {
        private static Dictionary<Collider, PlayerController> _players = new Dictionary<Collider, PlayerController>();

        public static void AddPlayer(Collider collider, PlayerController controller) {
            _players.Add(collider, controller);
        }

        public static PlayerController GetPlayer(Collider collider) {
            return _players[collider];
        }

        public static void RemovePlayer(Collider collider) {
            _players.Remove(collider);
        }
    }
}