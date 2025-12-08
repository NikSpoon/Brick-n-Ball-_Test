using Unity.Entities;
using UnityEngine;

public class CameraLookAuthoring : MonoBehaviour
{
    class Baker : Baker<CameraLookAuthoring>
    {
        public override void Bake(CameraLookAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CameraData
            {
            });
        }
    }
}