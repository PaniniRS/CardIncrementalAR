using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DeckCardVisualization : MonoBehaviour
{

    [SerializeField] private GameObject cardBackPrefab;
    [SerializeField] private GameObject cardFrontPrefab;
    [SerializeField] private CardManager.Card thisCard;

    void Start()
    {
        if (cardBackPrefab == null || cardFrontPrefab == null)
        {
            Debug.LogError("Card prefabs not found in the DeckCardVisualization object.");
            return;
        }
        {
            thisCard = CardManager.Instance.ConvertGameObjToCard(gameObject);
            StartCoroutine(CardIsInDeck());
            CardHide();
        }

        IEnumerator CardIsInDeck()
        {
            // Wait until the card is in the deck
            yield return new WaitUntil(() => CardManager.Instance.Deck.Contains(thisCard));

            // Once the card is in the deck, show it
            Debug.Log("Card is now in the deck.");
            CardShow();
        }
        void CardShow()
        {
            cardBackPrefab.SetActive(false);
            cardFrontPrefab.SetActive(true);
        }
        void CardHide()
        {
            cardBackPrefab.SetActive(true);
            cardFrontPrefab.SetActive(false);
        }
    }
}
