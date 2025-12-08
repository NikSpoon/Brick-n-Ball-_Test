using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

public class EcsPlayerTargetSync : MonoBehaviour
{
    private EntityManager _entityManager;
    private EntityQuery _playerQuery;
    private Entity _playerEntity;
    private bool _hasPlayer;

    private void Awake()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null)
        {
            Debug.LogWarning("No DefaultGameObjectInjectionWorld yet. ECS World not ready.");
            enabled = false;
            return;
        }

        _entityManager = world.EntityManager;

        _playerQuery = _entityManager.CreateEntityQuery(
            ComponentType.ReadOnly<PlayerData>(),
            ComponentType.ReadOnly<LocalTransform>()
        );
    }

    private void Update()
    {
        if (_entityManager == null)
            return;

        if (!_hasPlayer || !_entityManager.Exists(_playerEntity))
        {
            using (NativeArray<Entity> entities = _playerQuery.ToEntityArray(Allocator.Temp))
            {
                if (entities.Length == 0)
                    return; 

                _playerEntity = entities[0];
                _hasPlayer = true;
            }
        }

        if (!_entityManager.HasComponent<LocalTransform>(_playerEntity))
            return;

        LocalTransform lt = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

        transform.position = lt.Position;
        transform.rotation = lt.Rotation;
    }
}
