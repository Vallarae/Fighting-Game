namespace Player.Base.StateMachineSystem {
    public interface IState {
        void Enter();
        void Tick();
        void Exit();
    }
}