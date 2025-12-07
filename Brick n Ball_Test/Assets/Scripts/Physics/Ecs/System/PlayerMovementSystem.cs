using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine.UIElements;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        foreach (var (input, moveData, velocity, mass, transform, playerData, entity)
                 in SystemAPI.Query<
                        RefRO<PlayerEcsInputData>,
                        RefRO<PlayerMovementData>,
                        RefRW<PhysicsVelocity>,
                        RefRW<PhysicsMass>,
                        RefRO<LocalTransform>,
                        RefRO<PlayerData>>()
                     .WithAll<Simulate>()
                     .WithEntityAccess())
        {
            float2 move = math.normalizesafe(input.ValueRO.Move);

            float3 forward = math.mul(transform.ValueRO.Rotation, new float3(0, 0, 1));
            float3 right = math.mul(transform.ValueRO.Rotation, new float3(1, 0, 0));
            float3 moveDir = (forward * move.y + right * move.x) * moveData.ValueRO.MoveSpeed;

            velocity.ValueRW.Linear.x = moveDir.x;
            velocity.ValueRW.Linear.z = moveDir.z;


            bool grounded = IsGrounded(transform.ValueRO.Position, playerData.ValueRO.GraundRoot, moveData.ValueRO.JumpDistance);

            if (input.ValueRO.Jump && grounded)
            {
                velocity.ValueRW.Linear.y = moveData.ValueRO.JumpForce;
            }


            float3 start = transform.ValueRO.Position + playerData.ValueRO.GraundRoot;
            float3 end = start + new float3(0, -moveData.ValueRO.JumpDistance, 0);

            UnityEngine.Debug.DrawLine(start, end, UnityEngine.Color.red);
        }
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