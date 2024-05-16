using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSingleUI : MonoBehaviour {
  [SerializeField] TextMeshProUGUI wordText;
  [SerializeField] Image CardBackground;

  [SerializeField] GameObject voterCountContainer;
  [SerializeField] TextMeshProUGUI voterCountText;

  [SerializeField] Button choooseCardButton;

  private Card card;

  private void Awake() {
    choooseCardButton.onClick.AddListener(() => {
      ChooseCard();
    });
  }

  public void InitilizeCard(Card card) {
    this.card = card;
    wordText.text = card.Word;

    // store the color
    card.SetCloseColor(CardBackground.color);

    //var playerData = GameMultiplayer.Instance.GetPlayerData();
    bool isteller = false;
    if (isteller) {
      ShowCard();
    }
  }

  private void ShowCard() {
    CardBackground.color = card.Color;
    wordText.color = Color.white;
    choooseCardButton.gameObject.SetActive(false);
    card.OpenCard();
    voterCountContainer.SetActive(false);
  }

  public void ChooseCard() {
    // sýra bizde deðilse return

    // seçebilir mi

    // show card
    CardManager.Instance.ChooseCard(card.PositionIndex);

    // give the point to the team
  }

  public void ClientChooseCard(ulong senderClientId) {
    // show the color of the card everyone
    ShowCard();

    // give the point to the team
    TurnManager.Instance.DecreaseCardCount();
  }

  public void Vote() {
    if (card.IsOpen) {
      return;
    }

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