using Core.Input;
using Gameplay.Constants;
using UnityEngine;
using Zenject;

namespace Core
{
    public class CoreInstaller : Installer<CoreInstaller>
    {
        private readonly Canvas _canvas;
        private readonly GameplaySettingsInstaller.GameplayResources _gameplayResources;

        public CoreInstaller(
            [Inject (Id = Constants.ID.GameplayCanvas)] Canvas canvas,
            GameplaySettingsInstaller.GameplayResources gameplayResources)
        {
            _canvas = canvas;
            _gameplayResources = gameplayResources;
        }
        public override void InstallBindings()
        {
            InputInstaller();
        }

        private void InputInstaller()
        {
            Container
                .Bind(typeof(IInputHandler))
                .To<InputHandler>()
                .FromComponentsInNewPrefab(_gameplayResources.JoystickPrefab)
                .UnderTransform(_canvas.transform)
                .AsSingle();
        }
    }
}