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

    Coroutine passiveIncomeCoroutine;

    [Header("GameHandler Variables")]

    public int CardsNextCost { get; set; } = 50;
    int activeCardSlots = 1;
    int cardsActive = 0;
    public double income = 0;
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
    public int StatsUpgradesBought { get; set; } = 0;


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
        UIHandler.Instance.ShowNotification("Game Started");
    }

    //// UI FUNCTIONS
    //
    //
    //



    ////////////////////////
    /// Notifications

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
            StatsUpgradesBought += 1;
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
        UIHandler.Instance.UpdateUI();

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
        UIHandler.Instance.UpdateUI();
    }
    public double GetMoney()
    {
        return money;
    }
    void SetMoney(double amount)
    {
        money = amount;
        UIHandler.Instance.UpdateUI();
    }
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
    public void RemoveIncomeDouble(double amount)
    {
        income -= amount;
    }
    /// Setting a repeating loop that will increment money based on card drawn
    IEnumerator PassiveIncome()
    {
        while (true)
        {
            Debug.Log("Current Hand: " + string.Join(", ", CardManager.Instance.Hand.Select(card => $"{card.value} of {card.suit}")));
            //Checks whether we have more than the allowed amount of active cards
            if (cardsActive <= CARDS_MAX_ACTIVE && cardsActive <= activeCardSlots)
            {
                AddMoney(Convert.ToInt32((income + PrestigeHandler.Instance.PrestigeIncome) * incomeMultiplier * PrestigeHandler.Instance.PrestigeMultiplier * incomeComboMultiplier));
                //Disable alert if its active for the case when we have more than the allowed amount of active cards
                if (UIHandler.Instance.IsPopupActive()) { UIHandler.Instance.HidePopup(); }
            }
            else if (cardsActive > activeCardSlots) { AlertMoreActiveCards(); }


            UIHandler.Instance.UpdateUI();
            yield return new WaitForSeconds(TICKRATE_SECONDS - PrestigeHandler.Instance.PrestigeTickrate);
            // If the tick rate is below the minimum, wait for the minimum tick rate
            if (TICKRATE_SECONDS - PrestigeHandler.Instance.PrestigeTickrate < TICKRATE_SECONDS_MIN)
            {
                yield return new WaitForSeconds(TICKRATE_SECONDS_MIN);
            }
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
        UIHandler.Instance.UpdateUINextCardValue();
    }
    public void DecCardsNextCost()
    {
        if (CardsNextCost > 0)
        {
            CardsNextCost /= Convert.ToInt32(Math.Pow(10, Convert.ToDouble(cardsActive))); // Increment by 10 for example
            UIHandler.Instance.UpdateUINextCardValue();
        }
    }
    public void IncActiveCardSlots() { activeCardSlots += 1; }
    public void DecActiveCardSlots() { if (activeCardSlots > 0) { activeCardSlots -= 1; } }
    public int GetActiveCardSlots()
    {
        return activeCardSlots;
    }
    public void AlertMoreActiveCards()
    {
        // Once the number of active cards is below the limit, hide the alert
        UIHandler.Instance.ShowPopup("You can only have " + activeCardSlots + " active cards at a time.");
    }


    ////////////////////////// Functions for specialUpgrades
    //Random money adding function
    public void AddRandomMoney(float minInclusive, float maxInclusive)
    {
        // Generate a random amount of money between min and max
        int randomAmountMultiplier = UnityEngine.Random.Range((int)minInclusive, (int)maxInclusive + 1);

        AddMoney(Math.Round(money * randomAmountMultiplier));
        // Update the UI with the new money value
        UIHandler.Instance.UpdateUI();
    }

    //Temporary multiplier function
    IEnumerator TemporaryMultiplier(float value, float duration)
    {
        // Store the original multiplier
        float originalMultiplier = incomeMultiplier;
        // Apply the temporary multiplier
        incomeMultiplier *= value;
        UIHandler.Instance.UpdateUI();
        yield return new WaitForSeconds(duration);
        // Restore the original multiplier
        incomeMultiplier /= value;
        UIHandler.Instance.UpdateUI();
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
        UIHandler.Instance.UpdateUI();

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


    ///////////////////////
    ///////////////////////
    ///////////////////////
    ///////////////////////
    ///////////////////////
    /// Prestige System
    public void ResetGameVariables()
    {
        // Reset game variables to their initial state
        money = 10;
        income = 0;
        incomeMultiplier = 1f;
        incomeComboMultiplier = 1f;
        cardsActive = 0;
        activeCardSlots = 1;
        CardsNextCost = 50;
        TICKRATE_SECONDS = 1f;

        // Reset the UI
        UIHandler.Instance.UpdateUI();
    }
}