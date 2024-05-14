using System;
using System.Collections.Generic;

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

  public static List<int> GetRandomIndices(int max) {
    List<int> indices = new(max);

    // Populate the list of indices
    for (int i = 0; i < max; i++) {
      indices.Add(i);
    }

    // Shuffle the indices
    for (int i = 0; i < max; i++) {
      int swapIndex = random.Next(i, max); // Fisher-Yates shuffle
      int temp = indices[i];
      indices[i] = indices[swapIndex];
      indices[swapIndex] = temp;
    }

    return indices;
  }
}