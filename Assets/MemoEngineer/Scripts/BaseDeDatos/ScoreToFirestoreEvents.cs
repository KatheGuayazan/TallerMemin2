using UnityEngine;
using Firebase.Firestore;
public class ScoreToFirestoreEvents : MonoBehaviour
{
    #region VARIABLES

    private int collectedArduinos;
    private int lostArduinos;
    private int dodgedToxics;

    private Timestamp sessionStartTime;

    #endregion

    #region UNITY METHODS

    private void Start()
    {
        ResetStats();

        sessionStartTime = Timestamp.GetCurrentTimestamp();
    }

    private void OnEnable()
    {
        ScoreEvents.OnGoodCollected += OnGoodCollected;
        ScoreEvents.OnBadCollected += OnBadCollected;
        ScoreEvents.OnToxicDodged += OnToxicDodged;
        ScoreEvents.FinishGame += OnFinishGame;
    }

    private void OnDisable()
    {
        ScoreEvents.OnGoodCollected -= OnGoodCollected;
        ScoreEvents.OnBadCollected -= OnBadCollected;
        ScoreEvents.OnToxicDodged -= OnToxicDodged;
        ScoreEvents.FinishGame -= OnFinishGame;
    }

    #endregion

    #region RESET

    private void ResetStats()
    {
        collectedArduinos = 0;
        lostArduinos = 0;
        dodgedToxics = 0;
    }

    #endregion

    #region SCORE EVENTS

    private void OnGoodCollected()
    {
        collectedArduinos++;
    }

    private void OnBadCollected()
    {
        lostArduinos++;
    }

    private void OnToxicDodged()
    {
        dodgedToxics += 3;
    }

    #endregion

    #region FINISH GAME

    private void OnFinishGame()
    {
        FirestoreEvents.OnSaveStatistics?.Invoke(
            collectedArduinos,
            lostArduinos,
            dodgedToxics,
            sessionStartTime,
            Timestamp.GetCurrentTimestamp()
        );
    }

    #endregion
}