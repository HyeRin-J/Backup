using UnityEngine;

public class BaseMessage
{
    public string name;

    public BaseMessage()
    {
        name = GetType().Name;
    }
}

public class MapLoadFinishMessage : BaseMessage
{
    public int mapWidth, mapHeight;
    public Vector3Int startPos, endPos;

    public MapLoadFinishMessage(int width, int height, Vector3Int startPos, Vector3Int endPos)
    {
        mapWidth = width;
        mapHeight = height;

        this.startPos = startPos;
        this.endPos = endPos;
    }
}

public class AddTutorialValueMessage : BaseMessage
{
    public bool isReset = false;

    public AddTutorialValueMessage(bool isReset = false)
    {
        this.isReset = isReset;
    }
}

public class AddErgCostMessage : BaseMessage
{
    public int ergCost;

    public AddErgCostMessage(int ergCost)
    {
        this.ergCost = ergCost;
    }
}

public class ChangeWallOrDoorMessage : BaseMessage
{
    public bool isWall = true;

    public bool isDoor = false;

    public ChangeWallOrDoorMessage(bool isWall, bool isDoor)
    {
        this.isWall = isWall;
        this.isDoor = isDoor;
    }
}

public class SetInputCameraMessage : BaseMessage
{
    public Camera inputCamera;

    public SetInputCameraMessage(Camera inputCamera)
    {
        this.inputCamera = inputCamera;
    }
}

public class PhaseClearMessage : BaseMessage
{
    public bool isPhaseClear = true;

    public PhaseClearMessage(bool isPhaseClear = true)
    {
        this.isPhaseClear = isPhaseClear;

        if (isPhaseClear)
        {
            GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.페이즈클리어);
        }
        else
        {
            GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.스테이지클리어);
        }
    }
}

public class PhaseStartMessage : BaseMessage
{
    public int phase;

    public PhaseStartMessage(int phase)
    {
        this.phase = phase;
        GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.시작);
    }
}

public class StageFailMessage : BaseMessage
{
    public StageFailMessage()
    {
        GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.게임오버);
    }
}