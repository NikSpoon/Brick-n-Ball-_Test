using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField] private InputActionAsset _myInputAction;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 8f;
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerData>(entity);

            AddComponent(entity, new PlayerEcsInputData { });

            AddComponent(entity, new PlayerMovementData
            {
                MoveSpeed = authoring._moveSpeed,
                JumpForce = authoring._jumpForce
            });

            AddComponentObject(entity, new MyInputActionEcs
            {
                MyInputAction = authoring._myInputAction
            });

        }
    }
}