using TMPro;
using UnityEngine;

public class CityScript : MonoBehaviour
{

    [Header("City UI")]
    public GameObject popupPanel;
    public GameObject titleText;
    public GameObject descriptionText;
    public GameObject priceText;

    [Header("City Properties")]
    public string cityName;
    public string cityDescription;
    public double cityPrice;
    public int cityIncomeBenefit;

    private void Start()
    {
        // Initialize the city properties
        if (string.IsNullOrEmpty(cityName) || string.IsNullOrEmpty(cityDescription))
        {
            Debug.LogError("City name or description is not set for " + gameObject.name);
        }
        if (cityPrice <= 0)
        {
            Debug.LogError("City price must be greater than zero for " + cityName);
        }
        if (cityIncomeBenefit < 0)
        {
            Debug.LogError("City income benefit cannot be negative for " + cityName);
        }

        InitPopup();
        popupPanel.transform.parent.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(BuyCity);
    }
    public void BuyCity()
    {
        // Check if the player can afford the city
        if (GameHandler.Instance.GetMoney() >= cityPrice)
        {
            // Deduct the price from the player's money
            GameHandler.Instance.RemoveMoney(cityPrice);
            // Increase the income by the income benefit of the city
            GameHandler.Instance.AddIncome(cityIncomeBenefit);
            // Increment the number of cities bought
            WorldHandler.Instance.citiesBought++;
            GameHandler.Instance.UpdateUI();
            // Show notification for successful purchase
            GameHandler.Instance.ShowNotification("You bought " + cityName + " for " + GameHandler.Instance.FormatNotation(cityPrice));
            Debug.Log("You bought " + cityName + " for " + GameHandler.Instance.FormatNotation(cityPrice));
            // Destroy this city object
            Destroy(gameObject);
        }
        else
        {
            GameHandler.Instance.ShowNotification("Not enough money to buy " + cityName);
            Debug.Log("Not enough money to buy " + cityName);
        }
    }

    void OnMouseEnter()
    {
        // show popup with city name and description
        ShowPopup();
    }
    void OnMouseExit()
    {
        // hide popup
        HidePopup();
    }
    void ShowPopup()
    {
        popupPanel.SetActive(true);
    }
    void HidePopup()
    {
        // Implement the logic to hide the popup
        popupPanel.SetActive(false);
    }
    void InitPopup()
    {
        titleText.GetComponent<TextMeshProUGUI>().text = cityName;
        descriptionText.GetComponent<TextMeshProUGUI>().text = cityDescription;
        priceText.GetComponent<TextMeshProUGUI>().text = GameHandler.Instance.FormatNotation(cityPrice);
    }

}
