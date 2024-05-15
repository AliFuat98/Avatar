using UnityEngine;

public class ChatManager : MonoBehaviour {
  public static ChatManager Instance { get; private set; }

  [SerializeField] private GameObject giveClueUI;

  private void Awake() {
    Instance = this;
  }

  public void OpenGiveClueUI(int teamId) {
    giveClueUI.SetActive(true);
    giveClueUI.GetComponent<GiveClueUI>().SetTeamId(teamId);
  }

  public void AddNewClue(int teamId, string clue) {
    Debug.Log($"teamid : {teamId} clue: {clue}");
  }
}