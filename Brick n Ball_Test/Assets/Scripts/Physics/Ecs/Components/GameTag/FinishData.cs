using Unity.Entities;

public struct FinishData : IComponentData
{
    public FinishReason Reason;
}