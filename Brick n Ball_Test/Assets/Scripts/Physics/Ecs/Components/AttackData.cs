using Unity.Entities;

public struct AttackData : IComponentData
{
    public Entity ShellPrefab;   
    public float Coldaun;        
    public float CurrentColdaun; 


    public float Damage;
}