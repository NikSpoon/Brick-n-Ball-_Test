using Unity.Collections;
using Unity.Entities;

public partial struct EnsurePlayerSimulateOnGameEnterSystem : ISystem
{
    private AppState _prev;
    private bool _pending;
    private EntityQuery _playerQuery;

    public void OnCreate(ref SystemState state)
    {
        _prev = AppState.MainMenu;
        _pending = false;

        _playerQuery = state.GetEntityQuery(
            ComponentType.ReadOnly<PlayerData>()
        );
    }

    public void OnUpdate(ref SystemState state)
    {
        var ctx = Context.Instance;
        if (ctx == null) return;

        var cur = ctx.AppSystem.CurrentState;

        if (cur == AppState.Game && _prev != AppState.Game)
            _pending = true;

        if (!_pending || cur != AppState.Game)
        {
            _prev = cur;
            return;
        }

        
        if (_playerQuery.CalculateEntityCount() == 0)
            return;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        using var players = _playerQuery.ToEntityArray(Allocator.Temp);
        foreach (var e in players)
        {
            if (!state.EntityManager.HasComponent<Simulate>(e))
                ecb.AddComponent<Simulate>(e);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        _pending = false;
        _prev = cur; 
    }

}
