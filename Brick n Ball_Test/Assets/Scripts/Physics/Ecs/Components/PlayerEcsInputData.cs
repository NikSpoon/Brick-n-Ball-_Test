using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

public struct PlayerEcsInputData : IComponentData
{
    public float2 Move;

    public bool Jump;

    public bool Fire;
    public bool FierPresed;

}
public class MyInputActionEcs : IComponentData
{
    public InputActionAsset MyInputAction;
}