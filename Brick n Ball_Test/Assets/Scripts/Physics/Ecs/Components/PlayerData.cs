using Unity.Entities;
using Unity.Mathematics;

public struct PlayerData : IComponentData
{
    public float3 GraundRoot;
    public float3 GunPointerRoot;

    public int BollValue;
}
