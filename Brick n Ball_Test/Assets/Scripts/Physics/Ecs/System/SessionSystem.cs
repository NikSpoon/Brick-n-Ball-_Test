using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct SessionSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerProfComp = SystemAPI.ManagedAPI.GetSingleton<PlayerProfComponent>();
        var sessionComp = SystemAPI.ManagedAPI.GetSingleton<SessionDataComponent>();

        var playerProf = playerProfComp.PlayerProfaile;
        var session = sessionComp.SessionData;

        int scoreDelta = 0;
        int levelDelta = 0;

        foreach (var addScore in SystemAPI.Query<RefRO<AddScoreTag>>())
        {
            scoreDelta += addScore.ValueRO.Value;  
        }

        foreach (var addLevel in SystemAPI.Query<RefRO<AddLevelTag>>())
        {
            levelDelta += addLevel.ValueRO.Value;
        }

        if (scoreDelta != 0)
            session.AddScore(scoreDelta);

        if (levelDelta != 0)
            playerProf.AddLevl(levelDelta);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (tag, entity) in SystemAPI
                     .Query<RefRO<AddScoreTag>>()
                     .WithEntityAccess())
        {
            ecb.RemoveComponent<AddScoreTag>(entity);
        }

        foreach (var (tag, entity) in SystemAPI
                     .Query<RefRO<AddLevelTag>>()
                     .WithEntityAccess())
        {
            ecb.RemoveComponent<AddLevelTag>(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

}
