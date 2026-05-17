using System;
using Firebase.Firestore;

public static class FirestoreEvents
{
    #region SAVE EVENTS

    public static Action<int, int, int, Timestamp, Timestamp>
        OnSaveStatistics;

    #endregion

    #region UPDATE EVENTS

    public static Action<float> OnUpdateInstructionTime;

    #endregion
}