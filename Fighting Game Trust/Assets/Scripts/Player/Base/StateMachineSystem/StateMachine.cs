namespace Player.Base.StateMachineSystem {
    public class StateMachine {
        public IState currentState { get; private set; }

        public void ChangeState(IState newState) {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }

        public void Tick() {
            currentState?.Tick();
        }
    }
}