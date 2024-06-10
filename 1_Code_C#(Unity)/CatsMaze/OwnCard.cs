using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using PolyAndCode.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class OwnCard : MonoBehaviour, ICell
{
    public UnitData unitData;
    public MagicCardData magicData;

    public List<Sprite> cardTypes;
    public Image cardType;
    public Image unitImage;

    [SerializeField] public LocalizeStringEvent nameLocalizedString;
    [SerializeField] TMP_Text cardCostText;
    [SerializeField] TMP_Text cardCost2Text;
    [SerializeField] TMP_Text cardNumberText;

    public int cardCount;

    public DeckUIManager uiManager;
    public Transform ownCardPanel;
    public RectTransform currentDeckPanel;

    private int _cellIndex;

    //This is called from the SetCell method in DataSource
    public void ConfigureCell(UnitData unitData, int cellIndex)
    {
        _cellIndex = cellIndex;
        this.unitData = unitData;
        cardType.sprite = cardTypes[(int)unitData.Type];
        cardType.gameObject.SetActive(true);
        unitImage.sprite = DataManager.Instance.GetUnitSprite(unitData.ID);
        nameLocalizedString.StringReference.TableEntryReference = unitData.ID;
        cardCostText.text = unitData.Erg.ToString();
        cardCost2Text.gameObject.SetActive(false);
    }
    //This is called from the SetCell method in DataSource
    public void ConfigureCell(MagicCardData magicData, int cellIndex)
    {
        _cellIndex = cellIndex;
        this.magicData = magicData;
        cardType.gameObject.SetActive(false);
        unitImage.sprite = DataManager.Instance.GetMagicCardSprite(magicData.cardID);
        nameLocalizedString.StringReference.TableEntryReference = magicData.cardID;
        cardCostText.text = magicData.Costvalue1.ToString();
        cardCost2Text.text = magicData.Costvalue2.ToString();
        cardCost2Text.gameObject.SetActive(true);
    }

    Vector3 originPos;

    public void SetCardCount(int count)
    {
        cardCount = count;
        cardNumberText.text = cardCount.ToString();
    }

    public void OnClick()
    {
        uiManager.ClickCard(this);
    }

    public void BeginDrag()
    {
        uiManager.isDrag = true;

        uiManager.ClickCard(this);

        if (cardCount == 0) return;

        originPos = GetComponent<RectTransform>().anchoredPosition;
        transform.SetParent(uiManager.transform);
        GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag()
    {
        uiManager.isDrag = true;

        if (cardCount == 0) return;

        transform.position = Input.mousePosition;
    }

    public void EndDrag()
    {
        uiManager.isDrag = false;

        if (cardCount == 0) return;

        GetComponent<Image>().raycastTarget = true;

        bool isAdd = false;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(currentDeckPanel, Input.mousePosition, null, out Vector2 localPoint))
        {
            isAdd = uiManager.AddCardToCurrentDeck();
        }

        if (isAdd)
        {
            cardCount--;
            cardNumberText.text = cardCount.ToString();
        }

        transform.SetParent(ownCardPanel);
        GetComponent<RectTransform>().anchoredPosition = originPos;

        uiManager.ownCardScrollView.Recycle();
    }
}
