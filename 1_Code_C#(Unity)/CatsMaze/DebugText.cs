using TMPro;
using UnityEngine;

public class DebugText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text arrow;

    [SerializeField]
    private TMP_Text G;

    [SerializeField]
    private TMP_Text H;

    [SerializeField]
    private TMP_Text F;

    [SerializeField]
    private TMP_Text roomID;

    public TMP_Text MyArrow { get => arrow; set => arrow = value; }
    public TMP_Text MyG { get => G; set => G = value; }
    public TMP_Text MyH { get => H; set => H = value; }
    public TMP_Text MyF { get => F; set => F = value; }
    public TMP_Text MyRoomID { get => roomID; set => roomID = value; }

}
