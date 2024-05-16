using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSingleUI : MonoBehaviour {
  [SerializeField] TextMeshProUGUI wordText;
  [SerializeField] Image CardBackground;

  [SerializeField] GameObject ChosenCardImage;

  [SerializeField] GameObject voterCountContainer;
  [SerializeField] TextMeshProUGUI voterCountText;

  [SerializeField] Button choooseCardButton;

  private Card card;

  private void Awake() {
    choooseCardButton.onClick.AddListener(() => {
      if (TurnManager.Instance.isGameOver) {
        return;
      }
      if (!TurnManager.Instance.IsMyTurn()) {
        MessageManager.Instance.SetText("it is not your turn for card vote");
        return;
      }

      if (card.IsOpen) {
        MessageManager.Instance.SetText("Card is already opened");
        return;
      }

      ChooseCard();

      TurnManager.Instance.OnTurnChanged += TurnManager_OnTurnChanged;
    });
  }

  private void TurnManager_OnTurnChanged(object sender, EventArgs e) {
    card.ResetVoter();
    voterCountContainer.SetActive(false);
  }

  public void InitilizeCard(Card card) {
    this.card = card;
    wordText.text = card.Word;

    // store the color
    card.SetCloseColor(CardBackground.color);

    var playerData = GameMultiplayer.Instance.GetPlayerData();
    if (playerData.isTeller) {
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
    CardManager.Instance.ChooseCard(card.PositionIndex);

    // end turn
    int teamId = TurnManager.Instance.IsFirstTeamTurn ? 0 : 2;
    int chosenCardTeamId = card.GetTeamId();

    if (chosenCardTeamId == -2) {
      // purple card chosen
      return;
    }

    if (teamId != chosenCardTeamId) {
      TurnManager.Instance.EndTurn();
    }
  }

  public void ClientChooseCard(ulong senderClientId) {
    ShowCard();
    card.ClientChoose();
    ChosenCardImage.SetActive(true);
  }

  public void Vote() {
    if (card.IsOpen) {
      return;
    }

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