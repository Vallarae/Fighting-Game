namespace Player.Base.Interfaces {
    public interface IHealth {
        public int maxHealth();
        public int currentHealth { get; set; }
    }
}