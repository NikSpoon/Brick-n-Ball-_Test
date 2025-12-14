using System;
public class FinishDataProf
{
    public int Score { get; private set; }
    public int StartLevl { get; private set; }
    public int FinishLevl { get; private set; }
    public int LevlScore { get; private set; }

    public FinishReason Reason { get; private set; }
    public void ClearFinishData()
    {
        Score = 0;
        StartLevl = Context.Instance.PlayerProf.Levl;
        LevlScore = 0;
        FinishLevl = 0;
        Reason = FinishReason.None;
    }
    public void AddScore(int value)
    {
        Score += value;
    }
    public void AddLevl(int value)
    {
        LevlScore += value;
    }
    public void FinishGame()
    {
        FinishLevl = Context.Instance.PlayerProf.Levl;
    }
   
    public void SetReason(FinishReason reason) 
    {
        Reason = reason;
    }
}

