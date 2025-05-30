using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationControler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animator animator;
    [Header("Animation Elements")]
    public GameObject shopPanel;

    void Start()
    {
        if (shopPanel == null)
        {
            Debug.LogError("Shop panel GameObject not assigned in AnimationController.");
        }
        animator = shopPanel.GetComponent<Animator>();
    }
    public void PlayOpenShopAnimation()
    {
        animator.SetBool("ShopButtonClicked", true);
    }
    public void PlayCloseShopAnimation()
    {
        animator.SetBool("ShopButtonClicked", false);
        StartCoroutine(WaitAndCloseShop());
    }

    private IEnumerator WaitAndCloseShop()
    {
        yield return new WaitForSeconds(0.5f); // Wait for the animation to finish
        shopPanel.SetActive(false);
    }
}
