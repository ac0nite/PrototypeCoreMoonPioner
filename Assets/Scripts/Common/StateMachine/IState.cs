namespace Common.StateMachine
{
    public interface IState
    {
        void OnEnter();
        void OnExit();
    }
}