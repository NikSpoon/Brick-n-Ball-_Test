using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        foreach (var (input, moveData, velocity, mass, transform, entity)
                 in SystemAPI.Query<
                        RefRO<PlayerEcsInputData>,
                        RefRO<PlayerMovementData>,
                        RefRW<PhysicsVelocity>,
                        RefRW<PhysicsMass>,
                        RefRO<LocalTransform>>()
                     .WithAll<Simulate>()
                     .WithEntityAccess())
        {
            float2 move = math.normalizesafe(input.ValueRO.Move);

            float3 forward = math.mul(transform.ValueRO.Rotation, new float3(0, 0, 1));
            float3 right = math.mul(transform.ValueRO.Rotation, new float3(1, 0, 0));
            float3 moveDir = (forward * move.y + right * move.x) * moveData.ValueRO.MoveSpeed;

            velocity.ValueRW.Linear.x = moveDir.x;
            velocity.ValueRW.Linear.z = moveDir.z;

            bool grounded = IsGrounded(transform.ValueRO.Position, entity, ref state);

            if (input.ValueRO.Jump && grounded)
            {
                velocity.ValueRW.Linear.y = moveData.ValueRO.JumpForce;
            }
        }
    }
    private bool IsGrounded(float3 position, Entity self, ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        RaycastInput ray = new RaycastInput
        {
            Start = new float3(0,-1,0),
            End = new float3(0, -3, 0),
            Filter = CollisionFilter.Default
        };

        if (!physicsWorld.CastRay(ray, out var hit))
        {
            UnityEngine.Debug.Log("IsGrounded: NO HIT");
            return false;
        }

        Entity hitEntity = physicsWorld.Bodies[hit.RigidBodyIndex].Entity;

        bool hitIsGround = state.EntityManager.HasComponent<GroundTag>(hitEntity);
    
        if (hitIsGround)
            return true;

        return false;
    }
}

