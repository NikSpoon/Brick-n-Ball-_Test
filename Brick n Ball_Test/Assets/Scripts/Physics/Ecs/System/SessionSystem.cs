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
        {
            sessionData.PlayerScore += scoreDelta;

            ApplyBrickLeveling(ref playerProfData, ref sessionData, scoreDelta, ref levelDelta);
        }

        if (levelDelta != 0)
            playerProfData.Levl += levelDelta;

        SystemAPI.SetSingleton(playerProfData);
        SystemAPI.SetSingleton(sessionData);
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


    /// Updates brick kill progress and awards level-ups.
    /// For every (currentLevel * 2) destroyed bricks → player gains +1 level.
    /// bricksGained — number of destroyed bricks gained this frame (scoreDelta).
    /// levelDelta is passed by ref so we can stack level gains together with AddLevelTag.
    /// Returns how many bricks are required to gain the next level
    
    /// from the given current level.
    /// Level 0 → need 1 brick,
    /// Level 1 → need 3 bricks,
    /// Level 2 → need 7 bricks,
    /// Level 3 → need 13 bricks, etc.
    /// Formula: 1 + level * (level + 1)

    private static void ApplyBrickLeveling(
     ref PlayerProfData playerProf,
     ref SessionDataEsc session,
     int bricksGained,
     ref int levelDelta)
    {
        if (bricksGained <= 0)
            return;

       
        session.BrickKillProgress += bricksGained;

        int currentLevel = playerProf.Levl;

       
        while (true)
        {
            int bricksNeeded = BricksNeededForNextLevel(currentLevel);

            
            if (session.BrickKillProgress < bricksNeeded)
                break;

          
            session.BrickKillProgress -= bricksNeeded;
            currentLevel++;
            levelDelta++;
        }

        playerProf.Levl = currentLevel;
    }

   
    private static int BricksNeededForNextLevel(int currentLevel)
    {
        if (currentLevel <= 0)
            return 1;

        return 1 + currentLevel * (currentLevel + 1);
    }
}
