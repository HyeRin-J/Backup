using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] Canvas backCanvas;
    [SerializeField] string sortingLayerName;
    int originOrder;

    public void SetOriginOrder(int originOrder)
    {
        this.originOrder = originOrder;
        SetOrder(originOrder);
    }

    public void SetMostFrontOrder(bool isMostFront)
    {
        SetOrder(isMostFront ? 100 : originOrder);
    }

    public void SetOrder(int order)
    {
        int mulOrder = order * 10;
        int index = 0;

        backCanvas.sortingLayerName = sortingLayerName;
        backCanvas.sortingOrder = mulOrder + ++index;
    }

}
