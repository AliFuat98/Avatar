using System.Collections;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour {
  public static MessageManager Instance { get; private set; }

  [SerializeField] private TextMeshProUGUI messageText;

  Coroutine lastCoroutine;

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    messageText.text = string.Empty;
  }

  public void SetText(string text) {
    if (lastCoroutine != null) {
      StopCoroutine(lastCoroutine);
    }

    lastCoroutine = StartCoroutine(SetTextCoroutine(text));
  }

  private IEnumerator SetTextCoroutine(string text) {
    messageText.text = text;
    yield return new WaitForSeconds(5);
    messageText.text = string.Empty;
  }
}