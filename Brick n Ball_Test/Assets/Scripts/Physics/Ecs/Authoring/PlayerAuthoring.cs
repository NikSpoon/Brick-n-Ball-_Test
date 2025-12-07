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
    [SerializeField] private float _distanseForce = 8f;
    [SerializeField] private Transform _graundRoot;
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerData
            {
                GraundRoot = authoring._graundRoot.position
            });

            AddComponent(entity, new PlayerEcsInputData { });

            AddComponent(entity, new PlayerMovementData
            {
                MoveSpeed = authoring._moveSpeed,
                JumpForce = authoring._jumpForce,
                JumpDistance = authoring._distanseForce
            });

            AddComponentObject(entity, new MyInputActionEcs
            {
                MyInputAction = authoring._myInputAction
            });

        }
    }
}