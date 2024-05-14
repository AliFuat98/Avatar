using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShuffleLogic {
  private static System.Random random = new System.Random();

  public static List<int> GetShuffleListNumbers(int listLenght) {
    List<int> randomIndexes = new();
    for (int i = listLenght - 1; i > 0; i--) {
      int j = random.Next(0, i + 1);
      randomIndexes.Add(j);
    }
    return randomIndexes;
  }

  public static List<T> ShuffleList<T>(List<T> lst, List<int> indexes) {
    List<T> shuffled = new(lst);

    int n = shuffled.Count;
    int randomNumberIndex = 0;
    for (int i = n - 1; i > 0; i--) {
      int j = indexes[randomNumberIndex];
      randomNumberIndex++;

      T temp = shuffled[i];
      shuffled[i] = shuffled[j];
      shuffled[j] = temp;
    }
    return shuffled;
  }
}