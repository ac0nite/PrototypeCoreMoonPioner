using System;
using Core.Input;
using Gameplay.Buildings;
using Gameplay.Characters;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameplaySettingsInstaller", menuName = "Installers/GameplaySettings")]
public class GameplaySettingsInstaller : ScriptableObjectInstaller<GameplaySettingsInstaller>
{
    [SerializeField] private GameplayResources _gameplayResources;
    [SerializeField] private Character.Settings _characterSettings;
    public override void InstallBindings()
    {
        Container.BindInstances(_gameplayResources);
        Container.BindInstance(_characterSettings);
    }
    
    [Serializable]
    public class GameplayResources
    {
        public InputHandler JoystickPrefab;
        public CharacterView CharacterPrefab;
        public BuildingView BuildingPrefab;
    }
}