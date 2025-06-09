using System;
using TMPro;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance;
    [Header("UI Elements")]
    [SerializeField] GameObject UIMoneyValue;
    [SerializeField] GameObject UIIncomeValue;
    [SerializeField] GameObject UIMultiplierValue;
    [SerializeField] GameObject UITickrateValue;
    [SerializeField] GameObject UICardCountValue;
    [SerializeField] GameObject UINextCardValue;
    [SerializeField] GameObject UIAlert;
    [SerializeField] GameObject UIAlertText;
    [SerializeField] GameObject UICardDrawnTotal;
    [SerializeField] GameObject UICardDrawnTotalSlots;
    [Header("Notification Elements")]
    [SerializeField] Animator animatorNotification;
    [SerializeField] GameObject UINotification;
    [SerializeField] GameObject UINotificationText;

    [Header("Prestige Elements")]
    [SerializeField] GameObject PrestigeRewardText;
    [SerializeField] GameObject PrestigePointsText;
    [SerializeField] GameObject PrestigeMultiplierText;
    [SerializeField] GameObject PrestigeIncomeText;
    [SerializeField] GameObject PrestigeTickrateText;




    void Awake()
    {
        Instance = this;
        InitNotificationAnimator();
    }

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        // Update the UI text with the current money value
        if (UIMoneyValue != null && UIMultiplierValue != null && UIIncomeValue != null && UITickrateValue != null && UICardDrawnTotal != null && UICardDrawnTotalSlots != null)
        {
            UpdateUIBalance();
            UpdateUIMultiplier();
            UpdateUIIncome();
            UpdateUITickrate();
            UpdateUICardDrawn();
            // UpdateUIPrestige();
        }
    }



    public string FormatNotation(double money)
    {

        if (money >= 1_000_000_000_000)
        {
            return (money / 1_000_000_000_000).ToString("F2") + "T"; // Trillions
        }
        else if (money >= 1_000_000_000)
        {
            return (money / 1_000_000_000).ToString("F2") + "B"; // Billions
        }
        else if (money >= 1_000_000)
        {
            return (money / 1_000_000).ToString("F2") + "M"; // Millions
        }
        else if (money >= 1_000)
        {
            return (money / 1_000).ToString("F2") + "K"; // Thousands
        }
        else
        {
            return money.ToString("F2"); // Less than a thousand
        }
    }
    private void InitNotificationAnimator()
    {
        // Initialize the animator controller for notifications
        if (animatorNotification == null)
        {
            Debug.LogError("Animator for notifications is not assigned in the GameHandler.");
            return;
        }
        animatorNotification = UINotification.GetComponent<Animator>();
        UINotification.SetActive(true);
    }
    private void UpdateUITickrate()
    {
        if (UITickrateValue == null)
        {
            Debug.LogError("UITickrateValue GameObject not found in the scene.");
            return;
        }
        UITickrateValue.GetComponent<TextMeshProUGUI>().text = "/" + GameHandler.Instance.TICKRATE_SECONDS.ToString("F2") + "s";
    }
    void UpdateUIBalance()
    {
        if (UIMoneyValue == null)
        {
            Debug.LogError("UIMoneyValue GameObject not found in the scene.");
            return;
        }
        UIMoneyValue.GetComponent<TextMeshProUGUI>().text = FormatNotation(GameHandler.Instance.money);

    }
    void UpdateUIMultiplier()
    {
        if (UIMultiplierValue == null)
        {
            Debug.LogError("UIMultiplierValue GameObject not found in the scene.");
            return;
        }
        UIMultiplierValue.GetComponent<TextMeshProUGUI>().text = (GameHandler.Instance.incomeMultiplier * GameHandler.Instance.incomeComboMultiplier).ToString("F2");
    }
    void UpdateUICardDrawn()
    {
        UICardDrawnTotal.GetComponent<TextMeshProUGUI>().text = CardManager.Instance.CardDrawn.ToString();
        UICardDrawnTotalSlots.GetComponent<TextMeshProUGUI>().text = CardManager.Instance.HandCardSlots.ToString();
    }
    void UpdateUIIncome()
    {
        if (UIIncomeValue == null)
        {
            Debug.LogError("UIIncomeValue GameObject not found in the scene.");
            return;
        }
        UIIncomeValue.GetComponent<TextMeshProUGUI>().text = FormatNotation(GameHandler.Instance.income * GameHandler.Instance.incomeMultiplier * GameHandler.Instance.incomeComboMultiplier);
    }
    public void UpdateUINextCardValue() { UINextCardValue.GetComponent<TextMeshProUGUI>().text = GameHandler.Instance.CardsNextCost.ToString(); }
    void UpdateUIPrestige()
    {
        if (PrestigeRewardText == null || PrestigePointsText == null || PrestigeMultiplierText == null || PrestigeIncomeText == null || PrestigeTickrateText == null)
        {
            Debug.LogError("Prestige UI elements not assigned in the GameHandler.");
            Debug.LogError("Prestige UI elements not assigned in the GameHandler.");
            Debug.LogError("Prestige UI elements not assigned in the GameHandler.");
            return;
        }
        Debug.LogWarning(" UI HANDLER Reward: " + PrestigeHandler.Instance.PrestigeReward +
                      ", Points: " + PrestigeHandler.Instance.PrestigePoints +
                      ", Multiplier: " + PrestigeHandler.Instance.PrestigeMultiplier +
                      ", Income: " + PrestigeHandler.Instance.PrestigeIncome +
                      ", Tickrate: " + PrestigeHandler.Instance.PrestigeTickrate);

        PrestigeRewardText.GetComponent<TextMeshProUGUI>().text = PrestigeHandler.Instance.PrestigeReward.ToString();
        PrestigePointsText.GetComponent<TextMeshProUGUI>().text = FormatNotation(PrestigeHandler.Instance.PrestigePoints);
        PrestigeMultiplierText.GetComponent<TextMeshProUGUI>().text = PrestigeHandler.Instance.PrestigeMultiplier.ToString("F2");
        PrestigeIncomeText.GetComponent<TextMeshProUGUI>().text = FormatNotation(PrestigeHandler.Instance.PrestigeIncome);
        PrestigeTickrateText.GetComponent<TextMeshProUGUI>().text = PrestigeHandler.Instance.PrestigeTickrate.ToString("F2");

    }
    ////////////////////////////
    /// /// Notification Methods
    /// 
    public void ShowNotification(string message)
    {
        if (UINotificationText == null || UINotification == null)
        {
            Debug.LogError("UINotificationText or UINotification GameObject not found in the scene.");
            return;
        }
        UINotificationText.GetComponent<TextMeshProUGUI>().text = message;
        OpenNotification();
        // Close the notification after 4 seconds
        Invoke(nameof(CloseNotification), 4f);
    }
    void OpenNotification()
    {
        animatorNotification.SetTrigger("Open");
    }
    void CloseNotification()
    {
        animatorNotification.SetTrigger("Close");
    }
    /////////////////////// 
    /// Popup Methods
    ///     ///////////////////////////
    /// Popup Alert Functions
    public void ShowPopup(string message)
    {
        UIAlertText.GetComponent<TextMeshProUGUI>().text = message;
        UIAlert.SetActive(true);
    }
    public void HidePopup()
    {
        UIAlert.SetActive(false);
    }

    public Boolean IsPopupActive() => UIAlert.activeSelf;

}
