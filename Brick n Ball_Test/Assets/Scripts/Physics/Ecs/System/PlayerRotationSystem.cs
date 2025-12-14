
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[BurstCompile]
public partial struct PlayerRotationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (look, transform) in
                 SystemAPI.Query<
                     RefRO<PlayerLookInput>,
                     RefRW<LocalTransform>>())
        {
            float3 dir = look.ValueRO.LookDirection;


            if (math.lengthsq(dir) < 0.0001f)
                continue;

            quaternion target = quaternion.LookRotationSafe(math.normalizesafe(dir), math.up());

            float speed = math.max(0.01f, look.ValueRO.Speed);
            float t = 1f - math.exp(-speed * dt);

           transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, target, t);
        }
    }
}
