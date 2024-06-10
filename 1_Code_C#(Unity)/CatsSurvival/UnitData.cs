using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CharacterData
{
    public string name;

    public int attDmg;
    public float moveSpeed;
    public float attackSpeed;
    public float attRange = 3;

    public int lifeMax = 300;
}

public class UnitData : MonoBehaviour
{
    public GameObject hpBar;
    public GameObject hpBarBack;

    CharacterData characterData;

    private int life = 300;

    void Init(CharacterData data)
    {
        characterData = data;
        life = data.lifeMax;
    }


}
