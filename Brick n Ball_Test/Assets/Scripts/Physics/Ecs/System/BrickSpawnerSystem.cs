using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct BrickSpawnerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (Context.Instance.AppSystem.CurrentState != AppState.Game)
            return;

        var em = state.EntityManager;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (spawnerData, pointsBuffer, spawnerEntity)
                 in SystemAPI.Query<RefRO<BrickSpawnerData>, DynamicBuffer<BrickSpawnPoint>>()
                     .WithEntityAccess())
        {
            var data = spawnerData.ValueRO;

            if (data.BrickCount <= 0 || pointsBuffer.Length == 0)
            {
                ecb.RemoveComponent<BrickSpawnerData>(spawnerEntity);
                continue;
            }

            uint seed = data.RandomSeed;
            if (seed == 0) seed = 1;
            var rand = new Unity.Mathematics.Random(seed);

            var freeIndices = new NativeList<int>(pointsBuffer.Length, Allocator.Temp);
            for (int i = 0; i < pointsBuffer.Length; i++)
                freeIndices.Add(i);

            int spawnCount = math.min(data.BrickCount, pointsBuffer.Length);

            for (int i = 0; i < spawnCount; i++)
            {
                int idxInList = rand.NextInt(0, freeIndices.Length);
                int pointIndex = freeIndices[idxInList];
                freeIndices.RemoveAtSwapBack(idxInList);

                var point = pointsBuffer[pointIndex];

                Entity brick = ecb.Instantiate(data.BrickPrefab);

                ecb.SetComponent(brick, LocalTransform.FromPositionRotationScale(
                    point.Position,
                    point.Rotation,
                    1f
                ));

                ecb.AddComponent<BrickTag>(brick);
            }

            freeIndices.Dispose();

            ecb.RemoveComponent<BrickSpawnerData>(spawnerEntity);
        }
    }
}
