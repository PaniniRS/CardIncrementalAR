using System;
using System.Collections;
using JetBrains.Annotations;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class GameHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("GameHandler Objects")]
    public static GameHandler Instance;

    [Header("UI Elements")]
    public GameObject UIMoneyValue;
    public GameObject UIIncomeValue;
    public GameObject UIMultiplierValue;
    public GameObject UITickrateValue;
    public GameObject UICardCountValue;
    public GameObject UINextCardValue;
    public GameObject UIAlert;
    public GameObject UIAlertText;

    Coroutine passiveIncomeCoroutine;

    [Header("GameHandler Variables")]

    int cardsNextCost = 50;
    int cardSlots = 1;
    int cardsActive = 0;
    int income = 0;
    public float incomeMultiplier = 1f;
    /////////////////////////////////////////////////////////////
    public float TICKRATE_SECONDS = 1f;
    float TICKRATE_SECONDS_MIN = 0.2f;
    float TICKRATE_SECONDS_MAX = 10f;
    int CARDS_MAX_ACTIVE = 5;

    public long money = 10;


    //Awake runs before the scripts
    void Awake()
    {
        // By doing this we wouldn't have to open any other method or variable in the script to be static or globally available in unity
        Instance = this;
    }
    void Start()
    {
        initUI();

    }

    //// UI FUNCTIONS
    //
    //
    //

    void initUI()
    {
        initBalance();
        initMultiplier();
        initTickrate();
    }

    private void initTickrate()
    {
        if (UITickrateValue == null)
        {
            Debug.LogError("UITickrateValue GameObject not found in the scene.");
            return;
        }
        UITickrateValue.GetComponent<TextMeshProUGUI>().text = "/" + TICKRATE_SECONDS.ToString("F2") + "s";
    }

    void initBalance()
    {
        if (UIMoneyValue == null)
        {
            Debug.LogError("UIMoneyValue GameObject not found in the scene.");
            return;
        }
        UIMoneyValue.GetComponent<TextMeshProUGUI>().text = money.ToString();

    }
    void initMultiplier()
    {
        if (UIMultiplierValue == null)
        {
            Debug.LogError("UIMultiplierValue GameObject not found in the scene.");
            return;
        }
        UIMultiplierValue.GetComponent<TextMeshProUGUI>().text = incomeMultiplier.ToString("F2");

    }
    //// MONEY HANDLING FUNCTIONS
    ///     
    /// 
    /// 

    /// Buying from shop
    public bool BuyFromShop(int price)
    {
        //Check if we have enough money
        if (money >= price)
        {
            RemoveMoney(price);
            return true;
        }
        else
        {
            Debug.Log("Not enough money to buy this item.");
            return false;
        }
    }

    void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();

    }
    public void RemoveMoney(int amount)
    {
        long newMoney = money -= amount;
        //We cant go into negative balance values
        if (newMoney < 0)
        {
            newMoney = 0;
        }

        money = newMoney;
        UpdateUI();
    }
    public long GetMoney()
    {
        return money;
    }
    void SetMoney(int amount)
    {
        money = amount;
        UpdateUI();
    }

    public void UpdateUI()
    {
        // Update the UI text with the current money value
        if (UIMoneyValue != null && UIMultiplierValue != null)
        {
            UIMoneyValue.GetComponent<TextMeshProUGUI>().text = money.ToString();
            UIMultiplierValue.GetComponent<TextMeshProUGUI>().text = incomeMultiplier.ToString("F2");
            UIIncomeValue.GetComponent<TextMeshProUGUI>().text = Convert.ToInt32(income * incomeMultiplier).ToString("");
            UITickrateValue.GetComponent<TextMeshProUGUI>().text = TICKRATE_SECONDS.ToString("F2") + "/s";
        }
    }

    //Income Multiplier functions

    public float getMultiplier()
    {
        return incomeMultiplier;
    }


    public void addMultiplier(float value)
    {
        incomeMultiplier *= value;
    }

    public void removeMultiplier(float value)
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
        income += amount;
    }
    public void RemoveIncome(int amount)
    {
        int newIncome = income - amount;
        if (newIncome < 0) { newIncome = 0; }
        income = newIncome;
    }
    /// Setting a repeating loop that will increment money based on card drawn
    IEnumerator PassiveIncome()
    {
        while (true)
        {

            //Checks whether we have more than the allowed amount of active cards
            if (cardsActive <= CARDS_MAX_ACTIVE && cardsActive <= cardSlots)
            {
                AddMoney(Convert.ToInt32(income * incomeMultiplier));
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

    ///////////////////////////
    /// Card Count Functions
    public int GetCardsNextCost()
    {
        return cardsNextCost;
    }
    public void SetCardsNextCost(int value)
    {
        cardsNextCost = value;
        if (UINextCardValue != null)
        {
            UINextCardValue.GetComponent<TextMeshProUGUI>().text = cardsNextCost.ToString();
        }
    }
    public void IncCardsNextCost()
    {
        cardsNextCost *= Convert.ToInt32(Math.Pow(10, Convert.ToDouble(cardsActive))); // Increment by 10 for example
        UINextCardValue.GetComponent<TextMeshProUGUI>().text = cardsNextCost.ToString();
    }
    public void DecCardsNextCost()
    {
        if (cardsNextCost > 0)
        {
            cardsNextCost /= Convert.ToInt32(Math.Pow(10, Convert.ToDouble(cardsActive))); // Increment by 10 for example
            UINextCardValue.GetComponent<TextMeshProUGUI>().text = cardsNextCost.ToString();
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

        AddMoney(Mathf.RoundToInt(money * randomAmountMultiplier));
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
        incomeMultiplier = originalMultiplier;
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
        int amountChange = Mathf.RoundToInt(money * randomPercentage);
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
