using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text toxicDodgedText;
    [SerializeField] private TMP_Text goodCollectedText;
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text goodCollectedLostText;

    [Header("Panels")]
    [SerializeField] private GameObject scorePanel;

    private int toxicDodged;
    private int goodCollected;
    private int total;
    private int goodCollectedLost;

    private int multiplerToxic = 3;

    private void OnEnable()
    {
        ScoreEvents.OnToxicDodged += AddToxicDodged;
        ScoreEvents.OnGoodCollected += AddGoodCollected;
        ScoreEvents.FinishGame += ShowScorePanel;
        ScoreEvents.OnBadCollected += minusTotal;
    }

    private void OnDisable()
    {
        ScoreEvents.OnToxicDodged -= AddToxicDodged;
        ScoreEvents.OnGoodCollected -= AddGoodCollected;
        ScoreEvents.FinishGame -= ShowScorePanel;
        ScoreEvents.OnBadCollected -= minusTotal;
    }

    private void Start()
    {
        scorePanel.SetActive(false);

        UpdateUI();
        GameManager.CursorVisible(false);
    }

    private void AddToxicDodged()
    {
        toxicDodged += multiplerToxic;

        total += toxicDodged;

        UpdateUI();
    }

    private void AddGoodCollected()
    {
        goodCollected++;

        total += 1;

        UpdateUI();
    }

    private void minusTotal() {

        goodCollectedLost++;
        total -= 1;
        UpdateUI();
    }

    private void ShowScorePanel()
    {
        scorePanel.SetActive(true);
        GameManager.CursorVisible(true);

    }



    private void UpdateUI()
    {
        toxicDodgedText.text = $"{toxicDodged}";
        goodCollectedText.text = $"{goodCollected}";
        totalScoreText.text = $"{total}";
        goodCollectedLostText.text = $"arduinos perdidos: {goodCollectedLost}";

        ScoreEvents.ScoreChanged?.Invoke(total);

        Debug.Log($"Toxics Dodged: {toxicDodged}, Goods Collected: {goodCollected}, Total Score: {total}");
    }
}