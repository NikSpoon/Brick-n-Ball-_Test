using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
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
            ApplyMovementPhysicsXZ(ref velocity.ValueRW, in transform.ValueRO, in moveData.ValueRO, in input.ValueRO);

           
            bool grounded = IsGrounded(transform.ValueRO.Position, playerData.ValueRO.GraundRoot, moveData.ValueRO.JumpDistance);
            if (input.ValueRO.Jump && grounded)
            {
                velocity.ValueRW.Linear.y = moveData.ValueRO.JumpForce;
            }

        }
    }

    private static void ApplyMovementPhysicsXZ(
      ref PhysicsVelocity velocity,
      in LocalTransform transform,
      in PlayerMovementData moveData,
      in PlayerEcsInputData input)
    {
        float2 raw = input.Move;

        float3 forward = math.mul(transform.Rotation, new float3(0, 0, 1));
        float3 right = math.mul(transform.Rotation, new float3(1, 0, 0));

        float3 wishDir = forward * raw.y + right * raw.x;
        wishDir.y = 0f;

        float3 v = velocity.Linear;

        if (math.lengthsq(wishDir) < 1e-6f)
        {
            v.x = 0f;
            v.z = 0f;
        }
        else
        {
            wishDir = math.normalizesafe(wishDir);
            v.x = wishDir.x * moveData.MoveSpeed;
            v.z = wishDir.z * moveData.MoveSpeed;
        }

        velocity.Linear = v;
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

}
