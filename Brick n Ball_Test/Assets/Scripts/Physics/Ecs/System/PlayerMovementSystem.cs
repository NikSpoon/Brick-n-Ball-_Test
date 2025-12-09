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
        foreach (var (input, moveData, velocity, mass, transform, playerData, entity)
                 in SystemAPI.Query<
                        RefRO<PlayerEcsInputData>,
                        RefRO<PlayerMovementData>,
                        RefRW<PhysicsVelocity>,
                        RefRW<PhysicsMass>,
                        RefRW<LocalTransform>,
                        RefRO<PlayerData>>()
                     .WithAll<Simulate>()
                     .WithEntityAccess())
        {

            ApplyMovement(ref velocity.ValueRW, in transform.ValueRO, in moveData.ValueRO, in input.ValueRO);


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
  
    private void ApplyMovement(ref PhysicsVelocity velocity, in LocalTransform transform, in PlayerMovementData moveData, in PlayerEcsInputData input)
    {
        float2 move = math.normalizesafe(input.Move);

        float3 forward = math.mul(transform.Rotation, new float3(0, 0, 1));
        float3 right = math.mul(transform.Rotation, new float3(1, 0, 0));
        float3 moveDir = (forward * move.y + right * move.x) * moveData.MoveSpeed;

        velocity.Linear.x = moveDir.x;
        velocity.Linear.z = moveDir.z;
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
