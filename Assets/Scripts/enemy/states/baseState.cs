public abstract class baseState
{
    public enemy enemy; // instance of enemy class
    // instance of state machine class
    public stateMachine stateMachine;

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}