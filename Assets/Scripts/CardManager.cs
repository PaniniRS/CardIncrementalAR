using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{

    public static CardManager instance;
    void Awake()
    {
        instance = this;
    }

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

    PokerHand[] activeCombinations = new PokerHand[10];

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
        Hand.Add(new Card(suit, cardValue)); // Add the card to the Hand
        Debug.Log($"Added {cardValue} of {suit} to Hand.");
        CheckPokerHand(); // Call the function to check poker Hands after adding a card
        GameHandler.Instance.UpdateUI();
    }

    public void RemoveFromHandUniversal(GameObject gameObject)
    {
        // This function will remove a card from the Hand based on the GameObject's name.
        // The name should contain the suit and value information.
        name = gameObject.name;
        Suits suit = ExtractSuitFromCardName(name); // Extract suit from the card name

        Value cardValue = (Value)Enum.Parse(typeof(Value), name.Substring(1)); // Extract value from name (e.g., "C2" -> 2
        // Remove the card from the Hand | Where removes on false
        Hand.RemoveAll(card => card.suit == suit && card.value == cardValue);
        Debug.Log($"Removed {cardValue} of {suit} from Hand.");
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
        if (cardName.Contains("JokerBlack")) return Suits.JokerBlack;
        if (cardName.Contains("JokerRed")) return Suits.JokerRed;
        if (cardName.Contains("Socials")) return Suits.Socials;
        if (cardName.Contains("Info")) return Suits.Info;
        if (cardName.Contains("InfoRed")) return Suits.InfoRed;
        if (cardName.Contains("InfoBlue")) return Suits.InfoBlue;

        return Suits.CardBack; // Default case
    }
}
