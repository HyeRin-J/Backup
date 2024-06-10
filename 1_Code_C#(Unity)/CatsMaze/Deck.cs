using System.Collections.Generic;

[System.Serializable]
public class CardData
{
    public string ID;   // ���� ���Ե� ī�� ���̵�
    public int Num;     // �� ���̵��� ����
}

[System.Serializable]
public class Deck
{
    public string DeckName; //����� ���� �̸�
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