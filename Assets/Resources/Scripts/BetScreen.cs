using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System;
using Unity.VisualScripting;

public class BettingManager : MonoBehaviour
{
    [Header("UI References")]
    public Text balanceText;
    public UnityEngine.UI.Button betBlueButton;
    public UnityEngine.UI.Button betRedButton;
    public long betAmount = 0;
    private long playerBalance;
    private float timer;

    public Text errorCode;
    public Text betText;

    public UnityEngine.UI.Button plus1;
    public UnityEngine.UI.Button plus10;
    public UnityEngine.UI.Button plus100;
    public UnityEngine.UI.Button plus1000;

    public UnityEngine.UI.Button minus1;
    public UnityEngine.UI.Button minus10;
    public UnityEngine.UI.Button minus100;
    public UnityEngine.UI.Button minus1000;

    public UnityEngine.UI.Button ALLIN;
    const long inf = 99999999999999999;
    void Start()
    {
        playerBalance = GameDataManager.Instance.playerBalance;

        UpdateBalanceText();

        betBlueButton.onClick.AddListener(() => PlaceBet("Blue"));
        betRedButton.onClick.AddListener(() => PlaceBet("Red"));

        plus1.onClick.AddListener(() => betSetter(1));
        plus10.onClick.AddListener(() => betSetter(10));
        plus100.onClick.AddListener(() => betSetter(100));
        plus1000.onClick.AddListener(() => betSetter(1000));

       minus1.onClick.AddListener(() => betSetter(-1));
       minus10.onClick.AddListener(() => betSetter(-10));
       minus100.onClick.AddListener(() => betSetter(-100));
       minus1000.onClick.AddListener(() => betSetter(-1000));     
        ALLIN.onClick.AddListener(() => AllIn());
    }
    private void Update()
    {
        if(timer != 0)
        {
            if(Time.time - timer > 3f)
            {
                errorCode.enabled = false;
                timer = 0;
            }
        }
    }
    void betSetter(long bet)
    {
        betAmount += bet;
        betAmount = Convert.ToInt64(Mathf.Clamp(betAmount, 0, inf));
        betText.text = betAmount.ToString();
    }

    void AllIn()
    {
        betAmount = playerBalance;
        betText.text = betAmount.ToString();
    }
    void PlaceBet(string team)
    {
        if (betAmount > playerBalance)
        {
            errorCode.enabled = true;
            timer = Time.time;
            return;
        }

        GameDataManager.Instance.currentBetAmount = betAmount;
        GameDataManager.Instance.chosenTeam = team;

        playerBalance -= betAmount;
        GameDataManager.Instance.playerBalance = playerBalance;
        UpdateBalanceText();

        betBlueButton.interactable = false;
        betRedButton.interactable = false;

        Debug.Log($"Bet of {betAmount} placed on {team} team. Switching scene...");
        string[] battleScenes = { "Map_1", "Map_2", "Map_3", /*"Map_4",*/ "Map_5"}; 
        string chosenScene = battleScenes[UnityEngine.Random.Range(0, battleScenes.Length)];

        SceneManager.LoadScene(chosenScene);
    }

    void UpdateBalanceText()
    {
        balanceText.text = "Balance: " + playerBalance.ToString();
    }
}
