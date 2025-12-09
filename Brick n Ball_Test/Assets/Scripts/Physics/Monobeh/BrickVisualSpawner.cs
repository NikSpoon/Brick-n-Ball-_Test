using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BrickVisualSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _visualPrefab;   
    [SerializeField] private Transform _root;          

    private EntityManager _em;
    
    private readonly Dictionary<Entity, GameObject> _brickVisuals = new();
    private void Awake()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update()
    {
        SpawnVisualsForNewBricks();
        SyncVisualsWithEcs();
    }

    private void SpawnVisualsForNewBricks()
    {
        using var entities = _em.CreateEntityQuery(
            ComponentType.ReadOnly<BrickTag>(),
            ComponentType.ReadOnly<LocalTransform>(),
            ComponentType.Exclude<BrickVisualTag>()   
        ).ToEntityArray(Allocator.Temp);

        foreach (var entity in entities)
        {
            var transform = _em.GetComponentData<LocalTransform>(entity);

            Vector3 pos = transform.Position;
            quaternion rot = transform.Rotation;
            Quaternion q = new Quaternion(rot.value.x, rot.value.y, rot.value.z, rot.value.w);

            var gameOb = Instantiate(_visualPrefab, pos, q, _root);

            _em.AddComponent<BrickVisualTag>(entity);

            _em.AddComponentObject(entity, gameOb);

            _brickVisuals[entity] = gameOb;

        }
    }

    private void SyncVisualsWithEcs()
    {
        var toRemove = new List<Entity>();

        foreach (var kvp in _brickVisuals)
        {
            var entity = kvp.Key;
            var go = kvp.Value;

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
            _brickVisuals.Remove(entity);
        }
    }
}
