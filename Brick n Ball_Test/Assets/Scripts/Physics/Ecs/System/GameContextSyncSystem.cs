using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct GameContextSyncSystem : ISystem
{
    private bool _initializedForGame;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerProfData>();
        state.RequireForUpdate<SessionDataEsc>();
        _initializedForGame = false;
    }

    public void OnUpdate(ref SystemState state)
    {
        var ctx = Context.Instance;
        if (ctx == null) return;

        bool inGame = ctx.AppSystem.CurrentState == AppState.Game;

        if (inGame && !_initializedForGame)
        {
            var profData = SystemAPI.GetSingleton<PlayerProfData>();
            profData.Levl = ctx.PlayerProf.Levl;
            SystemAPI.SetSingleton(profData);

            var sessionData = SystemAPI.GetSingleton<SessionDataEsc>();
            sessionData.PlayerScore = 0;
            sessionData.BrickKillProgress = 0;
            SystemAPI.SetSingleton(sessionData);

            _initializedForGame = true;
        }

        if (!inGame)
        {
            _initializedForGame = false;
            return;
        }

        var prof = SystemAPI.GetSingleton<PlayerProfData>();
        var session = SystemAPI.GetSingleton<SessionDataEsc>();

        SyncPlayerProfRaw(ctx.PlayerProf, in prof);
        SyncSessionRaw(ctx.SessionData, in session);
    }

    private static void SyncPlayerProfRaw(PlayerProf prof, in PlayerProfData data)
    {
        if (prof.Name == null)
            prof.InitProf("Player");

        if (prof.Levl != data.Levl)
            prof.ForceSetLevel(data.Levl);
    }

    private static void SyncSessionRaw(SessionData session, in SessionDataEsc data)
    {
        if (session.PlayerScore != data.PlayerScore)
            session.ForceSetScore(data.PlayerScore);
    }
}
