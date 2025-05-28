using System;
using UnityEngine;

public class ShopHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject canvas;
    public GameObject upgradePanel;
    public GameObject upgradeTemplate;
    struct Upgrade
    {
        String name;
        int price;
        float upgrade;
        Upgrade[] upgradeRequirements;

        public Upgrade(string name, int price, float upgrade, Upgrade[] upgradeRequirements)
        {
            this.name = name;
            this.price = price;
            this.upgrade = upgrade;
            this.upgradeRequirements = upgradeRequirements;
        }
        public Upgrade(string name, int price, float upgrade)
        {
            this.name = name;
            this.price = price;
            this.upgrade = upgrade;
            this.upgradeRequirements = new Upgrade[] {};
        }
    }

    /// ///////////////////////////////////////////////////////
    /// ///////////////////////////////////////////////////////
    /// ///////////////////////////////////////////////////////
    

    Upgrade[] UpgradesList = {
        new Upgrade("Upgrade 1", 100, 1.5f),
        new Upgrade("Upgrade 2", 200, 2.0f),
        new Upgrade("Upgrade 3", 300, 2.5f)
    };

/// ///////////////////////////////////////////////////////
/// ///////////////////////////////////////////////////////
/// ///////////////////////////////////////////////////////

    void Start()
    {

    }

    void addUpgradeElement()
    {
        
    }
    void removeUpgradeElement()
    {
        
    }

}
