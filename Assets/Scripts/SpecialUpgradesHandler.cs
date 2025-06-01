using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class SpecialUpgradesHandler : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject specialUpgradesContainer;
    public GameObject specialUpgradeItemPrefab;
    public GameObject specialUpgradePanel;
    /// <summary>
    /// Defines a structure for special upgrades in the game.
    /// </summary>
    /// 
    public struct Upgrade
    {
        public string name;
        public string description;
        public Action action;
        public Sprite image;

        public Upgrade(string name, string description, Action action)
        {
            this.name = name;
            this.description = description;
            this.action = action;
            this.image = null; // Default to null if no image is provided
        }
        public Upgrade(string name, string description, Action action, Sprite image)
        {
            this.name = name;
            this.description = description;
            this.action = action;
            this.image = image;
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

    void Awake()
    {
        specialUpgrades = new Upgrade[]
        {
            cashBlast,
            cashBlast2,
            cashBlast3,
            doubleCash,
            tripleCash,
            cashInjection,
            giveOrTake,
            giveOrTake2,
            giveOrTake3,
            tickRateReduce,
            tickRateReduce2,
            tickRateGiveOrTake,
            tickRateGamble,
            tickRateGamble2
        };
    }

    void Start()
    {
        if (specialUpgradeItemPrefab == null || specialUpgradesContainer == null || specialUpgradePanel == null)
        {
            Debug.LogError("Special Upgrade UI elements are not assigned.");
        }
    }
    // Function to select a random upgrade from the special upgrades
    Upgrade SelectRandomUpgrade()
    {
        int randomIndex = UnityEngine.Random.Range(0, specialUpgrades.Length);
        Upgrade selectedUpgrade = specialUpgrades[randomIndex];
        Debug.Log("Selected Upgrade: " + selectedUpgrade.name);
        return selectedUpgrade;
    }


    public void SelectRandomUpgrades()
    {
        // Ensure the special upgrade panel is active
        specialUpgradePanel.SetActive(true);
        // Array to hold the selected upgrades
        Upgrade[] selectedUpgrades = new Upgrade[3];
        //Selecting 3 random upgrades from the special upgrades
        for (int i = 0; i < selectedUpgrades.Length; i++)
        {
            Upgrade temporaryUpgrade = SelectRandomUpgrade();
            // Ensure the selected upgrade wont repeat
            while (Array.Exists(selectedUpgrades, upgrade => upgrade.name == temporaryUpgrade.name))
            {
                temporaryUpgrade = SelectRandomUpgrade();
            }
            selectedUpgrades[i] = temporaryUpgrade;
            Debug.Log("Selected Upgrade " + (i + 1) + ": " + temporaryUpgrade.name);
        }
        // Clear the container before adding new upgrades
        foreach (Transform child in specialUpgradesContainer.transform)
        {
            Destroy(child.gameObject);
        }
        // Instantiate the selected upgrades in the UI

        foreach (Upgrade upgrade in selectedUpgrades)
        {
            // GameObject upgradeItem = Instantiate(specialUpgradeItemPrefab, specialUpgradesContainer.transform);
            GameObject upgradeItem = Instantiate(specialUpgradeItemPrefab, specialUpgradesContainer.transform);
            // Set the upgrade item properties
            upgradeItem.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = upgrade.name;
            upgradeItem.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = upgrade.image;
            upgradeItem.transform.Find("Description").GetComponent<TMPro.TextMeshProUGUI>().text = upgrade.description;
            upgradeItem.transform.Find("Button").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                            {
                                upgrade.action.Invoke();
                                GameHandler.Instance.UpdateUI();
                                Destroy(upgradeItem);
                                specialUpgradePanel.SetActive(false);
                                Debug.Log("Upgrade applied: " + upgrade.name + " - " + upgrade.description);
                            });
            upgradeItem.gameObject.SetActive(true);
        }


    }
}