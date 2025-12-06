using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAuthoring : MonoBehaviour
{
   [SerializeField] private InputActionAsset _myInputAction;
    class PlayeInputBaking : Baker<PlayerInputAuthoring>
    {
       public override void Bake(PlayerInputAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PlayerEcsInputData { });

            AddComponentObject(entity, new MyInputActionEcs
            {
                MyInputAction = authoring._myInputAction
            });
        }
    }
}
 