using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    private EntityManager _em;

    [SerializeField] private Transform _root;
    [SerializeField] private GameObject _bullet;

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

        foreach (var entiti in entities)
        {
            var prefabGO = _em.GetComponentObject<GameObject>(entiti);
            var transform = _em.GetComponentData<LocalTransform>(entiti);

            Vector3 pos = transform.Position;
            quaternion rot = transform.Rotation;
            Quaternion q = new Quaternion(rot.value.x, rot.value.y, rot.value.z, rot.value.w);

            var gameOb = Instantiate(_bullet, pos, q, _root);

            _em.RemoveComponent<GameObject>(entiti);
            _em.AddComponentObject(entiti, gameOb);

            _em.RemoveComponent<NewBullet>(entiti);
        }
    }

    private void SyncVisualsWithEcs()
    {
        using var entities = _em.CreateEntityQuery(
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
