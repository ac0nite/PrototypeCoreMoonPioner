using Core;
using Gameplay.Buildings;
using Gameplay.Characters;
using Gameplay.Locations;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [Inject] private GameplaySettingsInstaller.GameplayResources _gameplayResources;

    public override void InstallBindings()
    {
        CoreInstaller.Install(Container);

        Container
            .BindFactory<CharacterView, CharacterView.Factory>()
            .FromComponentInNewPrefab(_gameplayResources.CharacterPrefab)
            .WhenInjectedInto<Character>();

        Container
            .BindMemoryPool<Character, Character.Pool>()
            .WithInitialSize(1);

        Container.Bind<LoadLocations>().ToSelf().AsSingle();
        
        Container.BindInterfacesTo<LocationModel>().AsSingle();

        Container.BindFactory<BuildingView, BuildingView.Factory>()
            .FromComponentInNewPrefab(_gameplayResources.BuildingPrefab)
            .WhenInjectedInto<Building>();
        
        Container.BindMemoryPool<Building, Building.Pool>()
            .WithInitialSize(3);
    }
}