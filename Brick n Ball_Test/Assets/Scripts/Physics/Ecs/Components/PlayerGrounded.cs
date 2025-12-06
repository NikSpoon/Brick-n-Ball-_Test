using Unity.Entities;

public struct GroundTag : IComponentData { }

public struct PlayerGrounded : IComponentData
{
    public bool Value;
}
