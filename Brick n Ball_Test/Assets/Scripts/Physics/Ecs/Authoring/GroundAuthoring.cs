using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class GroundAuthoring : MonoBehaviour
{
    class Baker : Baker<GroundAuthoring>
    {
        public override void Bake(GroundAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<GroundTag>(entity);
        }
    }
}