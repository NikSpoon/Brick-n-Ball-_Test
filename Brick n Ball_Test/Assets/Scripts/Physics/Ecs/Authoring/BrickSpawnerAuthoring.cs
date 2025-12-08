using Unity.Entities;
using UnityEngine;

class BrickSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _brickPrefab;   
    [SerializeField] private int _brickCount = 10;

    class Baker : Baker<BrickSpawnerAuthoring>
    {
        public override void Bake(BrickSpawnerAuthoring authoring)
        {
            var spawnerEntity = GetEntity(TransformUsageFlags.None);

            var brickPrefabEntity =
                GetEntity(authoring._brickPrefab, TransformUsageFlags.Dynamic);

            AddComponent(spawnerEntity, new BrickSpawnerData
            {
                BrickPrefab = brickPrefabEntity,
                BrickCount = authoring._brickCount,
                RandomSeed = 123u  
            });

            var buffer = AddBuffer<BrickSpawnPoint>(spawnerEntity);

            foreach (var t in authoring._spawnPoints)
            {
                if (t == null) continue;

                buffer.Add(new BrickSpawnPoint
                {
                    Position = t.position,
                    Rotation = t.rotation
                });
            }
        }
    }
}

