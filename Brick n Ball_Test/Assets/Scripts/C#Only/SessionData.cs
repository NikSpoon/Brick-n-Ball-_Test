
public class SessionData 
{
    public int PlayerScore { get; private set; }
    
    public void InitSession()
    {
        PlayerScore = 0;
    }
    public void AddScore (int value)
    {
        PlayerScore += value;
    }
    public void AddScore()
    {
        PlayerScore += 1;
    }
    public void ClearSession()
    {
        PlayerScore = 0;
    }
}
