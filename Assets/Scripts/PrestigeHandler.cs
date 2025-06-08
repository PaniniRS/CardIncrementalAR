using UnityEngine;

public class PrestigeHandler : MonoBehaviour
{
    [SerializeField] int prestigePoints = 0;
    [SerializeField] int prestigeCost = 1000; // Cost to prestige, can be adjusted
    [SerializeField] int prestigeReward = 10; // Points rewarded for prestiging

    public static PrestigeHandler Instance;
    void Awake()
    {
        Instance = this;
    }



    ///////////////////////
    /// 
    /// Prestige System Methods
    /// 

    public void Prestige()
    {
        if (prestigePoints >= prestigeCost)
        {
            prestigePoints -= prestigeCost;
            prestigeCost = Mathf.RoundToInt(prestigeCost * 1.5f); // Increase cost for next prestige
            prestigePoints += prestigeReward; // Reward points for prestiging

            // Reset game variables
            GameHandler.Instance.ResetGameVariables(); // Assuming this method resets necessary game variables
            CardManager.Instance.ResetCards(); // Assuming this method resets cards


            // Feedback
            Debug.Log($"Prestiged! New Prestige Points: {prestigePoints}, Next Cost: {prestigeCost}");
            //Maybe trigger some UI update or animation here
        }
        else
        {
            UIHandler.Instance.ShowNotification("Not enough Prestige Points to prestige.");
            Debug.Log("Not enough Prestige Points to prestige.");
        }
    }
}
