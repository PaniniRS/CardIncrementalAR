using UnityEngine;

[CreateAssetMenu(fileName = "ShopUpgrades", menuName = "Scriptable Objects/ShopUpgrades")]
public class ShopUpgrades : ScriptableObject
{
    public ShopHandler.Upgrade upgrade1;
    public ShopHandler.Upgrade upgrade2;
    public ShopHandler.Upgrade upgrade3;
    public ShopHandler.Upgrade upgrade4;
    private void OnEnable()
    {
        upgrade1 = new ShopHandler.Upgrade("Upgrade 1", 100, 1.05f);
        upgrade2 = new ShopHandler.Upgrade("Upgrade 2", 200, 1.1f, new ShopHandler.Upgrade[] { upgrade1 });
        upgrade3 = new ShopHandler.Upgrade("Upgrade 3", 300, 1.15f, new ShopHandler.Upgrade[] { upgrade2 });
        upgrade4 = new ShopHandler.Upgrade("Upgrade 4", 400, 1.2f, new ShopHandler.Upgrade[] { upgrade3 });        
    }
}
