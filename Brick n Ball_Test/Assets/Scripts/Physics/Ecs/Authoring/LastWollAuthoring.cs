using Unity.Entities;
using UnityEngine;

class LastWollAuthoring : MonoBehaviour
{
    class LastWollAuthoringBaker : Baker<LastWollAuthoring>
    {
        public override void Bake(LastWollAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent<LastWallTeg>(entity);
        }
    }

}
