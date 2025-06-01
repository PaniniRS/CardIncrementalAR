using System;
using UnityEngine;

public class ShopUpgrade : MonoBehaviour
{

    [Header("Upgrade Properties")]
    public string nameUpgrade;
    public string descriptionUpgrade;
    public int currentPrice;
    public int startPrice;
    public int level;
    public float upgradeMultiplier;
    public bool isUpgrade = true;

    [Header("UI Elements")]
    public GameObject labelPrice;
    public GameObject labelLevel;
    public GameObject labelName;
    public GameObject labelDescription;
    public GameObject upgradeButton;

    public struct Upgrade
    {
        public string name;
        public string description;
        public Action action;
        public Upgrade(string name, string description, Action action)
        {
            this.name = name;
            this.description = description;
            this.action = action;
        }
    }

    [Header("Upgrades")]
    Upgrade[] specialUpgrades;
    Upgrade cashBlast = new Upgrade("Cash Blast", "Increases your income by 10%", () => GameHandler.Instance.addMultiplier(1.1f));
    Upgrade cashBlast2 = new Upgrade("Cash Blast II", "Increases your income by 20%", () => GameHandler.Instance.addMultiplier(1.2f));
    Upgrade cashBlast3 = new Upgrade("Cash Blast III", "Increases your income by 30%", () => GameHandler.Instance.addMultiplier(1.3f));
    Upgrade doubleCash = new Upgrade("Double Cash", "Doubles your income for 30 seconds", () => GameHandler.Instance.TemporarilyAddMultiplier(2.0f, 30f));
    Upgrade tripleCash = new Upgrade("Triple Cash", "Triples your income for 30 seconds", () => GameHandler.Instance.TemporarilyAddMultiplier(3.0f, 30f));
    Upgrade cashInjection = new Upgrade("Cash Injection", "Gives you a one-time cash boost (between 20% and 120% of current income)", () => GameHandler.Instance.AddRandomMoney(0.2f, 1.2f));
    Upgrade giveOrTake = new Upgrade("Give or Take", "Randomly gives or takes a small amount of cash (between -10% and 10% of current income)", () => GameHandler.Instance.RandomCashChange(-0.1f, 0.1f));
    Upgrade giveOrTake2 = new Upgrade("Give or Take II", "Randomly gives or takes a small amount of cash (between -30% and 30% of current income)", () => GameHandler.Instance.RandomCashChange(-0.3f, 0.3f));
    Upgrade giveOrTake3 = new Upgrade("Give or Take III", "Randomly gives or takes a small amount of cash (between -50% and 50% of current income)", () => GameHandler.Instance.RandomCashChange(-0.5f, 0.5f));
    Upgrade tickRateReduce = new Upgrade("Tick Rate Reduction", "Reduces the tick rate by 0.1 seconds", () => GameHandler.Instance.ReduceTickRate(0.1f));
    Upgrade tickRateReduce2 = new Upgrade("Tick Rate Reduction II", "Reduces the tick rate by 0.2 seconds", () => GameHandler.Instance.ReduceTickRate(0.2f));
    Upgrade tickRateGiveOrTake = new Upgrade("Tick Rate Give or Take", "Randomly gives or takes a small amount of cash (between -10% and 10% of current income) and reduces the tick rate by 0.1 seconds", () =>
    {
        GameHandler.Instance.RandomCashChange(-0.1f, 0.1f);
        GameHandler.Instance.ReduceTickRate(0.1f);
    });
    Upgrade tickRateGamble = new Upgrade("Tick Rate Gamble", "Randomly increases or decreases the tick rate by 0.1 seconds", () =>
    {
        float randomChange = UnityEngine.Random.Range(-0.1f, 0.1f);
        GameHandler.Instance.ReduceTickRate(randomChange);
    });
    Upgrade tickRateGamble2 = new Upgrade("Tick Rate Gamble II", "Randomly increases or decreases the tick rate by 0.2 seconds", () =>
    {
        float randomChange = UnityEngine.Random.Range(-0.2f, 0.2f);
        GameHandler.Instance.ReduceTickRate(randomChange);
    });

    // Array of special upgrades to be used in the shop


    // Function to select a random upgrade from the special upgrades
    public Upgrade SelectRandomUpgrade()
    {
        int randomIndex = UnityEngine.Random.Range(0, specialUpgrades.Length);
        Upgrade selectedUpgrade = specialUpgrades[randomIndex];
        Debug.Log("Selected Upgrade: " + selectedUpgrade.name);
        return selectedUpgrade;
    }

    void Start()
    {
        if (labelName != null && labelPrice != null && labelLevel != null && labelDescription != null)
        {

            //Initialize the upgrade with the starting price and level
            if (isUpgrade)
            {
                currentPrice = startPrice;
                level = 0;
            }
            else
            {
                currentPrice = GameHandler.Instance.GetCardsNextCost();
                level = GameHandler.Instance.GetCardSlots();
            }
            //Update the UI with the initial values
            UpdateUpgradeUI();
            GameHandler.Instance.UpdateUI();
            // Add listener to the upgrade button
            upgradeButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(BuyClick);
            Debug.Log("ShopUpgrade initialized: " + name);
        }
        else
        {
            Debug.LogError("UI elements not assigned for upgrade: " + name);
        }
    }
    int CalculatePrice(int level)
    {
        // Price increases by 10% for each upgrade bought
        return Mathf.RoundToInt(currentPrice * Mathf.Pow(1.1f, level));
    }

    void ActivateUpgrade(float upgradeMultiplier)
    {
        GameHandler.Instance.addMultiplier(upgradeMultiplier);
    }
    void DeactivateUpgrade(float upgradeMultiplier)
    {
        GameHandler.Instance.removeMultiplier(upgradeMultiplier);
    }

    void BuyClick()
    {
        bool purchaseStatus = GameHandler.Instance.BuyFromShop(currentPrice);
        if (purchaseStatus)
        {
            level++;
            currentPrice = CalculatePrice(level);
            if (isUpgrade) ActivateUpgrade(upgradeMultiplier);
            else BuyCardSlot();
            Debug.Log("Upgrade purchased: " + name + " at level " + level + " for price " + currentPrice + "|isUpgrade: " + isUpgrade);
            UpdateUpgradeUI();
        }
    }

    void BuyCardSlot()
    {
        GameHandler.Instance.IncCardSlots();
        GameHandler.Instance.IncCardsNextCost();
        currentPrice = GameHandler.Instance.GetCardsNextCost();
        level = GameHandler.Instance.GetCardSlots();
        //Updateing UIs
        GameHandler.Instance.UpdateUI();
        UpdateUpgradeUI();
    }

    public void UpdateUpgradeUI()
    {
        if (labelPrice != null && labelLevel != null && labelDescription != null)
        {
            Debug.Log("Updating UI for upgrade: " + name);
            labelName.GetComponent<TMPro.TextMeshProUGUI>().text = nameUpgrade;
            labelPrice.GetComponent<TMPro.TextMeshProUGUI>().text = currentPrice.ToString();
            labelLevel.GetComponent<TMPro.TextMeshProUGUI>().text = level.ToString();
            labelDescription.GetComponent<TMPro.TextMeshProUGUI>().text = descriptionUpgrade;
            GameHandler.Instance.UpdateUI();
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


    public void SelectRandomUpgrade(Upgrade[] specialUpgrades)
    {

    }
}
