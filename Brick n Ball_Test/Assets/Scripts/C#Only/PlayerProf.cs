
public class PlayerProf
{

    public string Name { get; private set; }
    public int Levl { get; private set; }

    public void InitProf(string name)
    {
        Name = name;
        Levl = 0;
    }
    public void InitTestProf()
    {
        Name = "Testr";
        Levl = 100;
    }
    public void AddLevl(int value)
    {
        Levl += value;
    }
    public void AddLevl()
    {
        Levl += 1;
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
}
