using System.Collections.Generic;
using UnityEngine;

public class HighscoreUI : MonoBehaviour
{
    #region REFERENCES

    [Header("Firestore Handler")]
    [SerializeField]
    private FirestoreEventHandler firestoreHandler;

    [Header("UI")]
    [SerializeField]
    private HighscoreItemUI itemPrefab;

    [SerializeField]
    private Transform contentParent;

    #endregion

    #region UNITY METHODS

    private void OnEnable()
    {
        FirestoreEvents.OnScoresLoaded += OnScoresLoaded;
    }

    private void OnDisable()
    {
        FirestoreEvents.OnScoresLoaded -= OnScoresLoaded;
    }

    #endregion

    #region BUTTON EVENT


    public void LoadHighscores()
    {
        if (firestoreHandler == null)
        {
            Debug.LogWarning("FirestoreHandler reference missing");
            return;
        }

        firestoreHandler.LoadScores();
    }

    #endregion

    #region SCORE LOADED

    private void OnScoresLoaded(
        List<PlayerScoreData> scores)
    {
        ClearContent();

        foreach (PlayerScoreData data in scores)
        {
            HighscoreItemUI item =
                Instantiate(
                    itemPrefab,
                    contentParent);

            item.Setup(
                data.playerName,
                data.score);
        }
    }

    #endregion

    #region CLEAR

    private void ClearContent()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion
}