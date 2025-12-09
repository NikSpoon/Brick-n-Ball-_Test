using Unity.Entities;
using UnityEngine;

public class GameDataAuthoring : MonoBehaviour
{
    class Baker : Baker<GameDataAuthoring>
    {
        public override void Bake(GameDataAuthoring authoring)
        {

            var entity = GetEntity(TransformUsageFlags.None);

           
            AddComponent(entity, new SessionDataEsc(){});
        }
    }
}