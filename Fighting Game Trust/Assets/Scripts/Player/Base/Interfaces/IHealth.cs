namespace Player.Base.Interfaces {
    public interface IHealth {
        public int MaxHealth();
        public int CurrentHealth { get; set; }
    }
}