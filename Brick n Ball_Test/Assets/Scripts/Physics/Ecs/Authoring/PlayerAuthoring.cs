using Unity.Entities;
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
    [SerializeField] private Transform _gunPointRoot;
    [SerializeField] private GameObject _shellPrefab;
    [SerializeField] private float _coldaun = 0.25f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private int _bollValue = 10;
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerData
            {
                GraundRoot = authoring._graundRoot.localPosition,
                GunPointerRoot = authoring._gunPointRoot.localPosition,
                BollValue = authoring._bollValue
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
           
            var shellEntity = GetEntity(authoring._shellPrefab,
                                       TransformUsageFlags.Dynamic);

            AddComponent(entity, new AttackData
            {
                ShellPrefab = shellEntity,
                Coldaun = authoring._coldaun,
                Damage = authoring._damage,
                CurrentColdaun = 0f
            });
        }
    }
}