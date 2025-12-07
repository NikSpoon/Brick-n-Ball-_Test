using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct AttackSystem : ISystem
{
  

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        // ECB для спавна сущностей
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (input, transform, playerData, attackDataRW, entity)
                 in SystemAPI.Query<
                        RefRO<PlayerEcsInputData>,
                        RefRO<LocalTransform>,
                        RefRO<PlayerData>,
                        RefRW<AttackData>>()
                     .WithAll<Simulate>()
                     .WithEntityAccess())
        {
            ref var attackData = ref attackDataRW.ValueRW;

            // уменьшаем кулдаун
            if (attackData.CurrentColdaun > 0f)
                attackData.CurrentColdaun -= dt;

            // нет нажатия или кулдаун ещё идёт – пропускаем
            if (!input.ValueRO.Fire || attackData.CurrentColdaun > 0f)
                continue;

            // считаем позицию ствола
            float3 gunPos = transform.ValueRO.Position + playerData.ValueRO.GunPointerRoot;

            // направление вперёд из поворота игрока
            float3 forward = math.mul(transform.ValueRO.Rotation, new float3(0, 0, 1));

            // спавним снаряд
            Entity shell = ecb.Instantiate(attackData.ShellPrefab);

            // ставим позицию и поворот пули
            ecb.SetComponent(shell, new LocalTransform
            {
                Position = gunPos,
                Rotation = transform.ValueRO.Rotation,
                Scale = 1f
            });

            // задаём начальную скорость
            float shellSpeed = 20f; // можешь вынести в данные
            ecb.SetComponent(shell, new PhysicsVelocity
            {
                Linear = forward * shellSpeed,
                Angular = float3.zero
            });

            // сбрасываем кулдаун
            attackData.CurrentColdaun = attackData.Coldaun;
        }
    }
}