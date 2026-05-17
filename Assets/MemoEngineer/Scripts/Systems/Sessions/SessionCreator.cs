using TMPro;
using UnityEngine;

public class SessionCreator : MonoBehaviour
{
    #region REFERENCES

    [SerializeField] private Firestore firestore;

    [SerializeField] private TMP_InputField nameInput;

    #endregion

    #region CREATE SESSION

    public async void CreateSession()
    {
        if (firestore == null) return;

        string playerName =
            nameInput.text;

        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "Player";
        }

        string documentId =
            await firestore.CreateSection(playerName);

        FirestoreSession.CurrentDocumentId =
            documentId;

        FirestoreSession.CurrentPlayerName =
            playerName;

        Debug.Log(
            "Session Created: " +
            documentId
        );
    }

    #endregion
}