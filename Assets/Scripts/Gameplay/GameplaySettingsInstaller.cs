using System;
using Core.Input;
using Gameplay.Buildings;
using Gameplay.Characters;
using Gameplay.Constants;
using Gameplay.Warehouses;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameplaySettingsInstaller", menuName = "Installers/GameplaySettings")]
public class GameplaySettingsInstaller : ScriptableObjectInstaller<GameplaySettingsInstaller>
{
    [SerializeField] private GameplayResources _gameplayResources;
    [SerializeField] private Character.Settings _characterSettings;
    [SerializeField] private Manufacture.Settings _manufactureSettings;
    [SerializeField] private ResourceItemSettings _resourceSettings;
    public override void InstallBindings()
    {
        Container.BindInstances(_gameplayResources);
        Container.BindInstance(_characterSettings);
        Container.BindInstance(_manufactureSettings);
        Container.BindInstance(_resourceSettings);
    }
    
    [Serializable]
    public class GameplayResources
    {
        public InputHandler JoystickPrefab;
        public CharacterView CharacterPrefab;
        public BuildingView BuildingPrefab;
        public ResourceItemView ResourcePrefab;
        public InfoBoardView InfoBoardPrefab;
    }
}