using TMPro;
using UnityEngine;

public class CardSingleUI : MonoBehaviour {
  [SerializeField] TextMeshProUGUI wordText;

  private Card card;

  public void SetCard(Card card) {
    this.card = card;
    wordText.text = card.Word;
  }
}