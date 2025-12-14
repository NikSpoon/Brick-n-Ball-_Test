using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

[BurstCompile]
public partial struct BulletHitConsumeSystem : ISystem
{
    private ComponentLookup<BulletTag> _bulletLookup;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        _bulletLookup = state.GetComponentLookup<BulletTag>(false);
    }

    public void OnUpdate(ref SystemState state)
    {
        if (Context.Instance.AppSystem.CurrentState != AppState.Game)
            return;

        var sim = SystemAPI.GetSingleton<SimulationSingleton>();

        _bulletLookup.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var job = new ConsumeHitsJob
        {
            BulletLookup = _bulletLookup,
            ECB = ecb.AsParallelWriter()
        };

        state.Dependency = job.Schedule(sim, state.Dependency);
    }

    [BurstCompile]
    private struct ConsumeHitsJob : ICollisionEventsJob
    {
        [NativeDisableParallelForRestriction]
        public ComponentLookup<BulletTag> BulletLookup;

        public EntityCommandBuffer.ParallelWriter ECB;

        public void Execute(CollisionEvent ev)
        {
            var a = ev.EntityA;
            var b = ev.EntityB;

            if (BulletLookup.HasComponent(a)) ConsumeOneHit(a);
            if (BulletLookup.HasComponent(b)) ConsumeOneHit(b);
        }

        private void ConsumeOneHit(Entity bullet)
        {
            var data = BulletLookup[bullet];
            data.MaxCollValue--;

            if (data.MaxCollValue <= 0)
                ECB.DestroyEntity(0, bullet);
            else
                BulletLookup[bullet] = data;
        }
    }
}


