using System;
using UnityEngine;

public class ShopUpgrade : MonoBehaviour
{

    [Header("Upgrade Properties")]
    public string name;
    public int currentPrice;
    public int startPrice;
    public int level;
    public float upgradeMultiplier;

    [Header("UI Elements")]
    public GameObject labelPrice;
    public GameObject labelLevel;
    public GameObject labelName;



    // Update is called once per frame

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
        if (labelPrice != null && labelLevel != null)
        {
            Debug.Log("Updating UI for upgrade: " + name);
            labelPrice.GetComponent<TMPro.TextMeshProUGUI>().text = currentPrice.ToString();
            labelLevel.GetComponent<TMPro.TextMeshProUGUI>().text = level.ToString();
        }
        else
        {
            Debug.LogError("UI elements not assigned for upgrade: " + name);
        }
    }
}
