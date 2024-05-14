using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSingleUI : MonoBehaviour {
  [SerializeField] TextMeshProUGUI wordText;
  [SerializeField] Image CardBackground;

  private Card card;

  public void SetCard(Card card) {
    this.card = card;
    wordText.text = card.Word;

    //var playerData = GameMultiplayer.Instance.GetPlayerData();
    bool isteller = true;
    if (isteller) {
      CardBackground.color = card.Color;
      wordText.color = Color.white;
    }
  }
}