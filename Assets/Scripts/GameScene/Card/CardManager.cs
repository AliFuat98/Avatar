using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CardManager : NetworkBehaviour {
  public static CardManager Instance { get; private set; }

  [SerializeField] private int emptyCardCount = 7;
  [SerializeField] private int redCardCount = 8;
  [SerializeField] private int blueCardCount = 7;
  [SerializeField] private int blackCardCount = 1;
  [SerializeField] private int purpleCardCount = 2;

  [SerializeField] private Color emptyCardColor;
  [SerializeField] private Color redCardColor;
  [SerializeField] private Color blueCardColor;
  [SerializeField] private Color blackCardColor;
  [SerializeField] private Color purpleCardColor;

  [SerializeField] private Transform cardContainer;
  [SerializeField] private GameObject cardPrefab;

  public List<Card> cardList;
  private List<string> wordList;
  private int totalCardCount = 25;

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
    ReadWordListFromFile();
    ChooseWordIndices();
    InitilizeCardList();
    ShuffleCards();
    AssignPositionIndexes();
    SpawnCardPrefab();
  }

  private void ReadWordListFromFile() {
    TextAsset wordData = Resources.Load<TextAsset>("words");
    if (wordData != null) {
      wordList = new List<string>(wordData.text.Split('\n'));
    }
  }

  private void ChooseWordIndices() {
    // chose 25 word
    if (IsServer) {
      var randomNumbers = ShuffleLogic.GetRandomIndices(wordList.Count);

      // get the first 25 of the indicies
      for (int i = 0; i < totalCardCount; i++) {
        randomCardWordIndexes.Add(randomNumbers[i]);
      }
    }
  }

  private void InitilizeCardList() {
    cardList = new List<Card>();
    int wordlistIndex = 0;
    for (int i = 0; i < emptyCardCount; i++) {
      int index = randomCardWordIndexes[wordlistIndex];
      cardList.Add(new EmptyCard($"{wordList[index]}", emptyCardColor));
      wordlistIndex++;
    }

    for (int i = 0; i < redCardCount; i++) {
      int index = randomCardWordIndexes[wordlistIndex];
      cardList.Add(new RedCard($"{wordList[index]}", redCardColor));
      wordlistIndex++;
    }

    for (int i = 0; i < blueCardCount; i++) {
      int index = randomCardWordIndexes[wordlistIndex];
      cardList.Add(new BlueCard($"{wordList[index]}", blueCardColor));
      wordlistIndex++;
    }

    for (int i = 0; i < blackCardCount; i++) {
      int index = randomCardWordIndexes[wordlistIndex];
      cardList.Add(new BlackCard($"{wordList[index]}", blackCardColor));
      wordlistIndex++;
    }
    for (int i = 0; i < purpleCardCount; i++) {
      int index = randomCardWordIndexes[wordlistIndex];
      cardList.Add(new PurpleCard($"{wordList[index]}", purpleCardColor));
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
      GameObject cardGameObject = Instantiate(cardPrefab, cardContainer);
      cardGameObject.GetComponent<CardSingleUI>().SetCard(card);
    }
  }
}