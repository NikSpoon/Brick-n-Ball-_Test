using Unity.Entities;

public enum FinishReason : byte { None, Win, Lose }

public partial struct GameOverSystem : ISystem
{
    private EntityQuery _bulletsQuery;
    private EntityQuery _bricksQuery;
    private EntityQuery _playerQuery;

    private bool _levelStarted; 

    public void OnCreate(ref SystemState state)
    {
        _bulletsQuery = state.GetEntityQuery(ComponentType.ReadOnly<BulletTag>());
        _bricksQuery = state.GetEntityQuery(ComponentType.ReadOnly<BrickTag>());
        _playerQuery = state.GetEntityQuery(ComponentType.ReadOnly<PlayerData>());

        state.RequireForUpdate(_playerQuery);
        _levelStarted = false;
    }

    public void OnUpdate(ref SystemState state)
    {
        var ctx = Context.Instance;
        if (ctx == null) return;

        bool inGame = ctx.AppSystem.CurrentState == AppState.Game;
        if (!inGame)
        {
            _levelStarted = false;
            return;
        }

        int bricksAlive = _bricksQuery.CalculateEntityCount();

       
        if (!_levelStarted)
        {
            if (bricksAlive > 0)
                _levelStarted = true;
            else
                return; 
        }
        if (bricksAlive == 0)
        {
            TriggerFinish(ref state, FinishReason.Win);
            return;
        }

        var playerData = _playerQuery.GetSingleton<PlayerData>();
        int bulletsAlive = _bulletsQuery.CalculateEntityCount();

        if (playerData.BollValue <= 0 && bulletsAlive == 0)
        {
            TriggerFinish(ref state, FinishReason.Lose);
        }
    }

    private void TriggerFinish(ref SystemState state, FinishReason reason)
    {
        var em = state.EntityManager;

        if (!SystemAPI.HasSingleton<FinishData>())
        {
            var e = em.CreateEntity();
            em.AddComponentData(e, new FinishData { Reason = reason });
        }
        else
        {
            var data = SystemAPI.GetSingleton<FinishData>();
            data.Reason = reason;
            SystemAPI.SetSingleton(data);
        }

        if (Context.Instance.AppSystem.CurrentState == AppState.Finish) return;

        Context.Instance.AppSystem.Trigger(AppTriger.ToFinish);
    }
}
