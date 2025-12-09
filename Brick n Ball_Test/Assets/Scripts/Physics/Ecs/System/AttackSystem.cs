
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct AttackSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (input, transform, playerData, attackDataRW, entity)
                 in SystemAPI.Query<
                        RefRO<PlayerEcsInputData>,
                        RefRO<LocalTransform>,
                        RefRW<PlayerData>,
                        RefRW<AttackData>>()
                     .WithAll<Simulate>()
                     .WithEntityAccess())
        {
            ref var attackData = ref attackDataRW.ValueRW;



            if (attackData.CurrentColdaun > 0f)
                attackData.CurrentColdaun -= dt;

            if (!input.ValueRO.Fire || attackData.CurrentColdaun > 0f)
                continue;

            if (playerData.ValueRO.BollValue == 0)
            {
                // or crtatr some for add shots
                return;
            }
            

            float3 localGunOffset = playerData.ValueRO.GunPointerRoot;
            float3 worldGunOffset = math.mul(transform.ValueRO.Rotation, localGunOffset);
            float3 gunPos = transform.ValueRO.Position + worldGunOffset;

            float3 forward = math.mul(transform.ValueRO.Rotation, new float3(0, 0, 1));

            Entity shell = ecb.Instantiate(attackData.ShellPrefab);
            playerData.ValueRW.BollValue -= 1;

            ecb.AddComponent(shell, new NewBullet { });
            ecb.SetComponent(shell, new LocalTransform
            {
                Position = gunPos,
                Rotation = transform.ValueRO.Rotation,
                Scale = 1f
            });

            float shellSpeed = 20f;
            ecb.SetComponent(shell, new PhysicsVelocity
            {
                Linear = forward * shellSpeed,
                Angular = float3.zero
            });

            attackData.CurrentColdaun = attackData.Coldaun;
        }
    }
}