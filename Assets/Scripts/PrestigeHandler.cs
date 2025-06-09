using System;
using UnityEngine;

public class PrestigeHandler : MonoBehaviour
{
    [SerializeField] public int PrestigeTimes { get; private set; } = 1; // Number of times the player has prestiged
    [SerializeField] public int PrestigePoints { get; private set; } = 0;
    [SerializeField] public int PrestigeReward => CalculatePrestige(); // Points rewarded for presaging //TODO: Calculate this formula
    [SerializeField] public int PrestigeIncome { get; set; } = 0;
    [SerializeField] public float PrestigeMultiplier { get; set; } = 0;
    [SerializeField] public float PrestigeTickrate { get; set; } = 0;
    [Header("Prestige Elements")]
    [SerializeField] GameObject PrestigeRewardText;
    [SerializeField] GameObject PrestigePointsText;
    [SerializeField] GameObject PrestigeMultiplierText;
    [SerializeField] GameObject PrestigeIncomeText;
    [SerializeField] GameObject PrestigeTickrateText;

    public static PrestigeHandler Instance;

    void Awake()
    {
        Instance = this;
    }

    ///////////////////////
    /// 
    /// Prestige System Methods
    /// 

    ///  
    int CalculatePrestige()
    {
        //Get prestige cost from gamehandler upgrades bought * citiesbought and somethign else
        return (int)Math.Floor((double)(GameHandler.Instance.StatsUpgradesBought * WorldHandler.Instance.CitiesBought / 10 / PrestigeTimes)); // Example formula, adjust as needed
    }

    void Prestige()
    {
        if (PrestigePoints < 0)
        {
            UIHandler.Instance.ShowNotification("Not enough Prestige Points to prestige.");
            Debug.LogError("Not enough Prestige Points to prestige.");
            return;
        }
        //If can prestige, reset game state and add prestige points
        GameHandler.Instance.ResetGameVariables();
        PrestigePoints += PrestigeReward;
        PrestigeTimes++;
        //Notify user and update UI
        UIHandler.Instance.ShowPopup($"Prestiged {PrestigeTimes} times! You have {PrestigePoints} Prestige Points.");
        UIHandler.Instance.UpdateUI();

    }

    public bool BuyFromPrestige(double cost)
    {
        cost = Math.Round(cost); // Ensure cost is rounded to the nearest whole number
        if (PrestigePoints >= cost)
        {
            PrestigePoints -= (int)cost;
            return true; // Purchase successful
        }
        else
        {
            UIHandler.Instance.ShowNotification("Not enough Prestige Points to buy this upgrade.");
            Debug.LogError("Not enough Prestige Points to buy this upgrade.");
            return false; // Purchase failed
        }
    }
}
