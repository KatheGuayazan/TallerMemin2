using Firebase.Firestore;
using UnityEngine;

public class FirestoreEventHandler : MonoBehaviour
{
    #region REFERENCES

    [Header("Firestore Reference")]
    [SerializeField] private Firestore firestore;

    #endregion

    #region DOCUMENT SETTINGS

    [Header("Document Settings")]

    [SerializeField]
    private bool useGeneratedDocumentId = true;

    [SerializeField]
    private string manualDocumentId = "IDPartida";

    #endregion

    #region UNITY METHODS

    private void OnEnable()
    {
        FirestoreEvents.OnSaveStatistics +=
            OnSaveStatistics;

        FirestoreEvents.OnUpdateInstructionTime +=
            OnUpdateInstructionTime;
    }

    private void OnDisable()
    {
        FirestoreEvents.OnSaveStatistics -=
            OnSaveStatistics;

        FirestoreEvents.OnUpdateInstructionTime -=
            OnUpdateInstructionTime;
    }

    #endregion

    #region DOCUMENT ID

    private string GetDocumentId()
    {
        if (useGeneratedDocumentId)
        {
            return FirestoreSession.CurrentDocumentId;
        }

        return manualDocumentId;
    }

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

        string documentId =
            GetDocumentId();

        if (string.IsNullOrEmpty(documentId))
        {
            Debug.LogWarning(
                "Document ID is null or empty"
            );

            return;
        }

        await firestore.AppendStatisticsEntry(
            documentId,
            recolectados,
            perdidos,
            esquivados,
            horaInicio,
            horaFinal
        );

        Debug.Log("Statistics Saved");
    }

    #endregion

    #region UPDATE INSTRUCTION TIME

    private async void OnUpdateInstructionTime(
        float time)
    {
        if (firestore == null) return;

        string documentId =
            GetDocumentId();

        if (string.IsNullOrEmpty(documentId))
        {
            Debug.LogWarning(
                "Document ID is null or empty"
            );

            return;
        }

        await firestore.UpdateTiempoInstrucciones(
            documentId,
            time
        );

        Debug.Log(
            "Instruction Time Updated"
        );
    }

    #endregion
}