using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

[BurstCompile]
public partial struct PlayerEcsInputSystem : ISystem
{
    private bool _initialized;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MyInputActionEcs>();
        state.RequireForUpdate<PlayerEcsInputData>();

        _initialized = false;
    }

    public void OnDestroy(ref SystemState state)
    {
        if (SystemAPI.ManagedAPI.TryGetSingleton<MyInputActionEcs>(out var inputSingleton))
        {
            inputSingleton.MyInputAction.Disable();
        }
    }

    public void OnUpdate(ref SystemState state)
    {
        if (Context.Instance.AppSystem.CurrentState != AppState.Game)
            return;
        if (!SystemAPI.ManagedAPI.TryGetSingleton<MyInputActionEcs>(out var inputSingleton))
            return;

        var asset = inputSingleton.MyInputAction;

        if (!_initialized)
        {
            asset.Enable();
            _initialized = true;
        }

        var map = asset.FindActionMap("GamePlay", throwIfNotFound: true);

        var moveAction = map.FindAction("Movement", throwIfNotFound: true);
        var jumpAction = map.FindAction("Jump", throwIfNotFound: true);
        var fireAction = map.FindAction("Fire", throwIfNotFound: true);
        var firePressedAction = map.FindAction("FierPresed", throwIfNotFound: true);

        var moveVec = moveAction.ReadValue<UnityEngine.Vector2>();
        float2 move = new float2(moveVec.x, moveVec.y);

        bool jump = jumpAction.WasPerformedThisDynamicUpdate();
        bool fire = fireAction.WasPerformedThisDynamicUpdate();
        bool firePressed = firePressedAction.WasPerformedThisDynamicUpdate();

        foreach (var playerInput in SystemAPI.Query<RefRW<PlayerEcsInputData>>())
        {
            playerInput.ValueRW = default;

            playerInput.ValueRW.Move = move;
            playerInput.ValueRW.Jump = jump;
            playerInput.ValueRW.Fire = fire;
            playerInput.ValueRW.FierPresed = firePressed;
        }
    }
}
