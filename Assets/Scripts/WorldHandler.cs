using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class WorldHandler : MonoBehaviour
{
    public static WorldHandler Instance;

    public int citiesBought = 0;

    readonly public int MAX_CITIES = 6;

    void Start()
    {
        StartCoroutine("WonGame");
    }

    ////////////////////////////////////////////////////
    ////////////////////////////////////////////////////
    ////////////////////////////////////////////////////
    private void Awake()
    {
        Instance = this;
    }
    ////////////////////////////////////////////////////
    ////////////////////////////////////////////////////
    ////////////////////////////////////////////////////
    IEnumerable WonGame()
    {
        yield return new WaitUntil(() => citiesBought >= MAX_CITIES);
        UIHandler.Instance.ShowNotification("You won the game! Congratulations!");
    }
    void ResetCities()
    {
        citiesBought = 0;
        UIHandler.Instance.ShowNotification("Cities have been reset.");
    }
}
