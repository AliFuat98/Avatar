using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSingleUI : MonoBehaviour {
  [SerializeField] TextMeshProUGUI wordText;
  [SerializeField] Image CardBackground;

  [SerializeField] GameObject voterCountContainer;
  [SerializeField] TextMeshProUGUI voterCountText;

  private Card card;

  public void SetCard(Card card) {
    this.card = card;
    //wordText.text = card.Word;
    wordText.text = card.PositionIndex.ToString();

    //var playerData = GameMultiplayer.Instance.GetPlayerData();
    bool isteller = true;
    if (isteller) {
      CardBackground.color = card.Color;
      wordText.color = Color.white;
    }
  }

  public void Vote() {
    // sýra bizde deðilse return

    CardManager.Instance.Vote(card.PositionIndex);
  }

  public void ClientVote(ulong senderClientId) {
    if (card.CanVote(senderClientId)) {
      card.AddNewVoter(senderClientId);
    } else {
      card.DeleteVoter(senderClientId);
    }

    int voterCount = card.VoterClientIDlist.Count;
    if (voterCount == 0) {
      voterCountContainer.SetActive(false);
    } else {
      voterCountContainer.SetActive(true);
      voterCountText.text = card.VoterClientIDlist.Count.ToString();
    }
  }

  public Card GetCard() {
    return card;
  }
}