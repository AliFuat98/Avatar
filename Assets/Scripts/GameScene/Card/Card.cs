using System.Collections.Generic;
using UnityEngine;

public class Card {
  public string Word { get; private set; }
  public int PositionIndex { get; private set; }
  public Color Color { get; private set; }
  public HashSet<ulong> VoterClientIDlist { get; private set; }
  public bool IsOpen { get; private set; }

  public Card(string word, Color color) {
    Word = word.ToUpper();

    Color = color;
    VoterClientIDlist = new HashSet<ulong>();
    IsOpen = false;
  }

  public void SetPositionIndex(int index) {
    PositionIndex = index;
  }

  public bool CanVote(ulong clientId) {
    return !VoterClientIDlist.Contains(clientId);
  }

  public void AddNewVoter(ulong clientId) {
    VoterClientIDlist.Add(clientId);
  }

  public void DeleteVoter(ulong clientId) {
    VoterClientIDlist.Remove(clientId);
  }
}

public class BlueCard : Card {

  public BlueCard(string word, Color color) : base(word, color) {
  }
}

public class RedCard : Card {

  public RedCard(string word, Color color) : base(word, color) {
  }
}

public class BlackCard : Card {

  public BlackCard(string word, Color color) : base(word, color) {
  }
}

public class PurpleCard : Card {

  public PurpleCard(string word, Color color) : base(word, color) {
  }
}

public class EmptyCard : Card {

  public EmptyCard(string word, Color color) : base(word, color) {
  }
}