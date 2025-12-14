using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLookInputMono : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private EntityManager _em;
    private EntityQuery _playerQuery;

    void Awake()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;

        _playerQuery = _em.CreateEntityQuery(
            ComponentType.ReadWrite<PlayerLookInput>(),
            ComponentType.ReadOnly<PlayerData>()
        );
    }

    void Update()
    {
        if (_playerQuery.IsEmpty) return;
        if (Mouse.current == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();

        Ray ray = _camera.ScreenPointToRay(screenPos);

        using var entities = _playerQuery.ToEntityArray(Allocator.Temp);

        foreach (var entity in entities)
        {
            float3 playerPos = _em.GetComponentData<LocalTransform>(entity).Position;

            Plane plane = new Plane(Vector3.up, new Vector3(0f, playerPos.y, 0f));

            if (!plane.Raycast(ray, out float enter)) continue;

            Vector3 hitPoint = ray.GetPoint(enter);

            float3 dir = (float3)hitPoint - playerPos;
            dir.y = 0f;

            if (math.lengthsq(dir) < 1e-6f) continue;

            _em.SetComponentData(entity, new PlayerLookInput
            {
                LookDirection = math.normalizesafe(dir),
                Speed = 15f 
            });
        }
    }
}
