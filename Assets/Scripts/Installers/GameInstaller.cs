using UnityEngine;
using PathCreation;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private Player _player;
    [SerializeField] private PathCreator _pathCreator;

    public override void InstallBindings()
    {
        Container.Bind<Player>().FromInstance(_player).AsSingle();
        Container.Bind<PathCreator>().FromInstance(_pathCreator).AsSingle();
    }
}