using System;
public interface IAppSystem
{
    AppState CurrentState { get; }
    void Trigger(AppTriger trigger);
    event Action<StateChangeData<AppState, AppTriger>> OnStateChange;
}
public class AppSystem : IAppSystem
{
    private Fsm<AppState, AppTriger> _stateMashine;
    public AppState CurrentState => _stateMashine.CurrentState;

    event Action<StateChangeData<AppState, AppTriger>> IAppSystem.OnStateChange
    {
        add => _stateMashine.OnStateChange += value;
        remove => _stateMashine.OnStateChange -= value;
    }

    public AppSystem()
    {
        _stateMashine = new Fsm<AppState, AppTriger>(AppState.Loading);

        _stateMashine.AddTransition(AppState.Loading, AppTriger.ToMainMenu, AppState.MainMenu);

        _stateMashine.AddTransition(AppState.MainMenu, AppTriger.ToGame, AppState.Game);

        _stateMashine.AddTransition(AppState.Game, AppTriger.ToFinish, AppState.Finish);

        _stateMashine.AddTransition(AppState.Finish, AppTriger.ToMainMenu, AppState.MainMenu);

    }

    public void Trigger(AppTriger trigger)
    {
        _stateMashine.SetTrigger(trigger);
    }

}

public enum AppState
{
    Loading,
    MainMenu,
    Game,
    Finish
}

public enum AppTriger
{
    ToMainMenu,
    ToGame,
    ToFinish
}