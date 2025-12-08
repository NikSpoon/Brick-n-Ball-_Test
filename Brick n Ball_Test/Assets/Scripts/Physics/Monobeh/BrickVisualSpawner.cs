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

        foreach (var entiti in entities)
        {
            var transform = _em.GetComponentData<LocalTransform>(entiti);

            Vector3 pos = transform.Position;
            quaternion rot = transform.Rotation;
            Quaternion q = new Quaternion(rot.value.x, rot.value.y, rot.value.z, rot.value.w);

            var gameOb = Instantiate(_visualPrefab, pos, q, _root);

            _em.AddComponent<BrickVisualTag>(entiti);

            _em.AddComponentObject(entiti, gameOb);
        }
    }

    private void SyncVisualsWithEcs()
    {
        using var entities = _em.CreateEntityQuery(
            ComponentType.ReadOnly<BrickTag>(),
            ComponentType.ReadOnly<BrickVisualTag>(),
            ComponentType.ReadOnly<LocalTransform>(),
            ComponentType.ReadOnly<GameObject>()  
        ).ToEntityArray(Allocator.Temp);

        foreach (var entiti in entities)
        {
            var transform = _em.GetComponentData<LocalTransform>(entiti);
            var gameOb = _em.GetComponentObject<GameObject>(entiti);

            Vector3 pos = transform.Position;
            quaternion rot = transform.Rotation;
            Quaternion q = new Quaternion(rot.value.x, rot.value.y, rot.value.z, rot.value.w);

            gameOb.transform.position = pos;
            gameOb.transform.rotation = q;
        }
    }
}
