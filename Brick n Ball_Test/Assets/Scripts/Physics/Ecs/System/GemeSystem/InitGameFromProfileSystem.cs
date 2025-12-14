using Unity.Entities;

public partial struct InitGameFromProfileSystem : ISystem
{
    private AppState _prev;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerProfData>();
        state.RequireForUpdate<SessionDataEsc>();
        _prev = AppState.MainMenu;
    }

    public void OnUpdate(ref SystemState state)
    {
        var ctx = Context.Instance;
        if (ctx == null) return;

        var cur = ctx.AppSystem.CurrentState;

        if (cur == AppState.Game && _prev != AppState.Game)
        {
            ctx.FinishRunData.ClearFinishData();
            var prof = SystemAPI.GetSingleton<PlayerProfData>();
            prof.Levl = ctx.PlayerProf.Levl;
            SystemAPI.SetSingleton(prof);

            var session = SystemAPI.GetSingleton<SessionDataEsc>();
            session.PlayerScore = 0;
            session.BrickKillProgress = 0;
            SystemAPI.SetSingleton(session);


        }

        _prev = cur;
    }
}
