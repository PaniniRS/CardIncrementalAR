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

    public static GameHandler Instance;
    public GameObject UIMoneyValue;
    Coroutine passiveIncomeCoroutine;

    // int upgradesBought = 0;
    int cardsActive = 0;
    int incrementalStep = 0;
    float incomeMultiplier = 1f;
    /////////////////////////////////////////////////////////////
    public float TICKRATE_SECONDS = 1f;
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
    //// MONEY HANDLING FUNCTIONS
    ///     
    /// 
    /// 


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

    void UpdateUI()
    {
        // Update the UI text with the current money value
        if (UIMoneyValue != null)
        {
            UIMoneyValue.GetComponent<TextMeshProUGUI>().text = money.ToString();
        }
    }

    //Income Multiplier functions
    public float getMulitplier()
    {
        return incomeMultiplier;
    }
        
        
    public void addMultiplier(float value)
    {
        incomeMultiplier += value;
    }

    public void removeMultiplier(float value)
    {
        float newIncMultiplier = incomeMultiplier - value;
        //Check for negative multiplier
        if (newIncMultiplier < 1) { newIncMultiplier = 1; }
        incomeMultiplier = newIncMultiplier;
    }
    

    // Functions for changing count of current active cards
    public void ActivateCard(int amount)
    {
        AddIncrementalStep(amount);
        IncActiveCard();
    }

    public void DeactivateCard(int amount)
    {
        RemoveIncrementalStep(amount);
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
    public void AddIncrementalStep(int amount)
    {
        incrementalStep += amount;
    }
    public void RemoveIncrementalStep(int amount)
    {
        int newIncrementalStep = incrementalStep - amount;
        if (newIncrementalStep < 0) { newIncrementalStep = 0; }
        incrementalStep = newIncrementalStep;
    }
    /// Setting a repeating loop that will increment money based on card drawn
    IEnumerator PassiveIncome()
    {
        while (true)
        {
            AddMoney(Convert.ToInt32(incrementalStep * incomeMultiplier));
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
}




}
