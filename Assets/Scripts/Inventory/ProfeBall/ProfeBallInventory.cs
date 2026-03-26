using System;
using System.Collections.Generic;
using UnityEngine;
public enum BallType
{
    Basic,
    Super,
    Ultra
}

[Serializable]
public class BallEntry
{
    public BallType type;
    public int amount;
}

[Serializable]
public class ProfeBallInventory
{
    private Dictionary<BallType, int> balls = new();

    [SerializeField] private List<BallEntry> debugBalls = new();

    public event Action<BallType, int> OnBallChanged;

    public void Add(BallType type, int amount)
    {
        if (amount <= 0) return;

        if (!balls.ContainsKey(type))
            balls[type] = 0;

        balls[type] += amount;

        SyncDebug();
        OnBallChanged?.Invoke(type, balls[type]);
    }

    public bool Use(BallType type, int amount = 1)
    {
        if (!balls.ContainsKey(type) || balls[type] < amount)
            return false;

        balls[type] -= amount;

        SyncDebug();
        OnBallChanged?.Invoke(type, balls[type]);

        return true;
    }

    public int GetBall(BallType type)
    {
        return balls.ContainsKey(type) ? balls[type] : 0;
    }

    public bool HasBall(BallType type, int amount = 1)
    {
        return GetBall(type) >= amount;
    }

    private void SyncDebug()
    {
        debugBalls.Clear();

        foreach (var pair in balls)
        {
            debugBalls.Add(new BallEntry
            {
                type = pair.Key,
                amount = pair.Value
            });
        }
    }
}