using System;
using UnityEngine;

public class ShopUpgrade : MonoBehaviour
{

    [Header("Upgrade Properties")]
    public string nameUpgrade;
    public string descriptionUpgrade;
    public double currentPrice;
    public double startPrice;
    public int level;
    public float upgradeMultiplier;
    public int upgradeIncome;
    public bool isUpgrade = true;
    public bool useMultiplier = true;
    public bool useIncome = false;
    public GameHandler.Suits suit;

    [Header("UI Elements")]
    public GameObject labelPrice;
    public GameObject labelLevel;
    public GameObject labelName;
    public GameObject labelDescription;
    public GameObject upgradeButton;




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
    double CalculatePrice(int level)
    {
        // Price increases by 10% for each upgrade bought
        return Math.Round(currentPrice * Mathf.Pow(1.1f, level));
    }

    void ActivateUpgrade(float upgradeMultiplier)
    {
        if (useMultiplier) { GameHandler.Instance.AddMultiplier(upgradeMultiplier); return; }
        if (useIncome) { GameHandler.Instance.AddIncome(upgradeIncome); return; }
        Debug.LogWarning("Upgrade type not defined for: " + nameUpgrade);

    }
    void DeactivateUpgrade(float upgradeMultiplier)
    {
        if (useMultiplier) { GameHandler.Instance.RemoveMultiplier(upgradeMultiplier); return; }
        if (useIncome) { GameHandler.Instance.RemoveIncome(upgradeIncome); return; }
        Debug.LogWarning("Upgrade type not defined for: " + nameUpgrade);
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
            labelPrice.GetComponent<TMPro.TextMeshProUGUI>().text = GameHandler.Instance.FormatNotation(currentPrice);
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


}
