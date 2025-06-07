using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR;
using Debug = UnityEngine.Debug;

public class GameHandler : MonoBehaviour
{
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Header("GameHandler Objects")]
    public static GameHandler Instance;

    [Header("UI Elements")]
    [SerializeField] GameObject UIMoneyValue;
    [SerializeField] GameObject UIIncomeValue;
    [SerializeField] GameObject UIMultiplierValue;
    [SerializeField] GameObject UITickrateValue;
    [SerializeField] GameObject UICardCountValue;
    [SerializeField] GameObject UINextCardValue;
    [SerializeField] GameObject UIAlert;
    [SerializeField] GameObject UIAlertText;

    [SerializeField] Animator animatorNotification;
    [SerializeField] GameObject UINotification;
    [SerializeField] GameObject UINotificationText;

    Coroutine passiveIncomeCoroutine;

    [Header("GameHandler Variables")]

    public int CardsNextCost { get; set; } = 50;
    int cardSlots = 1;
    int cardsActive = 0;
    double income = 0;
    public float incomeMultiplier = 1f;
    public float incomeComboMultiplier { get; set; } = 1f;
    public double citiesIncome = 0;
    public float TICKRATE_SECONDS = 1f;
    readonly float TICKRATE_SECONDS_MIN = 0.2f;
    readonly float TICKRATE_SECONDS_MAX = 10f;
    readonly int CARDS_MAX_ACTIVE = 5;
    public double money = 10;

    /// ///////////////////////
    /// Stats
    long upgradesBought { get; set; } = 0;


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //Awake runs before the scripts
    void Awake()
    {
        // By doing this we wouldn't have to open any other method or variable in the script to be static or globally available in unity
        Instance = this;

    }
    void Start()
    {
        InitUI();
        UINotification.SetActive(true);
    }
    void OnEnable()
    {
        InitNotificationAnimator();
    }
    //// UI FUNCTIONS
    //
    //
    //

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
    void InitUI()
    {
        InitBalance();
        InitMultiplier();
        InitTickrate();
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
    }
    private void InitTickrate()
    {
        if (UITickrateValue == null)
        {
            Debug.LogError("UITickrateValue GameObject not found in the scene.");
            return;
        }
        UITickrateValue.GetComponent<TextMeshProUGUI>().text = "/" + TICKRATE_SECONDS.ToString("F2") + "s";
    }
    void InitBalance()
    {
        if (UIMoneyValue == null)
        {
            Debug.LogError("UIMoneyValue GameObject not found in the scene.");
            return;
        }
        UIMoneyValue.GetComponent<TextMeshProUGUI>().text = FormatNotation(money);

    }
    void InitMultiplier()
    {
        if (UIMultiplierValue == null)
        {
            Debug.LogError("UIMultiplierValue GameObject not found in the scene.");
            return;
        }
        UIMultiplierValue.GetComponent<TextMeshProUGUI>().text = incomeMultiplier.ToString("F2");
    }
    public void UpdateUI()
    {
        // Update the UI text with the current money value
        if (UIMoneyValue != null && UIMultiplierValue != null)
        {
            UIMoneyValue.GetComponent<TextMeshProUGUI>().text = FormatNotation(money);
            UIMultiplierValue.GetComponent<TextMeshProUGUI>().text = (incomeMultiplier * incomeComboMultiplier).ToString("F2");
            UIIncomeValue.GetComponent<TextMeshProUGUI>().text = FormatNotation(income * incomeMultiplier * incomeComboMultiplier);
            UITickrateValue.GetComponent<TextMeshProUGUI>().text = TICKRATE_SECONDS.ToString("F2") + "/s";
        }
    }

    ////////////////////////
    /// Notifications

    public void ShowNotification(string message)
    {
        if (UINotificationText == null || UINotification == null)
        {
            Debug.LogError("UINotificationText or UINotification GameObject not found in the scene.");
            return;
        }
        UINotificationText.GetComponent<TextMeshProUGUI>().text = message;
        OpenNotification();
        // Close the notification after 3 seconds
        Invoke(nameof(CloseNotification), 3f);
    }
    void OpenNotification()
    {
        animatorNotification.SetTrigger("Open");
    }
    void CloseNotification()
    {
        animatorNotification.SetTrigger("Close");
    }
    //// MONEY HANDLING FUNCTIONS
    ///     
    /// 
    /// 

    /// Buying from shop
    public bool BuyFromShop(double price)
    {
        //Check if we have enough money
        if (money >= price)
        {
            RemoveMoney(price);
            IncUpgradeBoughtStat();
            return true;
        }
        else
        {
            Debug.Log("Not enough money to buy this item.");
            return false;
        }

    }
    void AddMoney(double amount)
    {
        money += amount;
        UpdateUI();

    }
    public void RemoveMoney(double amount)
    {
        double newMoney = money -= amount;
        //We cant go into negative balance values
        if (newMoney < 0)
        {
            newMoney = 0;
        }

        money = newMoney;
        UpdateUI();
    }
    public double GetMoney()
    {
        return money;
    }
    void SetMoney(double amount)
    {
        money = amount;
        UpdateUI();
    }
    void IncUpgradeBoughtStat() { upgradesBought += 1; }
    //Income Multiplier functions
    public float GetMultiplier()
    {
        return incomeMultiplier;
    }
    public void AddMultiplier(float value)
    {
        incomeMultiplier *= value;
    }
    public void RemoveMultiplier(float value)
    {
        float newIncMultiplier = incomeMultiplier / value;
        //Check for negative multiplier
        if (newIncMultiplier < 0) { newIncMultiplier = 1; }
        incomeMultiplier = newIncMultiplier;
    }
    // Functions for changing count of current active cards
    public void ActivateCard(int amount)
    {
        AddIncome(amount);
        IncActiveCard();
    }

    public void DeactivateCard(int amount)
    {
        RemoveIncome(amount);
        DecActiveCard();
    }
    public void IncActiveCard()
    {
        cardsActive += 1;
        SyncStartPassiveIncome();
    }
    public void DecActiveCard()
    {
        if (cardsActive != 0)
        {
            cardsActive -= 1;
            SyncStartPassiveIncome();
        }
    }

    // Functions for changing how much money is added every tick to the balance
    public void AddIncome(int amount)
    {
        income += Convert.ToDouble(amount);
    }
    public void AddIncomeDouble(double amount)
    {
        income += amount;
    }
    public void RemoveIncome(int amount)
    {
        double newIncome = income - Convert.ToDouble(amount);
        if (newIncome < 0) { newIncome = 0; }
        income = newIncome;
    }
    public void RemoveInwcomeDouble(double amount)
    {
        income -= amount;
    }
    /// Setting a repeating loop that will increment money based on card drawn
    IEnumerator PassiveIncome()
    {
        while (true)
        {
            Debug.Log("Current Hand: " + string.Join(", ", CardManager.instance.Hand.Select(card => $"{card.value} of {card.suit}")));
            //Checks whether we have more than the allowed amount of active cards
            if (cardsActive <= CARDS_MAX_ACTIVE && cardsActive <= cardSlots)
            {
                AddMoney(Convert.ToInt32(income * incomeMultiplier * incomeComboMultiplier));
                //Disable alert if its active for the case when we have more than the allowed amount of active cards
                if (UIAlert.activeSelf) { HideAlert(); }
            }
            else if (cardsActive > cardSlots) { AlertMoreActiveCards(); }


            UpdateUI();
            yield return new WaitForSeconds(TICKRATE_SECONDS);
        }
    }
    //Function is added so we can start coroutine from click or target added
    public void SyncStartPassiveIncome()
    {
        if (cardsActive > 0 && passiveIncomeCoroutine == null)
        {
            passiveIncomeCoroutine = StartCoroutine(PassiveIncome());
            Debug.Log("Starting PassiveIncome.");
        }
        else if (cardsActive == 0 && passiveIncomeCoroutine != null)
        {
            StopCoroutine(passiveIncomeCoroutine);
            passiveIncomeCoroutine = null;
            Debug.Log("Stopping PassiveIncome — no active cards.");
        }
        else if (cardsActive > 0 && passiveIncomeCoroutine != null)
        {
            // If we already have a coroutine running, we can just update the tick rate
            StopCoroutine(passiveIncomeCoroutine);
            passiveIncomeCoroutine = StartCoroutine(PassiveIncome());
            Debug.Log("Restarting PassiveIncome with new tick rate.");
        }
    }
    public void IncCardsNextCost()
    {
        CardsNextCost *= Convert.ToInt32(Math.Pow(10, Convert.ToDouble(cardsActive))); // Increment by 10 for example
        UINextCardValue.GetComponent<TextMeshProUGUI>().text = CardsNextCost.ToString();
    }
    public void DecCardsNextCost()
    {
        if (CardsNextCost > 0)
        {
            CardsNextCost /= Convert.ToInt32(Math.Pow(10, Convert.ToDouble(cardsActive))); // Increment by 10 for example
            UINextCardValue.GetComponent<TextMeshProUGUI>().text = CardsNextCost.ToString();
        }
    }
    public void IncCardSlots() { cardSlots += 1; }

    public void DecCardSlots() { if (cardSlots > 0) { cardSlots -= 1; } }

    public int GetCardSlots()
    {
        return cardSlots;
    }

    public void AlertMoreActiveCards()
    {
        // Once the number of active cards is below the limit, hide the alert
        ShowAlert("You can only have " + cardSlots + " active cards at a time.");
    }

    ///////////////////////////
    /// Popup Alert Functions
    public void ShowAlert(string message)
    {
        UIAlertText.GetComponent<TextMeshProUGUI>().text = message;
        UIAlert.SetActive(true);
    }
    public void HideAlert()
    {
        UIAlert.SetActive(false);
    }

    ////////////////////////// Functions for specialUpgrades
    //Random money adding function
    public void AddRandomMoney(float minInclusive, float maxInclusive)
    {
        // Generate a random amount of money between min and max
        int randomAmountMultiplier = UnityEngine.Random.Range((int)minInclusive, (int)maxInclusive + 1);

        AddMoney(Math.Round(money * randomAmountMultiplier));
        // Update the UI with the new money value
        UpdateUI();
    }

    //Temporary multiplier function
    IEnumerator TemporaryMultiplier(float value, float duration)
    {
        // Store the original multiplier
        float originalMultiplier = incomeMultiplier;
        // Apply the temporary multiplier
        incomeMultiplier *= value;
        UpdateUI();
        yield return new WaitForSeconds(duration);
        // Restore the original multiplier
        incomeMultiplier /= value;
        UpdateUI();
    }
    public void TemporarilyAddMultiplier(float value, float duration)
    {
        StartCoroutine(TemporaryMultiplier(value, duration));
    }

    //Randomly add or remove cash based on a random percentage of the current money
    public void RandomCashChange(float minPercentage, float maxPercentage)
    {
        // Generate a random percentage between the given range
        float randomPercentage = UnityEngine.Random.Range(minPercentage, maxPercentage);
        // Calculate the amount to add or subtract
        double amountChange = Math.Round(money * randomPercentage);
        // Add or subtract the amount from the current money
        if (amountChange < 0)
        {
            RemoveMoney(-amountChange); // If negative, remove money
        }
        else
        {
            AddMoney(amountChange); // If positive, add money
        }
        // Update the UI with the new money value
        UpdateUI();

    }
    public void ReduceTickRate(float value)
    {
        // Reduce the tick rate by the specified value
        TICKRATE_SECONDS -= value;
        // Ensure tick rate does not go below a minimum threshold
        if (TICKRATE_SECONDS < TICKRATE_SECONDS_MIN) { TICKRATE_SECONDS = TICKRATE_SECONDS_MIN; }
        // Ensure tick rate does not go above a maximum threshold
        if (TICKRATE_SECONDS > TICKRATE_SECONDS_MAX) { TICKRATE_SECONDS = TICKRATE_SECONDS_MAX; }
        // Restart the passive income coroutine with the new tick rate
        SyncStartPassiveIncome();
    }
    public void IncreaseTickRate(float value)
    {
        // Increase the tick rate by the specified value
        TICKRATE_SECONDS += value;
        // Ensure tick rate does not go below a minimum threshold
        if (TICKRATE_SECONDS < TICKRATE_SECONDS_MIN) { TICKRATE_SECONDS = TICKRATE_SECONDS_MIN; }
        // Ensure tick rate does not go above a maximum threshold
        if (TICKRATE_SECONDS > TICKRATE_SECONDS_MAX) { TICKRATE_SECONDS = TICKRATE_SECONDS_MAX; }
        // Restart the passive income coroutine with the new tick rate
        SyncStartPassiveIncome();
    }


}