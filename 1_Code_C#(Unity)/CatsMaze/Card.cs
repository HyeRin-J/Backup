using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image character;
    [SerializeField] LocalizeStringEvent nameLocalizedString;
    [SerializeField] LocalizeStringEvent descriptionLocalizedString;
    [SerializeField] TMP_Text attack;
    [SerializeField] TMP_Text defense;
    [SerializeField] TMP_Text health;
    [SerializeField] TMP_Text cost;
    [SerializeField] Image cardTypeImage;
    [SerializeField] Image cardImageryImage;
    [SerializeField] Image cardBackImage;
    [SerializeField] Image cardNameBack;

    [SerializeField] Sprite unitCardImage;
    [SerializeField] Sprite magicCardImage;
    [SerializeField] Sprite unitCardNameSprite;
    [SerializeField] Sprite magicCardNameSprite;
    [SerializeField] Sprite[] cardTypeSprites;
    [SerializeField] Sprite[] cardImagerySprites;

    public UnitData unitData;
    public MagicCardData magicData;

    public PRS originPrs;

    public RectTransform rectTransform;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.cardManager.CardMouseOver(this);
    }

    void IPointerDownHandler.OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        GameManager.Instance.cardManager.CardMouseDown();
    }

    void IPointerUpHandler.OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        GameManager.Instance.cardManager.CardMouseUp();
    }

    void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        GameManager.Instance.cardManager.CardMouseExit(this);
    }
    public void SetUp(MagicCardData data)
    {
        this.magicData = data;

        character.sprite = DataManager.Instance.GetMagicCardSprite(data.cardID);
        nameLocalizedString.StringReference.TableEntryReference = data.cardID;
        descriptionLocalizedString.StringReference.TableEntryReference = data.cardID;
        cardBackImage.sprite = magicCardImage;
        cardNameBack.sprite = magicCardNameSprite;

        cardTypeImage.gameObject.SetActive(false);
        cardImageryImage.gameObject.SetActive(false);
        attack.gameObject.SetActive(false);
        defense.gameObject.SetActive(false);
        health.gameObject.SetActive(false);
        cost.gameObject.SetActive(false);
    }

    public void SetUp(UnitData data)
    {
        this.unitData = data;

        character.sprite = DataManager.Instance.GetUnitSprite(data.ID);
        nameLocalizedString.StringReference.TableEntryReference = data.ID;
        descriptionLocalizedString.StringReference.TableEntryReference = data.ID;
        cardBackImage.sprite = unitCardImage;
        cardNameBack.sprite = unitCardNameSprite;

        cardTypeImage.sprite = cardTypeSprites[(int)data.Type];
        cardImageryImage.sprite = cardImagerySprites[(int)data.Imagery];
        attack.text = data.Att.ToString();
        defense.text = data.Def.ToString();
        health.text = data.HP.ToString();
        cost.text = data.Erg.ToString();
    }

    public delegate void Func();

    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0, Func callBack = null)
    {
        if (useDotween)
        {
            rectTransform.DOMove(prs.position, dotweenTime).SetUpdate(true);
            rectTransform.DORotateQuaternion(prs.rotation, dotweenTime).SetUpdate(true);
            rectTransform.DOScale(prs.scale, dotweenTime).OnComplete(() => { callBack(); }).SetUpdate(true);
        }
        else
        {
            rectTransform.DOMove(prs.position, 0).SetUpdate(true);
            rectTransform.DORotateQuaternion(prs.rotation, 0).SetUpdate(true);
            rectTransform.DOScale(prs.scale, 0).SetUpdate(true);
        }
    }

    private void OnDestroy()
    {
        rectTransform.DOKill(false);
    }
}
