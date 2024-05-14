using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CardManager : NetworkBehaviour {
  public static CardManager Instance { get; private set; }

  [SerializeField] private int EmptyCardCount = 7;
  [SerializeField] private int RedCardCount = 8;
  [SerializeField] private int BlueCardCount = 7;
  [SerializeField] private int BlackCardCount = 1;
  [SerializeField] private int PurpleCardCount = 2;

  [SerializeField] private Transform CardContainer;
  [SerializeField] private GameObject CardPrefab;

  public List<Card> cardList;

  private NetworkList<int> randomCardIndexes;
  private NetworkList<int> randomCardWordIndexes;

  private void Awake() {
    Instance = this;
    randomCardIndexes = new();
    randomCardWordIndexes = new();
  }

  public void StartClient() {
    // test için
    NetworkManager.Singleton.StartClient();
  }

  public void StartHost() {
    // test için
    NetworkManager.Singleton.StartHost();
  }

  public override void OnNetworkSpawn() {
    Debug.Log("on network spawn");
    InitilizeCardList();
    ShuffleCards();
    AssignPositionIndexes();
    SpawnCardPrefab();
  }

  private void InitilizeCardList() {
    List<string> wordList = new() {
      "ali 0",
      "veli 0",
      "deli 0",
      "keli 0",
      "cali 0",
      "ali 1",
      "veli 1",
      "deli 1",
      "keli 1",
      "cali 1",
      "ali 2",
      "veli 2",
      "deli 2",
      "keli 2",
      "cali 2",
      "ali 3",
      "veli 3",
      "deli 3",
      "keli 3",
      "cali 3",
      "ali 4",
      "veli 4",
      "deli 4",
      "keli 4",
      "cali 4",
    };

    // chose 25 word
    if (IsServer) {
      var randomNumbers = ShuffleLogic.GetShuffleListNumbers(wordList.Count);
      foreach (var item in randomNumbers) {
        randomCardWordIndexes.Add(item);
      }

      wordList = ShuffleLogic.ShuffleList(wordList, randomNumbers);
    } else {
      List<int> randomNumberList = new();
      foreach (var index in randomCardWordIndexes) {
        randomNumberList.Add(index);
      }

      wordList = ShuffleLogic.ShuffleList(wordList, randomNumberList);
    }

    cardList = new List<Card>();
    int wordlistIndex = 0;
    for (int i = 0; i < EmptyCardCount; i++) {
      cardList.Add(new EmptyCard($"{wordList[wordlistIndex]}"));
      wordlistIndex++;
    }

    for (int i = 0; i < RedCardCount; i++) {
      cardList.Add(new RedCard($"{wordList[wordlistIndex]}"));
      wordlistIndex++;
    }

    for (int i = 0; i < BlueCardCount; i++) {
      cardList.Add(new BlueCard($"{wordList[wordlistIndex]}"));
      wordlistIndex++;
    }

    for (int i = 0; i < BlackCardCount; i++) {
      cardList.Add(new BlackCard($"{wordList[wordlistIndex]}"));
      wordlistIndex++;
    }
    for (int i = 0; i < PurpleCardCount; i++) {
      cardList.Add(new PurpleCard($"{wordList[wordlistIndex]}"));
      wordlistIndex++;
    }
  }

  private void ShuffleCards() {
    if (IsServer) {
      var randomNumbers = ShuffleLogic.GetShuffleListNumbers(cardList.Count);
      foreach (var item in randomNumbers) {
        randomCardIndexes.Add(item);
      }

      cardList = ShuffleLogic.ShuffleList(cardList, randomNumbers);
    } else {
      List<int> randomNumberList = new();
      foreach (var index in randomCardIndexes) {
        randomNumberList.Add(index);
      }

      cardList = ShuffleLogic.ShuffleList(cardList, randomNumberList);
    }
  }

  private void AssignPositionIndexes() {
    int index = 0;
    foreach (var item in cardList) {
      item.SetPositionIndex(index);
    }
  }

  private void SpawnCardPrefab() {
    foreach (var card in cardList) {
      GameObject cardPrefab = Instantiate(CardPrefab, CardContainer);
      cardPrefab.GetComponent<CardSingleUI>().SetCard(card);
    }
  }
}