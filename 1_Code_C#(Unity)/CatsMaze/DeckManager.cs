using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DeckManager : SingletonMonoBehaviour<DeckManager>
{
    public List<Deck> decks = new List<Deck>(4);

    public Deck currentSelectedDeck;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            if (initialized == false)
                DestroyImmediate(gameObject);
        }

        decks = UserData.Instance.totalDeckList.DeckList;
        currentSelectedDeck = decks[0];
    }

    public void SetCurrentDeck(int index)
    {
        currentSelectedDeck = decks[index];
    }

    public void SetCurrentSelectedDeck(int index)
    {
        currentSelectedDeck = decks[index];
    }

    public string GetDeckName(int i)
    {
        return decks[i].DeckName;
    }

    public int GetDeckCount(int i)
    {
        int count = 0;

        foreach (var card in decks[i].DeckData)
        {
            count += card.Num;
        }

        return count;
    }

    public void ResetCurrentDeck()
    {
        currentSelectedDeck.DeckData.Clear();
    }

    void ArrangeById()
    {
        currentSelectedDeck.DeckData.Sort((a, b) => { return a.ID.CompareTo(b.ID); });
    }

    public void RemoveCard(string ID)
    {
        for (int i = 0; i < currentSelectedDeck.DeckData.Count; ++i)
        {
            if (currentSelectedDeck.DeckData[i].ID == ID)
            {
                if (currentSelectedDeck.DeckData[i].Num > 1)
                {
                    currentSelectedDeck.DeckData[i].Num--;
                }
                else
                {
                    currentSelectedDeck.DeckData.RemoveAt(i);
                }
                break;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            UserData.Instance.totalDeckList.DeckList[i] = decks[i];
        }

        ArrangeById();
    }

    public bool AddCard(string ID)
    {
        if (currentSelectedDeck.DeckData.Count >= 40)
        {
            return false;
        }

        if (ID.Length <= 4)
            return false;

        int count = 0;
        int checkIndex = -1;

        for (int i = 0; i < currentSelectedDeck.DeckData.Count; i++)
        {
            if (ID.StartsWith("U"))
            {
                string idFront = ID.Substring(0, 4);
                int idBack = int.Parse(ID.Substring(4));

                if (currentSelectedDeck.DeckData[i].ID == ID)
                {
                    count += currentSelectedDeck.DeckData[i].Num;
                    checkIndex = i;
                }
                else if (currentSelectedDeck.DeckData[i].ID.Equals(idFront + (idBack > 4 ? idBack - 4 : idBack + 4)))
                {
                    count += currentSelectedDeck.DeckData[i].Num;
                }
            }
            else
            {
                if (currentSelectedDeck.DeckData[i].ID == ID)
                {
                    count += currentSelectedDeck.DeckData[i].Num;
                    checkIndex = i;
                }
            }

            if (count >= 4)
                return false;
        }

        if (checkIndex != -1)
        {
            currentSelectedDeck.DeckData[checkIndex].Num++;
        }
        else
        {
            CardData cardData = new CardData();
            cardData.ID = ID;
            cardData.Num = 1;
            currentSelectedDeck.DeckData.Add(cardData);

            ArrangeById();
        }

        for (int i = 0; i < 4; i++)
        {
            UserData.Instance.totalDeckList.DeckList[i] = decks[i];
        }

        return true;
    }
}
