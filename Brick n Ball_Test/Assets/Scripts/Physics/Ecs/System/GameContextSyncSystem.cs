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

        ctx.PlayerProf.SyncFromData(profData);
        ctx.SessionData.SyncFromData(sessionData);
    }

}

    public static class PlayerProfExtensions
    {
        public static void SyncFromData(this PlayerProf prof, in PlayerProfData data)
        {

            prof.InitProf(prof.Name ?? "Player"); 
            prof.AddLevl();
        }
    }

    public static class SessionDataExtensions
    {
        public static void SyncFromData(this SessionData session, in SessionDataEsc data)
        {
            session.AddScore(); 
        }
    }