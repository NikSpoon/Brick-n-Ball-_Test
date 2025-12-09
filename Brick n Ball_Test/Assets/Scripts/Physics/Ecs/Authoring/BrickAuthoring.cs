using Unity.Entities;
using UnityEngine;

public class BrickAuthoring : MonoBehaviour
{
    public int StartHp = 3;

    class Baker : Baker<BrickAuthoring>
    {
        public override void Bake(BrickAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BrickTag>(entity);
            AddComponent(entity, new BrickHealth { Value = authoring.StartHp });
        }
    }
}
