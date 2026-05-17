using Firebase.Firestore;
using System;
using System.Collections.Generic;

public static class FirestoreEvents
{
    #region SAVE EVENTS

    public static Action<int, int, int, Timestamp, Timestamp>
        OnSaveStatistics;

    #endregion

    #region UPDATE EVENTS

    public static Action<float> OnUpdateInstructionTime;

    #endregion

    #region READ EVENTS

    public static Action<List<PlayerScoreData>>
        OnScoresLoaded;

    #endregion
}