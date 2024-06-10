using System.Collections.Generic;

[System.Serializable]
public class CardData
{
    public string ID;   // 덱에 포함된 카드 아이디
    public int Num;     // 그 아이디의 갯수
}

[System.Serializable]
public class Deck
{
    public string DeckName; //저장된 덱의 이름
    public List<CardData> DeckData = new List<CardData>();
}


[System.Serializable]
public class TotalDeckList
{
    public List<Deck> DeckList = new List<Deck>();
}

[System.Serializable]
public class OwnCardList
{
    public List<CardData> ownCardList = new List<CardData>();
}