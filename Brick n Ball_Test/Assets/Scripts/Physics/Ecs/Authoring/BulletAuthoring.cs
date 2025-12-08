
using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
   private GameObject _visualPrefab;
    class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponentObject(entity, authoring._visualPrefab);
        }
    }
}
