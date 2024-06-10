using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class DeckItem : MonoBehaviour, IPointerClickHandler
{
    public UnitData unitData;
    public MagicCardData magicData;

    public Sprite blankCardBack;
    public Sprite cardBack;
    public Sprite magiCardBack;

    public Image cardBackImage;
    public Button cardButton;

    public GameObject unitImageMask;
    public Image unitImage;
    public GameObject cardNameBackground;

    public Sprite unitCardNameSprite;
    public Sprite magicCardNameSprite;

    public TMPro.TMP_Text cardName;
    [SerializeField] LocalizeStringEvent nameLocalizedString;

    public DeckUIManager uiManager;

    public bool IsBlank = true;

    public void SetCard(UnitData unitData)
    {
        magicData = null;
        this.unitData = unitData;
        cardBackImage.sprite = cardBack;
        unitImage.sprite = DataManager.Instance.GetUnitSprite(unitData.ID);
        nameLocalizedString.StringReference.TableEntryReference = unitData.ID;

        cardNameBackground.GetComponent<Image>().sprite = unitCardNameSprite;
        cardNameBackground.SetActive(true);
        unitImageMask.SetActive(true);
        cardButton.interactable = true;

        IsBlank = false;
    }

    public void SetCard(MagicCardData magicData)
    {
        unitData = null;
        this.magicData = magicData;
        cardBackImage.sprite = magiCardBack;
        unitImage.sprite = DataManager.Instance.GetMagicCardSprite(magicData.cardID);
        nameLocalizedString.StringReference.TableEntryReference = magicData.cardID;

        cardNameBackground.GetComponent<Image>().sprite = magicCardNameSprite;
        cardNameBackground.SetActive(true);
        unitImageMask.SetActive(true);
        cardButton.interactable = true;

        IsBlank = false;
    }

    public void SetBlankCard()
    {
        if (IsBlank == true) return;

        cardBackImage.sprite = blankCardBack;
        cardNameBackground.SetActive(false);
        unitImageMask.SetActive(false);
        cardButton.interactable = false;

        IsBlank = true;
    }

    public void OnClick()
    {
        uiManager.ClickCard(this);

    }

    public void OnDrop()
    {
        if (!uiManager.isDrag) return;
        uiManager.isDrag = false;

        if (uiManager.selectedDataID.StartsWith("U"))
        {
            var unitData = DataManager.Instance.GetUnitData(uiManager.selectedDataID);

            if (unitData == null) return;

            if (IsBlank == false)
            {
                SetCard(unitData);
            }
        }
        else
        {
            var magicData = DataManager.Instance.GetMagicCardData(uiManager.selectedDataID);

            if (magicData == null) return;

            if (IsBlank == false)
            {
                SetCard(magicData);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardButton.interactable == false) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            uiManager.RemoveCard(this);

            SetBlankCard();

            transform.SetAsLastSibling();
        }
    }
}
