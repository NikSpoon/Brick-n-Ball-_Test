using Unity.Entities;
using Unity.Mathematics;

public struct BrickTag : IComponentData { }
public struct BrickVisualTag : IComponentData { }
public struct BrickSpawnerData : IComponentData
{
    public Entity BrickPrefab;
    public int BrickCount;
    public uint RandomSeed;
}

public struct BrickSpawnPoint : IBufferElementData
{
    public float3 Position;
    public quaternion Rotation;
}

public struct BrickHealth : IComponentData
{
    public int Value;
}