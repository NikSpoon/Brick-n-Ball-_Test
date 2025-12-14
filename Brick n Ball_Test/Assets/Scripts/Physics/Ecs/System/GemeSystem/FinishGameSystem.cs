using Unity.Entities;

public partial struct FinishGameSystem : ISystem
{
    private AppState _prev;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerProfData>();
        state.RequireForUpdate<SessionDataEsc>(); // ✅ нужно
        _prev = AppState.Finish;
    }

    public void OnUpdate(ref SystemState state)
    {
        var ctx = Context.Instance;
        if (ctx == null) return;

        var cur = ctx.AppSystem.CurrentState;

        if (_prev == AppState.Game && cur != AppState.Game)
        {
            var profData = SystemAPI.GetSingleton<PlayerProfData>();
            var sessionData = SystemAPI.GetSingleton<SessionDataEsc>();

            ctx.PlayerProf.ForceSetLevel(profData.Levl);

            var reason = FinishReason.None;
            if (SystemAPI.HasSingleton<FinishData>())
                reason = SystemAPI.GetSingleton<FinishData>().Reason;

            ctx.FinishRunData.SetReason(reason);

            ctx.FinishRunData.AddScore(sessionData.PlayerScore);

            int gained = profData.Levl - ctx.FinishRunData.StartLevl;
            if (gained > 0)
                ctx.FinishRunData.AddLevl(gained);

            ctx.FinishRunData.FinishGame();
        }

        _prev = cur;
    }
}
