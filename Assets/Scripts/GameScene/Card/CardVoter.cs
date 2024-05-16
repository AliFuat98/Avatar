using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardVoter : MonoBehaviour, IPointerClickHandler {

  public void OnPointerClick(PointerEventData eventData) {
    try {
      // ali fuat
      if (!TurnManager.Instance.IsMyTurn()) {
        MessageManager.Instance.SetText("it is not your turn for card vote");
        return;
      }
    } catch (Exception) {
    }

    CardSingleUI cardSingleUI = GetComponentInParent<CardSingleUI>();
    cardSingleUI.Vote();
  }
}