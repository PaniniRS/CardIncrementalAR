using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using Unity.Mathematics;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR;
using Debug = UnityEngine.Debug;

public class GameHandler : MonoBehaviour
{

    public enum Suits
    {
        Club, Heart, Spade, Diamond, JokerBlack, JokerRed, Socials, Info, InfoRed, InfoBlue, CardBack
    }
    public enum Value
    {
        A = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        J = 11,
        Q = 12,
        K = 13
    }
    enum PokerHand
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }
    struct Card
    {
        public Suits suit;
        public Value value;

        public Card(Suits suit, Value value)
        {
            this.suit = suit;
            this.value = value;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

    public Animator animatorNotification;
    public GameObject UINotification;
    public GameObject UINotificationText;

    Coroutine passiveIncomeCoroutine;

    [Header("GameHandler Variables")]

    int cardsNextCost = 50;
    int cardSlots = 1;
    int cardsActive = 0;
    double income = 0;
    public float incomeMultiplier = 1f;
    public float incomeComboMultiplier = 1f;
    public float TICKRATE_SECONDS = 1f;
    readonly float TICKRATE_SECONDS_MIN = 0.2f;
    readonly float TICKRATE_SECONDS_MAX = 10f;
    readonly int CARDS_MAX_ACTIVE = 5;
    public double money = 10;

    /// ///////////////////////
    /// Stats
    long upgradesBought = 0;

    List<Card> hand = new List<Card>();
    PokerHand[] activeCombinations = new PokerHand[10];

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
    public void RemoveIncome(int amount)
    {
        double newIncome = income - Convert.ToDouble(amount);
        if (newIncome < 0) { newIncome = 0; }
        income = newIncome;
    }
    /// Setting a repeating loop that will increment money based on card drawn
    IEnumerator PassiveIncome()
    {
        while (true)
        {
            Debug.Log("Current Hand: " + string.Join(", ", hand.Select(card => $"{card.value} of {card.suit}")));
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

    //////////////////////////////////
    /// Poker Hand Combos
    /// 
    /// 
    /// 
    /// 
    /// 
    //Functions repeat since we cannot use enums or structs in Unity UI
    public void AddToHandUniversal(GameObject gameObject)
    {
        name = gameObject.name;
        Suits suit = ExtractSuitFromCardName(name); // Extract suit from the card name

        Value cardValue = (Value)Enum.Parse(typeof(Value), name.Substring(1)); // Extract value from name (e.g., "C2" -> 2)
        hand.Add(new Card(suit, cardValue)); // Add the card to the hand
        Debug.Log($"Added {cardValue} of {suit} to hand.");
        CheckPokerHand(); // Call the function to check poker hands after adding a card
        UpdateUI();
    }
    public void RemoveFromHandUniversal(GameObject gameObject)
    {
        // This function will remove a card from the hand based on the GameObject's name.
        // The name should contain the suit and value information.
        name = gameObject.name;
        Suits suit = ExtractSuitFromCardName(name); // Extract suit from the card name

        Value cardValue = (Value)Enum.Parse(typeof(Value), name.Substring(1)); // Extract value from name (e.g., "C2" -> 2
        // Remove the card from the hand | Where removes on false
        hand.RemoveAll(card => card.suit == suit && card.value == cardValue);
        Debug.Log($"Removed {cardValue} of {suit} from hand.");
        CheckPokerHand(); // Call the function to check poker hands after removing a card
        UpdateUI();
    }
    void CheckPokerHand()
    {
        incomeComboMultiplier = 1f; // Reset combo multiplier for each check
        // This function will check the current hand and update the activeCombinations array with the detected poker hands.
        //If the hand is empty, we can skip the check
        if (hand.Count != 0)
        {
            if (IsRoyalFlush())
            {
                activeCombinations[0] = PokerHand.RoyalFlush;
                incomeComboMultiplier *= 10f;
                //Do animation and notification
                ShowNotification("Royal Flush!");
                Debug.Log("Royal Flush detected!");

            }
            else if (IsStraightFlush())
            {
                activeCombinations[1] = PokerHand.StraightFlush;
                incomeComboMultiplier *= 8f;
                ShowNotification("Straight Flush !");
                Debug.Log("Straight Flush detected!");
            }
            else if (IsFourOfAKind())
            {
                activeCombinations[2] = PokerHand.FourOfAKind;
                incomeComboMultiplier *= 7f;
                ShowNotification("Four of a Kind !");
                Debug.Log("Four of a Kind detected!");
            }
            else if (IsFullHouse())
            {
                activeCombinations[3] = PokerHand.FullHouse;
                ShowNotification("Full House !");
                Debug.Log("Full House detected!");
                incomeComboMultiplier *= 3f;

            }
            else if (IsFlush())
            {
                activeCombinations[4] = PokerHand.Flush;
                ShowNotification("Flush!");
                Debug.Log("Flush detected!");
                incomeComboMultiplier *= 2.8f;

            }
            else if (IsStraight())
            {
                activeCombinations[5] = PokerHand.Straight;
                ShowNotification("Straight!");
                Debug.Log("Straight detected!");
                incomeComboMultiplier *= 2.5f;

            }
            else if (IsThreeOfAKind())
            {
                activeCombinations[6] = PokerHand.ThreeOfAKind;
                ShowNotification("Three of a Kind!");
                Debug.Log("Three of a Kind detected!");
                incomeComboMultiplier *= 2f;

            }
            else if (IsTwoPair())
            {
                activeCombinations[7] = PokerHand.TwoPair;
                ShowNotification("Two Pair!");
                Debug.Log("Two Pair detected!");
                incomeComboMultiplier *= 1.5f;

            }
            else if (IsOnePair())
            {
                activeCombinations[8] = PokerHand.OnePair;
                ShowNotification("One Pair!");
                Debug.Log("One Pair detected!");
                incomeComboMultiplier *= 1.2f;
            }
            else
            {
                activeCombinations[9] = PokerHand.HighCard;
                ShowNotification("High Card!");
                Debug.Log("High Card detected!");
            }
            // The logic for checking poker hands will be implemented here.
            // For now, we can just log a message indicating that this function is called.
            Debug.Log("Checking poker hand combinations...");
        }
    }

    Boolean IsOnePair()
    {
        // Check if there is exactly one pair in the hand
        var groupedByValue = hand.GroupBy(card => card.value);
        return groupedByValue.Count(g => g.Count() == 2) == 1;
    }
    Boolean IsTwoPair()
    {
        // Check if there are exactly two pairs in the hand
        var groupedByValue = hand.GroupBy(card => card.value);
        return groupedByValue.Count(g => g.Count() == 2) == 2;
    }
    Boolean IsThreeOfAKind()
    {
        // Check if there is exactly one three of a kind in the hand
        var groupedByValue = hand.GroupBy(card => card.value);
        return groupedByValue.Any(g => g.Count() == 3);
    }
    Boolean IsFourOfAKind()
    {
        // Check if there is exactly one four of a kind in the hand
        var groupedByValue = hand.GroupBy(card => card.value);
        return groupedByValue.Any(g => g.Count() == 4);
    }
    Boolean IsStraight()
    {
        // Check if the hand contains a straight (five consecutive values)
        var values = hand.Select(card => (int)card.value).Distinct().OrderBy(val => val).ToList();
        for (int i = 0; i <= values.Count - 5; i++)
        {
            //Check the difference between the first and fifth card in the sorted list, if its 4, we have a straight
            if (values[i + 4] - values[i] == 4)
            {
                return true;
            }
        }
        return false;
    }
    Boolean IsFlush()
    {
        // Check if all cards in the hand have the same suit
        var groupedBySuit = hand.GroupBy(card => card.suit);
        return groupedBySuit.Any(g => g.Count() == 5);
    }
    Boolean IsFullHouse()
    {
        return IsThreeOfAKind() && IsOnePair();
    }
    Boolean IsStraightFlush()
    {
        // Check if the hand is both a straight and a flush
        return IsStraight() && IsFlush();
    }
    Boolean IsRoyalFlush()
    {
        // Check if the hand is a straight flush with the highest value (Ace, King, Queen, Jack, Ten)
        var values = hand.Select(card => (int)card.value).Distinct().OrderBy(val => val).ToList();
        bool isRoyal = values.Contains((int)Value.A) &&
                       values.Contains((int)Value.K) &&
                       values.Contains((int)Value.Q) &&
                       values.Contains((int)Value.J) &&
                       values.Contains((int)Value.Ten);
        return IsStraightFlush() && isRoyal;
    }
    Suits ExtractSuitFromCardName(string cardName)
    {
        // Extract the suit from the card name
        if (cardName.Contains("H")) return Suits.Heart;
        if (cardName.Contains("S")) return Suits.Spade;
        if (cardName.Contains("D")) return Suits.Diamond;
        if (cardName.Contains("C")) return Suits.Club;
        if (cardName.Contains("JokerBlack")) return Suits.JokerBlack;
        if (cardName.Contains("JokerRed")) return Suits.JokerRed;
        if (cardName.Contains("Socials")) return Suits.Socials;
        if (cardName.Contains("Info")) return Suits.Info;
        if (cardName.Contains("InfoRed")) return Suits.InfoRed;
        if (cardName.Contains("InfoBlue")) return Suits.InfoBlue;

        return Suits.CardBack; // Default case
    }
}