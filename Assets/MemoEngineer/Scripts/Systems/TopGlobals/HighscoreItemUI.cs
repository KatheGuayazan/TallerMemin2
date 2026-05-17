using TMPro;
using UnityEngine;

public class HighscoreItemUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text playerNameText;

    [SerializeField]
    private TMP_Text scoreText;

    public void Setup(
        string playerName,
        int score)
    {
        playerNameText.text = playerName;

        scoreText.text = score.ToString();
    }
}