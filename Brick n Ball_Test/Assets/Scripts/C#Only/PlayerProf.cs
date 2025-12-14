
using System;

public class PlayerProf
{

    public string Name { get; private set; }
    public int Levl { get; private set; }

    public event Action<int> OnLevlChenged;
    public void InitProf(string name)
    {
        Name = name;
        Levl = 0;
    }
    public void InitTestProf()
    {
        Name = "Testr";
        Levl = 0;
    }
    public void AddLevl(int value)
    {
        Levl += value;
        OnLevlChenged?.Invoke(Levl);
    }
    public void AddLevl()
    {
        Levl += 1;
        OnLevlChenged?.Invoke(Levl);
    }
    public void ChengeName(string name)
    {
        Name = name;
    }
    public void FindProf(string pasword)
    {
        //For find 

        /*
        foreach (var item in collection)
        {
            if(item.pas == pasword)
            {
                Name = item.Name;
                Levl = item.Levl;
            }

        }
        */
    }

    public void ClearProf()
    {
        Name = null;
        Levl = 0;
    }
    public void ForceSetLevel(int value)
    {
        var currentLevl = Levl;
        Levl = value;
        if (currentLevl != Levl)
            OnLevlChenged?.Invoke(Levl);   
    }
}
