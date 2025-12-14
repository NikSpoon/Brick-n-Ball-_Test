using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct GameContextSyncSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerProfData>();
        state.RequireForUpdate<SessionDataEsc>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ctx = Context.Instance;
        if (ctx == null)
            return;

        var profData = SystemAPI.GetSingleton<PlayerProfData>();    
        var sessionData = SystemAPI.GetSingleton<SessionDataEsc>();

        SyncPlayerProfRaw(ctx.PlayerProf, in profData);
        SyncSessionRaw(ctx.SessionData, in sessionData);
    }

    private static void SyncPlayerProfRaw(PlayerProf prof, in PlayerProfData data)
    {
       

        if (prof.Name == null)
            prof.InitProf("Player");  // for som...

        if (prof.Levl != data.Levl)
        {
            prof.ForceSetLevel(data.Levl);
        }
    }

    private static void SyncSessionRaw(SessionData session, in SessionDataEsc data)
    {
        if (session.PlayerScore != data.PlayerScore)
        {
            session.ForceSetScore(data.PlayerScore);
        }
    }
}