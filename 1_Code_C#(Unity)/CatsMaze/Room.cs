using System;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Imagery imagery;

    public List<RoomTile> tiles;
    public List<Unit> units;
    public List<Monster> monsters;
    public List<RoomTile> walls;
    public List<RoomTile> doors;

    public GameObject statusPanel;

    public int cardSlot;

    public bool isPossibleEdit = true;

    Dictionary<string, UnitArea_Imagery> unit_area_data = new();
    Dictionary<string, AreaArea_Imagery> area_area_data = new();

    private void Awake()
    {
        MessageQueue.Instance.AttachListener(typeof(PhaseClearMessage), ReviveAllUnits);
    }

    private void OnDestroy()
    {
        MessageQueue.Instance.DetachListener(typeof(PhaseClearMessage), ReviveAllUnits);
    }

    public Room()
    {
        tiles = new List<RoomTile>();
        units = new List<Unit>();
        monsters = new List<Monster>();
        walls = new List<RoomTile>();
        doors = new List<RoomTile>();

        isPossibleEdit = true;
    }

    public Room(Room room)
    {
        tiles = new List<RoomTile>();
        units = new List<Unit>();
        walls = new List<RoomTile>();
        doors = new List<RoomTile>();

        foreach (var tile in room.tiles)
        {
            tiles.Add(tile);
        }
        foreach (var unit in room.units)
        {
            units.Add(unit);
        }
        foreach (var wall in room.walls)
        {
            walls.Add(wall);
        }
        foreach (var door in room.doors)
        {
            doors.Add(door);
        }

        cardSlot = room.cardSlot;
        isPossibleEdit = true;
    }

    public void AddSynergy(UnitArea_Imagery synergy)
    {
        if (!unit_area_data.ContainsKey(synergy.ID))
            unit_area_data.Add(synergy.ID, synergy);
    }

    public void AddSynergy(AreaArea_Imagery synergy)
    {
        if (!area_area_data.ContainsKey(synergy.ID))
            area_area_data.Add(synergy.ID, synergy);
    }

    public void CheckSynergy()
    {
        //합연산은 모든 비율을 더한 값에 1을 더 해주는 것. ex) 10% + 10% = 1.2

        //곱연산은 모든 비율을 1을 각각 더한후 곱해주는 것.ex) 10 % * 10 % = 1.21

        var synList = GameManager.Instance.synergyChecker.CheckRoomSynergy(this);

        foreach (var syn in synList)
        {
            AddSynergy(syn);
        }
    }

    public void RemoveSynergy(string id)
    {
        if (area_area_data.ContainsKey(id))
            area_area_data.Remove(id);

        if (unit_area_data.ContainsKey(id))
            unit_area_data.Remove(id);
    }

    public void Init()
    {
        StatusPanel comp = statusPanel.GetComponent<StatusPanel>();
        comp.SetRoom(this);
        cardSlot = tiles.Count / 3;
        comp.SetStatusPanel(0, 0, 0, 0, 0, 0, 0, cardSlot);
        comp.F1Panel.SetActive(true);

        CheckSynergy();
    }

    public void Destroy()
    {
        foreach (var tile in tiles)
        {
            tile.room?.Remove(this);
        }
        foreach (var door in doors)
        {
            door.room?.Remove(this);
        }
        foreach (var wall in walls)
        {
            wall.room?.Remove(this);
        }
    }

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
        StatusPanel comp = statusPanel.GetComponent<StatusPanel>();
        comp.SetStatusPanel(0, 0, 0, 0, 0, 0, 0, cardSlot);

        isPossibleEdit = false;

        //var synList = GameManager.Instance.synergyChecker.CheckUnitSynergy(unit);

        //foreach (var syn in synList)
        //{
            //AddSynergy(syn);
        //}
    }

    public void RemoveUnit(int index)
    {
        GameObject.Destroy(units[index].gameObject);

        units.RemoveAt(index);

        cardSlot += 1;

        if (units.Count == 0) isPossibleEdit = true;
        else isPossibleEdit = false;

        statusPanel.GetComponent<StatusPanel>().SetStatusPanel(0, 0, 0, 0, 0, 0, 0, cardSlot);
    }

    public void AddMonster(Monster monster)
    {
        monsters.Add(monster);

    }

    public void RemoveMonster(Monster monster)
    {
        monsters.Remove(monster);
    }

    public bool ReviveAllUnits(BaseMessage msg)
    {
        if (msg == null)
        {
            Debug.Log("HandleYourMesssageListener : Message is null!");
            return false;
        }

        PhaseClearMessage castMsg = msg as PhaseClearMessage;

        if (castMsg == null)
        {
            Debug.Log("HandleYourMesssageListener : Cast Message is null!");
            return false;
        }

        Debug.Log(string.Format("HandleYourMesssageListener : Got the message! - {0}", castMsg.name));

        for (int i = 0; i < units.Count; ++i)
        {
            units[i].Revive();
        }

        return true;
    }


    public int RemoveAllUnits()
    {
        int count = units.Count;

        cardSlot += units.Count;

        foreach (var unit in units)
        {
            GameObject.Destroy(unit.gameObject);
        }

        units.Clear();

        isPossibleEdit = true;

        statusPanel.GetComponent<StatusPanel>().SetStatusPanel(0, 0, 0, 0, 0, 0, 0, cardSlot);

        return count;
    }

    public Unit TargetUnit()
    {
        if (units.Count == 0) return null;

        int index = -1;

        //가장 처음 살아있고, 어그로가 풀이지 않은 탱커인 유닛을 타겟으로 잡는다
        for (int i = 0; i < units.Count; i++)
        {
            if (!units[i].isDead &&
                (units[i].unitData.Type == UnitType.재담 ||
                units[i].unitData.Type == UnitType.괴담 ||
                units[i].unitData.Type == UnitType.치유)
                && !units[i].isTargetFull)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            //탱커가 없으면 가장 처음 살아있는 유닛을 타겟으로 잡는다
            for (int i = 0; i < units.Count; i++)
            {
                if (!units[i].isDead && !units[i].isTargetFull)
                {
                    index = i;
                    break;
                }
            }
        }

        return index == -1 ? null : units[index];
    }
}