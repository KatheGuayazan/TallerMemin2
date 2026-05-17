using Firebase.Firestore;
using UnityEngine;

public class FirestoreEventHandler : MonoBehaviour
{
    #region REFERENCES

    [SerializeField] private Firestore firestore;

    [SerializeField] private string documentIdPrefix = "IDPartida";

    #endregion

    #region UNITY METHODS

    private void OnEnable()
    {
        FirestoreEvents.OnSaveStatistics += OnSaveStatistics;
    }

    private void OnDisable()
    {
        FirestoreEvents.OnSaveStatistics -= OnSaveStatistics;
    }

    #endregion

    #region SESSION


    #endregion

    #region SAVE STATISTICS

    private async void OnSaveStatistics(
        int recolectados,
        int perdidos,
        int esquivados,
        Timestamp horaInicio,
        Timestamp horaFinal)
    {
        if (firestore == null) return;

        await firestore.AppendStatisticsEntry(
            documentIdPrefix,
            recolectados,
            perdidos,
            esquivados,
            horaInicio,
            horaFinal
        );

        Debug.Log("Statistics Saved");
    }

    #endregion
}