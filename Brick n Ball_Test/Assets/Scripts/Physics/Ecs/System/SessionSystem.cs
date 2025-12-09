using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
partial struct SessionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerProfData>();
        state.RequireForUpdate<SessionDataEsc>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (Context.Instance.AppSystem.CurrentState != AppState.Game)
            return;

        var playerProfData = SystemAPI.GetSingleton<PlayerProfData>();
        var sessionData = SystemAPI.GetSingleton<SessionDataEsc>();

        int scoreDelta = 0;
        int levelDelta = 0;

        foreach (var addScore in SystemAPI.Query<RefRO<AddScoreTag>>())
            scoreDelta += addScore.ValueRO.Value;

        foreach (var addLevel in SystemAPI.Query<RefRO<AddLevelTag>>())
            levelDelta += addLevel.ValueRO.Value;

        if (scoreDelta != 0)
            sessionData.PlayerScore += scoreDelta;

        if (levelDelta != 0)
            playerProfData.Levl += levelDelta;

        // записываем обратно
        SystemAPI.SetSingleton(playerProfData);
        SystemAPI.SetSingleton(sessionData);

        // чистим тэги
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
