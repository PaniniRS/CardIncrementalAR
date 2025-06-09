using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class SpecialUpgradesHandler : MonoBehaviour
{
    public static SpecialUpgradesHandler Instance;
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
    Upgrade cashBlast = new Upgrade("Cash Blast", "Increases your income by 10%", () => PrestigeHandler.Instance.PrestigeIncome = Convert.ToInt32(Math.Round(PrestigeHandler.Instance.PrestigeIncome * 0.1)));
    Upgrade cashBlast2 = new Upgrade("Cash Blast II", "Increases your income by 20%", () => PrestigeHandler.Instance.PrestigeIncome = Convert.ToInt32(Math.Round(PrestigeHandler.Instance.PrestigeIncome * 0.2)));
    Upgrade cashBlast3 = new Upgrade("Cash Blast III", "Increases your income by 30%", () => PrestigeHandler.Instance.PrestigeIncome = Convert.ToInt32(Math.Round(PrestigeHandler.Instance.PrestigeIncome * 0.3)));
    Upgrade doubleCash = new Upgrade("Double Cash", "Doubles your income for 5 minutes", () => GameHandler.Instance.TemporarilyAddMultiplier(2.0f, 300f));
    Upgrade tripleCash = new Upgrade("Triple Cash", "Triples your income for 5 minutes", () => GameHandler.Instance.TemporarilyAddMultiplier(3.0f, 300f));
    Upgrade tickRateReduce = new Upgrade("Tick Rate Reduction", "Reduces the tick rate by 0.1 seconds", () => GameHandler.Instance.ReduceTickRate(0.1f));
    Upgrade tickRateReduce2 = new Upgrade("Tick Rate Reduction II", "Reduces the tick rate by 0.2 seconds", () => GameHandler.Instance.ReduceTickRate(0.2f));
    Upgrade tickRateGamble = new Upgrade("Tick Rate Gamble", "Randomly increases or decreases the tick rate by 0.2 seconds", () =>
    {
        float randomChange = UnityEngine.Random.Range(-0.2f, 0.2f);
        PrestigeHandler.Instance.PrestigeTickrate += randomChange;
    });
    Upgrade multiplierIncrease = new Upgrade("Multiplier Increase", "Increases your multiplier by 0.1", () => PrestigeHandler.Instance.PrestigeMultiplier *= 0.1f);

    void Awake()
    {
        Instance = this;
        specialUpgrades = new Upgrade[]
        {
            cashBlast,
            cashBlast2,
            cashBlast3,
            doubleCash,
            tripleCash,
            tickRateReduce,
            tickRateReduce2,
            tickRateGamble,
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
                                UIHandler.Instance.UpdateUI();
                                specialUpgradePanel.SetActive(false);
                                Debug.Log("Upgrade applied: " + upgrade.name + " - " + upgrade.description);
                            });
            upgradeItem.gameObject.SetActive(true);
        }


    }
}