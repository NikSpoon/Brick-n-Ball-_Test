using Unity.Entities;
using Unity.Mathematics;

public struct PlayerLookInput : IComponentData
{
    public float3 LookDirection;
    public float Speed;
}
