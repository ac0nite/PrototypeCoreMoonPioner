using Application.StateMachine;
using Application.StateMachine.States;
using Common.StateMachine;
using DG.Tweening;
using Zenject;

namespace Application
{
    public class ApplicationInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InitialiseDoTween();
            InstallApplicationSignals();
            InstallStateMachine();
        }
        
        private void InitialiseDoTween()
        {
            DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(100, 50);
        }
        
        private void InstallApplicationSignals()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<GameplayStateMachine.Signals.NextState>().OptionalSubscriber();
        }
        
        private void InstallStateMachine()
        {
            Container.BindInterfacesTo<GameplayStateMachine>().AsSingle();
        
            Container.BindFactory<IState, InitialiseGameplayState.Factory>()
                .To<InitialiseGameplayState>()
                .WhenInjectedInto<GameplayStateMachine>();
        
            Container.BindFactory<IState, GameplayState.Factory>()
                .To<GameplayState>()
                .WhenInjectedInto<GameplayStateMachine>();
            
            Container.BindFactory<IState, ResetGameplayState.Factory>()
                .To<ResetGameplayState>()
                .WhenInjectedInto<GameplayStateMachine>();
        }
    }   
}