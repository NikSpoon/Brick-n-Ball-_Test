using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CameraToEcsBridge : MonoBehaviour
{
    private EntityManager _em;
    private Entity _cameraEntity;

    void Awake()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;

        
        var archetype = _em.CreateArchetype(typeof(CameraData));
        _cameraEntity = _em.CreateEntity(archetype);
    }

    void LateUpdate()
    {
        if (!_em.Exists(_cameraEntity)) return;

        float3 pos = transform.position;
        _em.SetComponentData(_cameraEntity, new CameraData
        {
            Position = pos
        });
    }
}