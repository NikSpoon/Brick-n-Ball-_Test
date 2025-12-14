using Unity.Entities;

public partial struct InitGameFromProfileSystem : ISystem
{
    private AppState _prev;
    private double _nextLogTime;

    public void OnCreate(ref SystemState state)
    {
        _prev = AppState.MainMenu;
    }

    public void OnUpdate(ref SystemState state)
    {
        var ctx = Context.Instance;
        if (ctx == null) return;

        var cur = ctx.AppSystem.CurrentState;

        bool hasProf = SystemAPI.HasSingleton<PlayerProfData>();
        bool hasSess = SystemAPI.HasSingleton<SessionDataEsc>();

        // раз в ~0.5 сек, чтобы не спамить
        if (SystemAPI.Time.ElapsedTime >= _nextLogTime)
        {
            _nextLogTime = SystemAPI.Time.ElapsedTime + 0.5;
            UnityEngine.Debug.Log($"[ECS Ready] state={cur} hasProf={hasProf} hasSess={hasSess}");
        }

        if (cur == AppState.Game && _prev != AppState.Game)
        {
            if (!SystemAPI.HasSingleton<PlayerProfData>() ||
                !SystemAPI.HasSingleton<SessionDataEsc>())
            {
                return;
            }

            ctx.FinishRunData.ClearFinishData();

            var prof = SystemAPI.GetSingleton<PlayerProfData>();
            prof.Levl = ctx.PlayerProf.Levl;
            SystemAPI.SetSingleton(prof);

            var session = SystemAPI.GetSingleton<SessionDataEsc>();
            session.PlayerScore = 0;
            session.BrickKillProgress = 0;
            SystemAPI.SetSingleton(session);

            _prev = cur;
            return;
        }

        _prev = cur;
    }

}
