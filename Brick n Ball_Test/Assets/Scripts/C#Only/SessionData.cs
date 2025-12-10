
using System;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine.SocialPlatforms.Impl;

public class SessionData
{
    public int PlayerScore { get; private set; }
    public event Action<int> OnScoreChenged;
    public void InitSession()
    {
        PlayerScore = 0;
    }
    public void AddScore(int value)
    {
        PlayerScore += value;
        OnScoreChenged?.Invoke(PlayerScore);
    }
    public void AddScore()
    {
        PlayerScore += 1;
        OnScoreChenged?.Invoke(PlayerScore);
    }
    public void ClearSession()
    {
        PlayerScore = 0;
    }
    public void ForceSetScore(int value)
    {
        var currentScore = PlayerScore;
        PlayerScore = value;
        if (currentScore != PlayerScore)
            OnScoreChenged?.Invoke(PlayerScore);
    }
}
