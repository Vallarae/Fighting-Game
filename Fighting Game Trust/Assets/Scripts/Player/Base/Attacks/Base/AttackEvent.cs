using Player.Base.Controller;

namespace Player.Base.Attacks.Base {
    public abstract class AttackEvent {
        public int frame;
        public abstract void Execute(PlayerController player);
    }
}