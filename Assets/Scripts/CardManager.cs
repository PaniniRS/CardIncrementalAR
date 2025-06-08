using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{

    public static CardManager Instance;
    [SerializeField] Animator CounterSlotAnimator;
    void Awake()
    {
        Instance = this;
    }

    public enum Suits
    {
        Club, Heart, Spade, Diamond, JokerBlack, JokerRed, Socials, Info, InfoRed, InfoBlue, CardBack
    }
    public enum Value
    {
        None = 0,
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
    public struct Card
    {
        public Suits suit;
        public Value value;

        public Card(Suits suit, Value value)
        {
            this.suit = suit;
            this.value = value;
        }
    }

    public List<Card> Hand { get; set; } = new List<Card>();
    public List<Card> Deck { get; set; } = new List<Card>();
    [SerializeField] public int HandCardSlots { get; set; } = 5;
    [SerializeField] public int CardDrawn { get; set; } = 0;
    [SerializeField] ShopUpgrade handCardSlotUpgrade;
    PokerHand[] activeCombinations = new PokerHand[10];
    ///////////////////////////////////
    /// Poker Hand Combos
    /// 
    /// 
    /// 
    /// 
    /// 
    //Functions repeat since we cannot use enums or structs in Unity UI
    public void AddToHandUniversal(GameObject gameObject)
    {
        Card card = ConvertGameObjToCard(gameObject);
        //If card is not in the deck check if we can add it, if not notify and exit fn.

        if (!CardIsInDeck(card))
        {
            if (CardDrawn < HandCardSlots) { AddCardToDeck(card); }
            else
            {
                GameHandler.Instance.ShowPopup("Cannot add more cards to the deck, please upgrade your Hand Card Slots.");
                Debug.LogWarning("Cannot add more cards to the deck, please upgrade your Hand Card Slots.");
                return;
            }
        }
        //Continues if we successfully added the card to our deck or card was already in deck
        AddCardToActiveSlot(card);
        //Activate card to generate income once it is in the active slot
        GameHandler.Instance.ActivateCard((int)card.value);
        // Debug.Log($"Activated {card.value} value of {card.suit} suit to ; Parsed value: {(int)card.value}");
        CheckPokerHand();
        GameHandler.Instance.UpdateUI();
    }
    public void RemoveFromHandUniversal(GameObject gameObject)
    {
        Card card = ConvertGameObjToCard(gameObject);
        // Remove the card from the Hand | Where removes on false
        Hand.RemoveAll(c => c.suit == card.suit && c.value == card.value);
        //Stop the card counting towards income
        GameHandler.Instance.DeactivateCard((int)card.value);
        Debug.Log($"Removed {card.value} of {card.suit} from Hand.");
        CheckPokerHand(); // Call the function to check poker Hands after removing a card

        GameHandler.Instance.UpdateUI();
    }

    void CheckPokerHand()
    {
        GameHandler.Instance.incomeComboMultiplier = 1f; // Reset combo multiplier for each check
        // This function will check the current Hand and update the activeCombinations array with the detected poker Hands.
        //If the Hand is empty, we can skip the check
        if (Hand.Count != 0)
        {
            if (IsRoyalFlush())
            {
                activeCombinations[0] = PokerHand.RoyalFlush;
                GameHandler.Instance.incomeComboMultiplier *= 10f;
                //Do animation and notification
                GameHandler.Instance.ShowNotification("Royal Flush!");
                Debug.Log("Royal Flush detected!");

            }
            else if (IsStraightFlush())
            {
                activeCombinations[1] = PokerHand.StraightFlush;
                GameHandler.Instance.incomeComboMultiplier *= 8f;
                GameHandler.Instance.ShowNotification("Straight Flush!");
                Debug.Log("Straight Flush detected!");
            }
            else if (IsFourOfAKind())
            {
                activeCombinations[2] = PokerHand.FourOfAKind;
                GameHandler.Instance.incomeComboMultiplier *= 7f;
                GameHandler.Instance.ShowNotification("Four of a Kind!");
                Debug.Log("Four of a Kind detected!");
            }
            else if (IsFullHouse())
            {
                activeCombinations[3] = PokerHand.FullHouse;
                GameHandler.Instance.ShowNotification("Full House!");
                Debug.Log("Full House detected!");
                GameHandler.Instance.incomeComboMultiplier *= 3f;

            }
            else if (IsFlush())
            {
                activeCombinations[4] = PokerHand.Flush;
                GameHandler.Instance.ShowNotification("Flush!");
                Debug.Log("Flush detected!");
                GameHandler.Instance.incomeComboMultiplier *= 2.8f;

            }
            else if (IsStraight())
            {
                activeCombinations[5] = PokerHand.Straight;
                GameHandler.Instance.ShowNotification("Straight!");
                Debug.Log("Straight detected!");
                GameHandler.Instance.incomeComboMultiplier *= 2.5f;

            }
            else if (IsThreeOfAKind())
            {
                activeCombinations[6] = PokerHand.ThreeOfAKind;
                GameHandler.Instance.ShowNotification("Three of a Kind!");
                Debug.Log("Three of a Kind detected!");
                GameHandler.Instance.incomeComboMultiplier *= 2f;

            }
            else if (IsTwoPair())
            {
                activeCombinations[7] = PokerHand.TwoPair;
                GameHandler.Instance.ShowNotification("Two Pair!");
                Debug.Log("Two Pair detected!");
                GameHandler.Instance.incomeComboMultiplier *= 1.5f;

            }
            else if (IsOnePair())
            {
                activeCombinations[8] = PokerHand.OnePair;
                GameHandler.Instance.ShowNotification("One Pair!");
                Debug.Log("One Pair detected!");
                GameHandler.Instance.incomeComboMultiplier *= 1.2f;
            }
            else
            {
                activeCombinations[9] = PokerHand.HighCard;
                GameHandler.Instance.ShowNotification("High Card!");
                Debug.Log("High Card detected!");
            }
            // The logic for checking poker Hands will be implemented here.
            // For now, we can just log a message indicating that this function is called.
            Debug.Log("Checking poker Hand combinations...");
        }
    }
    Boolean IsOnePair()
    {
        // Check if there is exactly one pair in the Hand
        var groupedByValue = Hand.GroupBy(card => card.value);
        return groupedByValue.Count(g => g.Count() == 2) == 1;
    }
    Boolean IsTwoPair()
    {
        // Check if there are exactly two pairs in the Hand
        var groupedByValue = Hand.GroupBy(card => card.value);
        return groupedByValue.Count(g => g.Count() == 2) == 2;
    }
    Boolean IsThreeOfAKind()
    {
        // Check if there is exactly one three of a kind in the Hand
        var groupedByValue = Hand.GroupBy(card => card.value);
        return groupedByValue.Any(g => g.Count() == 3);
    }
    Boolean IsFourOfAKind()
    {
        // Check if there is exactly one four of a kind in the Hand
        var groupedByValue = Hand.GroupBy(card => card.value);
        return groupedByValue.Any(g => g.Count() == 4);
    }
    Boolean IsStraight()
    {
        // Check if the Hand contains a straight (five consecutive values)
        var values = Hand.Select(card => (int)card.value).Distinct().OrderBy(val => val).ToList();
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
        // Check if all cards in the Hand have the same suit
        var groupedBySuit = Hand.GroupBy(card => card.suit);
        return groupedBySuit.Any(g => g.Count() == 5);
    }
    Boolean IsFullHouse()
    {
        return IsThreeOfAKind() && IsOnePair();
    }
    Boolean IsStraightFlush()
    {
        // Check if the Hand is both a straight and a flush
        return IsStraight() && IsFlush();
    }
    Boolean IsRoyalFlush()
    {
        // Check if the Hand is a straight flush with the highest value (Ace, King, Queen, Jack, Ten)
        var values = Hand.Select(card => (int)card.value).Distinct().OrderBy(val => val).ToList();
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
        if (cardName.Contains("JOKERBLACK")) return Suits.JokerBlack;
        if (cardName.Contains("JOKERRED")) return Suits.JokerRed;
        if (cardName.Contains("SOCIAL")) return Suits.Socials;
        if (cardName.Contains("INFO")) return Suits.Info;
        if (cardName.Contains("INFORED")) return Suits.InfoRed;
        if (cardName.Contains("INFOBLUE")) return Suits.InfoBlue;
        if (cardName.Contains("BACKCARD")) return Suits.CardBack;
        return Suits.CardBack; // Default case
    }
    ///////////////////////////////////
    /// Drawing Cards
    /// 
    bool CardIsInDeck(Card card) => Deck.Contains(card);
    void AddCardToDeck(Card card)
    {
        if (CardIsInDeck(card)) { GameHandler.Instance.ShowNotification("Drawn Card is already in the deck"); return; }
        Deck.Add(card);
        CardDrawn += 1;
        AnimationShakeCounterSlot();
        GameHandler.Instance.ShowNotification($"Added {card.value} of {card.suit} to Deck.");
    }
    void AddCardToActiveSlot(Card card)
    {
        Hand.Add(card);
        Debug.Log($"Added {card.value} of {card.suit} to Active Slot.");
    }
    public Card ConvertGameObjToCard(GameObject gameObject)
    {
        //Add card to active slot
        name = gameObject.name;
        Suits suit = ExtractSuitFromCardName(name); // Extract suit from the card name
        Value cardValue = name.Contains("A") ? Value.A :
                          name.Contains("2") ? Value.Two :
                          name.Contains("3") ? Value.Three :
                          name.Contains("4") ? Value.Four :
                          name.Contains("5") ? Value.Five :
                          name.Contains("6") ? Value.Six :
                          name.Contains("7") ? Value.Seven :
                          name.Contains("8") ? Value.Eight :
                          name.Contains("9") ? Value.Nine :
                          name.Contains("10") ? Value.Ten :
                          name.Contains("J") ? Value.J :
                          name.Contains("Q") ? Value.Q :
                          name.Contains("K") ? Value.K : Value.None;
        return new Card(suit, cardValue);
    }
    public void AnimationShakeCounterSlot() => CounterSlotAnimator?.SetTrigger("Shake");
}
