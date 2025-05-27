using System.Collections;
using UnityEngine;


public abstract class CardPassiveIncome : MonoBehaviour
{
    public int TICKRATE_SECONDS;
    protected Coroutine routineForIncome;

    public void startEarning(int amount){
        if (routineForIncome == null){
            routineForIncome = StartCoroutine(earningCoroutine(amount));
        }
    }
    public void stopEarning(int amount){
        if (routineForIncome != null){
            StopCoroutine(routineForIncome);
            routineForIncome = null;
        }
    }

    protected abstract IEnumerator earningCoroutine(int amount);
}
