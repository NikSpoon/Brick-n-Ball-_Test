using Unity.Entities;

public struct AttackData : IComponentData
{
    public Entity ShellPrefab;   
    public float Coldaun;        
    public float BigColdaun;    
    public float ShellsValue;    
    public float Damage;

    public float CurrentColdaun; 
}