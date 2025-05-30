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

    [Header("UI Elements")]
    public GameObject labelPrice;
    public GameObject labelLevel;
    public GameObject labelName;
    public GameObject labelDescription; 
    public GameObject upgradeButton;

    void Start()
    {
        if (labelName != null && labelPrice != null && labelLevel != null && labelDescription != null) {

            //Initialize the upgrade with the starting price and level
            currentPrice = startPrice;
            level = 0;
            //Update the UI with the initial values
            UpdateUpgradeUI();
            GameHandler.Instance.UpdateUI();
            // Add listener to the upgrade button
            upgradeButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(BuyClick);
            Debug.Log("ShopUpgrade initialized: " + name);
        } else {
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
                ActivateUpgrade(upgradeMultiplier);
                UpdateUpgradeUI();
            }
    }

    void UpdateUpgradeUI()
    {
        if (labelPrice != null && labelLevel != null && labelDescription != null)
        {
            Debug.Log("Updating UI for upgrade: " + name);
            labelName.GetComponent<TMPro.TextMeshProUGUI>().text = nameUpgrade;
            labelPrice.GetComponent<TMPro.TextMeshProUGUI>().text = currentPrice.ToString();
            labelLevel.GetComponent<TMPro.TextMeshProUGUI>().text = level.ToString();
            labelDescription.GetComponent<TMPro.TextMeshProUGUI>().text = descriptionUpgrade;
        }
        else
        {
            Debug.LogError("UI elements not assigned for upgrade: " + name);
        }
    }
}
