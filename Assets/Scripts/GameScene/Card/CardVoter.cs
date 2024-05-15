using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardVoter : MonoBehaviour, IPointerClickHandler {

  public void OnPointerClick(PointerEventData eventData) {
    CardSingleUI cardSingleUI = GetComponentInParent<CardSingleUI>();
    cardSingleUI.Vote();
  }
}