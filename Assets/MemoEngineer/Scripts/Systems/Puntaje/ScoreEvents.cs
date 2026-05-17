using System;
public class ScoreEvents
{

    public static Action OnToxicDodged;
    public static Action OnGoodCollected;
    public static Action OnBadCollected;
    public static Action FinishGame;

    public static Action<int> ScoreChanged;
}
