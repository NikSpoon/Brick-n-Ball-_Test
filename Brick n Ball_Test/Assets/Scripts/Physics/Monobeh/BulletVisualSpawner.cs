using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BulletVisualSpawner : MonoBehaviour
{
    private EntityManager _em;

    [SerializeField] private Transform _root;
    [SerializeField] private GameObject _bullet;

    private readonly Dictionary<Entity, GameObject> _bulletVisuals = new();

    void Awake()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    void Update()
    {
        SpawnVisualsForNewBullets();
        SyncVisualsWithEcs();
    }

    private void SpawnVisualsForNewBullets()
    {
        using var entities = _em.CreateEntityQuery(
            ComponentType.ReadOnly<NewBullet>(),
            ComponentType.ReadOnly<LocalTransform>(),
            ComponentType.ReadOnly<GameObject>()
        ).ToEntityArray(Allocator.Temp);

        foreach (var entity in entities)
        {
            var prefabGO = _em.GetComponentObject<GameObject>(entity);
            var transform = _em.GetComponentData<LocalTransform>(entity);

            Vector3 pos = transform.Position;
            quaternion rot = transform.Rotation;
            Quaternion q = new Quaternion(rot.value.x, rot.value.y, rot.value.z, rot.value.w);

            var gameOb = Instantiate(_bullet, pos, q, _root);

            _em.RemoveComponent<GameObject>(entity);
            _em.AddComponentObject(entity, gameOb);
            
            _bulletVisuals[entity] = gameOb;
            _em.RemoveComponent<NewBullet>(entity);
        }
    }

    private void SyncVisualsWithEcs()
    {
        var toRemove = new List<Entity>();

        foreach (var kvp in _bulletVisuals)
        {
            Entity entity = kvp.Key;
            GameObject go = kvp.Value;

            if (!_em.Exists(entity))
            {
                if (go != null)
                    Destroy(go);

                toRemove.Add(entity);
                continue;
            }

            var transform = _em.GetComponentData<LocalTransform>(entity);

            Vector3 pos = transform.Position;
            quaternion rot = transform.Rotation;
            Quaternion q = new Quaternion(rot.value.x, rot.value.y, rot.value.z, rot.value.w);

            if (go != null)
            {
                go.transform.position = pos;
                go.transform.rotation = q;
            }
        }

        foreach (var entity in toRemove)
        {
            _bulletVisuals.Remove(entity);
        }
    }
}
