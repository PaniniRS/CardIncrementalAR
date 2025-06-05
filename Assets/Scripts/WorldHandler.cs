using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class WorldHandler : MonoBehaviour
{
    //TODO: 5.6 -> Make a ecah of the cities have its own script then call the BuyCity method from the public WorldHandler class, when bought just destroy the object, this should make it easier to manage the cities and their properties.
    //TODO: 5.6 -> Make the cities have a description and a name, this should be displayed in the UI when the city is selected.
    public int citiesBought = 0;
    readonly public int MAX_CITIES = 6;

    public struct City
    {
        public string name;
        public string description;
        public double price;
        public double incomeBenefit;

        public City(string name, string desc, double price, double incomeBenefit)
        {
            this.name = name;
            this.description = desc;
            this.price = price;
            this.incomeBenefit = incomeBenefit;
        }
    }

    public enum Cities
    {
        London = 0,
        Paris = 1,
        NewYork = 2,
        Tokyo = 3,
        Sydney = 4,
        Berlin = 5,
    }

    City London = new City("London", "The capital city of England. 200k Cards get made here each day", 1_000_000, 50);
    City Paris = new City("Paris", "The capital city of France. 150k Cards get made here each day", 15_000_000, 30);
    City NewYork = new City("New York", "The capital city of the USA. 300k Cards get made here each day", 50_000_000, 60);
    City Tokyo = new City("Tokyo", "The capital city of Japan. 400k Cards get made here each day. Additionally has a state of the art card rating facility.", 100_000_000, 100);
    City Sydney = new City("Sydney", "The capital city of Australia. 350k Cards get made here each day", 400_000_000, 250);
    City Berlin = new City("Berlin", "The capital city of Germany. 400k Cards get made here each day", 1_000_000_000, 1000);
    City[] cities;

    void Awake()
    {
        cities = new City[] { London, Paris, NewYork, Tokyo, Sydney, Berlin };
    }
    void Start()
    {
        StartCoroutine("WonGame");
    }

    ////////////////////////////////////////////////////
    ////////////////////////////////////////////////////
    ////////////////////////////////////////////////////
    public void BuyCity(int cityIndex)
    {
        if (cityIndex < 0 || cityIndex >= cities.Length)
        {
            Debug.LogError("Invalid city index: " + cityIndex);
            return;
        }
        City city = cities[cityIndex];
        if (GameHandler.Instance.GetMoney() >= city.price)
        {
            GameHandler.Instance.RemoveMoney(city.price);
            GameHandler.Instance.AddIncomeDouble(city.incomeBenefit);
            GameHandler.Instance.ShowNotification("Successfully bought " + city.name + "!");
            Debug.Log("Bought city: " + city.name + " for " + city.price + " with income benefit of " + city.incomeBenefit + " Cities: " + citiesBought + "/" + MAX_CITIES);
            citiesBought++;
        }
        else
        {
            GameHandler.Instance.ShowNotification("Not enough money to buy " + city.name + ".");
        }
    }

    IEnumerable WonGame()
    {
        yield return new WaitUntil(() => citiesBought >= MAX_CITIES);
        GameHandler.Instance.ShowNotification("You won the game! Congratulations!");
    }
    void ResetCities()
    {
        citiesBought = 0;
        GameHandler.Instance.ShowNotification("Cities have been reset.");
    }


}
