using System;
using UnityEditor;
using UnityEngine;

public class ShopUpgrade : MonoBehaviour
{
    private enum UpgradeType
    {
        TickRate,
        Multiplier,
        Income,
        ActiveCardSlot,
        DeckCardSlot,
        RandomChoose,
    }

    [Header("Upgrade Properties")]
    [SerializeField] string nameUpgrade;
    [SerializeField] string descriptionUpgrade;
    [SerializeField] double currentPrice;
    [SerializeField] double startPrice;
    [SerializeField] int level;
    [SerializeField] float upgradeMultiplier = 1.01f; // Default multiplier increase
    [SerializeField] int upgradeIncome = 1; // Default income increase;
    [SerializeField] float upgradeTickRate = 0.1f; // Default tick rate increase
    [SerializeField] CardManager.Suits suit;
    [SerializeField] UpgradeType upgradeType;
    [SerializeField] bool destroyOnPurchase = false;
    [SerializeField] bool isPrestige = false;

    [Header("UI Elements")]
    [SerializeField] GameObject labelPrice;
    [SerializeField] GameObject labelLevel;
    [SerializeField] GameObject labelName;
    [SerializeField] GameObject labelDescription;
    [SerializeField] GameObject upgradeButton;




    void Start()
    {
        if (labelName != null && labelPrice != null && labelLevel != null && labelDescription != null)
        {

            //Initialize the upgrade with the starting price and level
            if (upgradeType == UpgradeType.ActiveCardSlot || upgradeType == UpgradeType.DeckCardSlot)
            {
                currentPrice = GameHandler.Instance.CardsNextCost;
                if (upgradeType == UpgradeType.ActiveCardSlot) level = GameHandler.Instance.GetActiveCardSlots();
                else level = CardManager.Instance.HandCardSlots;
            }
            else
            {
                currentPrice = startPrice;
                level = 0;
            }
            //Update the UI with the initial values
            UpdateUpgradeUI();
            UIHandler.Instance.UpdateUI();
            // Add listener to the upgrade button
            upgradeButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(BuyClick);
            Debug.Log("ShopUpgrade initialized: " + name);
        }
        else
        {
            Debug.LogError("UI elements not assigned for upgrade: " + name);
        }
    }
    double CalculatePrice(int level)
    {
        // Price increases by 10% for each upgrade bought
        return Math.Round(currentPrice * Mathf.Pow(1.1f, level));
    }
    void ActivateUpgrade()
    {
        switch (upgradeType)
        {
            case UpgradeType.TickRate:
                GameHandler.Instance.ReduceTickRate(upgradeTickRate);
                break;
            case UpgradeType.Multiplier:
                GameHandler.Instance.AddMultiplier(upgradeMultiplier);
                break;
            case UpgradeType.Income:
                GameHandler.Instance.AddIncome(upgradeIncome);
                break;
            case UpgradeType.ActiveCardSlot:
                BuyActiveCardSlot();
                break;
            case UpgradeType.DeckCardSlot:
                CardManager.Instance.HandCardSlots++;
                AnimationHandler.Instance.AnimationShakeCounterSlot();
                break;
            case UpgradeType.RandomChoose:
                // Handle random choose upgrade logic here
                Debug.Log("Random choose upgrade activated: " + nameUpgrade);
                // Example: Select a random card or perform a specific action
                SpecialUpgradesHandler.Instance.SelectRandomUpgrades();
                break;
            default:
                Debug.LogWarning("Unknown upgrade type: " + upgradeType);
                break;
        }
    }
    void DeactivateUpgrade()
    {
        switch (upgradeType)
        {
            case UpgradeType.TickRate:
                GameHandler.Instance.IncreaseTickRate(upgradeTickRate);
                break;
            case UpgradeType.Multiplier:
                GameHandler.Instance.RemoveMultiplier(upgradeMultiplier);
                break;
            case UpgradeType.Income:
                GameHandler.Instance.RemoveIncome(upgradeIncome);
                break;
            case UpgradeType.ActiveCardSlot:
                GameHandler.Instance.DecActiveCardSlots();
                break;
            case UpgradeType.DeckCardSlot:
                CardManager.Instance.HandCardSlots--;
                break;
            default:
                Debug.LogWarning("Unknown upgrade type: " + upgradeType);
                break;
        }
        Debug.LogWarning("Upgrade type not defined for: " + nameUpgrade);
    }
    void BuyClick()
    {
        bool purchaseStatus = (isPrestige) ? PrestigeHandler.Instance.BuyFromPrestige(currentPrice) : GameHandler.Instance.BuyFromShop(currentPrice);
        if (purchaseStatus)
        {
            level++;
            currentPrice = CalculatePrice(level);
            ActivateUpgrade();
            Debug.Log("Upgrade purchased: " + name + " at level " + level + " for price " + currentPrice + "|Type: " + upgradeType);
            UpdateUpgradeUI();

            if (destroyOnPurchase)
            {
                Destroy(gameObject);
                Debug.Log("Upgrade destroyed after purchase: " + name);
            }
        }
    }
    void BuyActiveCardSlot()
    {
        GameHandler.Instance.IncActiveCardSlots();
        GameHandler.Instance.IncCardsNextCost();
        currentPrice = GameHandler.Instance.CardsNextCost;
        level = GameHandler.Instance.GetActiveCardSlots();
    }
    public void UpdateUpgradeUI()
    {
        if (labelPrice != null && labelLevel != null && labelDescription != null)
        {
            Debug.Log("Updating UI for upgrade: " + name);
            labelName.GetComponent<TMPro.TextMeshProUGUI>().text = nameUpgrade;
            labelPrice.GetComponent<TMPro.TextMeshProUGUI>().text = UIHandler.Instance.FormatNotation(currentPrice);
            labelLevel.GetComponent<TMPro.TextMeshProUGUI>().text = level.ToString();
            labelDescription.GetComponent<TMPro.TextMeshProUGUI>().text = descriptionUpgrade;
            UIHandler.Instance.UpdateUI();
        }
        else
        {
            Debug.LogError("UI elements not assigned for upgrade: " + name);
        }
    }


    //
    //
    //
    //
    //
    //


}
