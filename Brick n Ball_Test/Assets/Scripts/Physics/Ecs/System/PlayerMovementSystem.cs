using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
   [BurstCompile]
    public void OnUpdate(ref SystemState state)

    {
        var dt = SystemAPI.Time.DeltaTime;
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        foreach (var (input, moveData, velocity, transform, playerData, entity)
          in SystemAPI.Query<
                 RefRO<PlayerEcsInputData>,
                 RefRO<PlayerMovementData>,
                 RefRW<PhysicsVelocity>,
                 RefRW<LocalTransform>,
                 RefRO<PlayerData>>()
              .WithAll<Simulate>()
              .WithEntityAccess())
        {
            ApplyMovementPhysics(
                ref velocity.ValueRW,
                ref transform.ValueRW,
                in moveData.ValueRO,
                in input.ValueRO,
                dt);

            bool grounded = IsGrounded(transform.ValueRO.Position, playerData.ValueRO.GraundRoot, moveData.ValueRO.JumpDistance);
            if (input.ValueRO.Jump && grounded)
            {
                velocity.ValueRW.Linear.y = moveData.ValueRO.JumpForce;
            }

            quaternion lookRot = GetRotationFromCamera(transform.ValueRO.Position);
            if (!lookRot.Equals(quaternion.identity))
            {
                transform.ValueRW.Rotation = lookRot;
            }
        }
    }

    private static void ApplyMovementPhysics(
    ref PhysicsVelocity velocity,
    ref LocalTransform transform,
    in PlayerMovementData moveData,
    in PlayerEcsInputData input,
    float dt)
    {
        // Сырой инпут
        float2 rawInput = input.Move;

        // Для дебага можно раскомментить
        // UnityEngine.Debug.Log($"[Move] RAW input={rawInput}, pos={transform.Position}, velBefore={velocity.Linear}");

        // Нет инпута — гасим горизонтальную скорость, позицию не трогаем
        if (math.lengthsq(rawInput) < 1e-5f)
        {
            float3 v = velocity.Linear;
            v.x = 0;
            v.z = 0;
            velocity.Linear = v;
            return;
        }

        float2 move = math.normalizesafe(rawInput);

        // Направления вперёд/вправо от текущего поворота
        float3 forward = math.mul(transform.Rotation, new float3(0, 0, 1));
        float3 right = math.mul(transform.Rotation, new float3(1, 0, 0));

        // Направление движения в мире
        float3 moveDir = (forward * move.y + right * move.x) * moveData.MoveSpeed;

        // 1) ДВИГАЕМ ПОЗИЦИЮ (как в рабочем варианте без физики)
        float3 oldPos = transform.Position;
        float3 newPos = oldPos + moveDir * dt;
        transform.Position = newPos;

        // 2) ВЫЧИСЛЯЕМ СКОРОСТЬ ДЛЯ ФИЗИКИ ИЗ СМЕЩЕНИЯ
        float3 vNew = velocity.Linear;

        vNew.x = (newPos.x - oldPos.x) / dt;
        vNew.z = (newPos.z - oldPos.z) / dt;
        // Y не трогаем — её меняет гравитация / прыжок

        velocity.Linear = vNew;

        // Для дебага можно раскомментить
        // UnityEngine.Debug.Log($"[Move] AFTER pos={transform.Position}, vel={velocity.Linear}");
    }

    private bool IsGrounded(float3 position, float3 offset, float distance)
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        float3 start = position + offset;

        var ray = new RaycastInput
        {
            Start = start,
            End = start + new float3(0, -distance, 0),
            Filter = CollisionFilter.Default
        };

        if (physicsWorld.CastRay(ray, out RaycastHit hit))
        {
            return hit.Fraction < 1f;
        }

        return false;
    }

    private quaternion GetRotationFromCamera(float3 playerPosition)
    {
        if (!SystemAPI.TryGetSingleton<CameraData>(out var camData))
            return quaternion.identity;

        float3 camPos = camData.Position;

        float3 dir = playerPosition - camPos;
        dir.y = 0;
        dir = math.normalizesafe(dir);

        if (math.lengthsq(dir) < 1e-5f)
            return quaternion.identity;

        return quaternion.LookRotationSafe(dir, math.up());
    }
}
