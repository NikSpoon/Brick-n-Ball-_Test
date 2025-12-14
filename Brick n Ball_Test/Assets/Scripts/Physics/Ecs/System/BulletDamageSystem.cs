using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

public partial struct BulletDamageSystem : ISystem
{
    private ComponentLookup<BulletTag> _bulletLookup;
    private ComponentLookup<BrickTag> _brickLookup;
    private ComponentLookup<LastWallTeg> _lastWallLookup;
    private ComponentLookup<BrickHealth> _brickHealthLookup;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();

        _bulletLookup = state.GetComponentLookup<BulletTag>(true);
        _brickLookup = state.GetComponentLookup<BrickTag>(true);
        _lastWallLookup = state.GetComponentLookup<LastWallTeg>(true);
        _brickHealthLookup = state.GetComponentLookup<BrickHealth>(false);
    }

    public void OnUpdate(ref SystemState state)
    {
        if (Context.Instance.AppSystem.CurrentState != AppState.Game)
            return;

        var sim = SystemAPI.GetSingleton<SimulationSingleton>();

        _bulletLookup.Update(ref state);
        _brickLookup.Update(ref state);
        _lastWallLookup.Update(ref state);
        _brickHealthLookup.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var job = new BulletCollisionJob
        {
            BulletLookup = _bulletLookup,
            BrickLookup = _brickLookup,
            LastWallLookup = _lastWallLookup,
            BrickHealthLookup = _brickHealthLookup,
            ECB = ecb.AsParallelWriter()
        };

        state.Dependency = job.Schedule(sim, state.Dependency);
    }


    [BurstCompile]
    struct BulletCollisionJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<BulletTag> BulletLookup;
        [ReadOnly] public ComponentLookup<BrickTag> BrickLookup;
        [ReadOnly] public ComponentLookup<LastWallTeg> LastWallLookup;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<BrickHealth> BrickHealthLookup;

        public EntityCommandBuffer.ParallelWriter ECB;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity a = collisionEvent.EntityA;
            Entity b = collisionEvent.EntityB;

            bool aIsBullet = BulletLookup.HasComponent(a);
            bool bIsBullet = BulletLookup.HasComponent(b);
            bool aIsBrick = BrickLookup.HasComponent(a);
            bool bIsBrick = BrickLookup.HasComponent(b);
            bool aIsWall = LastWallLookup.HasComponent(a);
            bool bIsWall = LastWallLookup.HasComponent(b);

            if (aIsBullet && !aIsBrick && (bIsBrick || bIsWall))
            {
                HandleHit(b, a);
            }
            else if (bIsBullet && !bIsBrick && (aIsBrick || aIsWall))
            {
                HandleHit(a, b);
            }
        }

        private void HandleHit(Entity target, Entity bullet)
        {
            if (BrickHealthLookup.HasComponent(target))
            {
                var health = BrickHealthLookup[target];
                health.Value--;

                AddScore(1);        
                
                if (health.Value <= 0)
                {
                    ECB.DestroyEntity(0, target);
                }
                else
                {
                    BrickHealthLookup[target] = health;
                }

            }
            else
            {
                ECB.DestroyEntity(0, bullet);
            }
        }

        // ====== Add SCORE / LEVEL ======

        private void AddScore(int amount)
        {
            var e = ECB.CreateEntity(0);
            ECB.AddComponent(0, e, new AddScoreTag { Value = amount });
        }

        private void AddLevel(int amount)
        {
            //For other Vip brick(for example)
            var e = ECB.CreateEntity(0);
            ECB.AddComponent(0, e, new AddLevelTag { Value = amount });
        }
    }
}
