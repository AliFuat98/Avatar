using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardVoter : MonoBehaviour, IPointerClickHandler {

  public void OnPointerClick(PointerEventData eventData) {
    if (TurnManager.Instance.isGameOver) {
      return;
    }

    if (!TurnManager.Instance.IsMyTurn()) {
      MessageManager.Instance.SetText("it is not your turn for card vote");
      return;
    }

    CardSingleUI cardSingleUI = GetComponentInParent<CardSingleUI>();
    cardSingleUI.Vote();
  }
}