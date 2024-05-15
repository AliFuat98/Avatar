using UnityEngine;
using UnityEngine.EventSystems;

public class CloseUIOnClick : MonoBehaviour, IPointerClickHandler {
  [SerializeField] private GameObject closedUI;

  public void OnPointerClick(PointerEventData eventData) {
    closedUI.SetActive(false);
  }
}