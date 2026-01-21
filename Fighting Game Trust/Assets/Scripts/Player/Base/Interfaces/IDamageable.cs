using Player.Base.Attacks.Base;

namespace Player.Base.Interfaces {
    public interface IDamageable {
        public void Damage(int amount, Attack attack);
    }
}