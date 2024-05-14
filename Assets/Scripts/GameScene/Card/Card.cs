using System.Collections.Generic;
using UnityEngine;

public class Card {
  public string Word { get; private set; }
  public int PositionIndex { get; private set; }
  public Color Color { get; private set; }
  public List<ulong> VoterClientIDlist { get; private set; }
  public bool IsOpen { get; private set; }

  public Card(string word) {
    Word = word.ToLower();

    Color = Color.white; // give default color
    VoterClientIDlist = new List<ulong>();
    IsOpen = false;
  }

  public void SetPositionIndex(int index) {
    PositionIndex = index;
  }
}

public class BlueCard : Card {

  public BlueCard(string word) : base(word) {
  }
}

public class RedCard : Card {

  public RedCard(string word) : base(word) {
  }
}

public class BlackCard : Card {

  public BlackCard(string word) : base(word) {
  }
}

public class PurpleCard : Card {

  public PurpleCard(string word) : base(word) {
  }
}

public class EmptyCard : Card {

  public EmptyCard(string word) : base(word) {
  }
}