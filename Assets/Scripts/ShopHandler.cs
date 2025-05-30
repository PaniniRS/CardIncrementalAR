using System;
using UnityEngine;

public class ShopHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static ShopHandler Instance;

    public GameObject canvas;
    public GameObject upgradePanel;
    public GameObject upgradeTemplate;
    public struct Upgrade
    {
        String name;
        int price;
        int startPrice;
        int level;
        float upgrade;
        Upgrade[] upgradeRequirements;

        public Upgrade(string name, int price, float upgrade, Upgrade[] upgradeRequirements)
        {
            this.name = name;
            this.price = price;
            this.upgrade = upgrade;
            this.upgradeRequirements = upgradeRequirements;
        }
        public Upgrade(string name, int price, int startPrice, float upgrade)
        {
            this.name = name;
            this.startPrice = startPrice;
            this.price = price;
            this.level = 0;
            this.upgrade = upgrade;
            this.upgradeRequirements = new Upgrade[] { };
        }

        public float GetUpgrade() { return upgrade; }
        public string GetName() { return name; }
        public int GetPrice() { return price; }
        public Upgrade[] GetUpgradeRequirements() { return upgradeRequirements; }
        public int CalculatePrice(int upgradesBought)
        {

            return (int)(price * Mathf.Pow(upgradePriceMultiplier, level));
        }

    }

    //Awake runs before the scripts
    void Awake()
    {
        // By doing this we wouldn't have to open any other method or variable in the script to be static or globally available in unity
        Instance = this;
    }
    /// ///////////////////////////////////////////////////////
    /// ///////////////////////////////////////////////////////
    /// ///////////////////////////////////////////////////////




    /// ///////////////////////////////////////////////////////
    /// ///////////////////////////////////////////////////////
    /// ///////////////////////////////////////////////////////

    void Start()
    {
        // Initialize the upgrade panel
    }

    void ActivateUpgrade(Upgrade upgrade)
    {
        GameHandler.Instance.addMultiplier(upgrade.GetUpgrade());
    }
    void DeactivateUpgrade(Upgrade upgrade)
    {
        GameHandler.Instance.removeMultiplier(upgrade.GetUpgrade());
    }

    void addUpgradeElement(Upgrade upgrade)
    {
        if (upgradePanel == null || upgradeTemplate == null)
        {
            Debug.LogError("Upgrade panel or template is not set.");
            return;
        }
        // Instantiate the upgrade template


    }
}
