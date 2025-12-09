using Unity.Entities;
using UnityEngine;

public class GameDataAuthoring : MonoBehaviour
{
    class Baker : Baker<GameDataAuthoring>
    {
        public override void Bake(GameDataAuthoring authoring)
        {

            var entity = GetEntity(TransformUsageFlags.None);

            AddComponentObject(entity, new PlayerProfComponent()
            {
                PlayerProfaile = Context.Instance.PlayerProf
            });
            AddComponentObject(entity, new SessionDataComponent()
            {
                SessionData = Context.Instance.SessionData
            });
        }
    }
}