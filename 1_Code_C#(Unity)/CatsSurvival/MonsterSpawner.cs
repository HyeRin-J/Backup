using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Redcode.Pools;
using System;

public class MonsterSpawner : MonoBehaviour
{
    public List<SpawnData> spawnDatas;

    public GameObject monsterPrefab;
    float nextSpawn;
    int screenWidth, screenHeight = 0;

    public float spawnTimer;

    public int monsterSize = 15;

    [SerializeField] PoolManager poolManager;

    public int spawnLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    void Spawn(int monIndex)
    {
        Monster mon = poolManager.GetFromPool<Monster>(monIndex);

        if (mon == null) return;

        int x = UnityEngine.Random.Range(0, screenWidth);
        int y = UnityEngine.Random.Range(0, screenHeight);

        int index = UnityEngine.Random.Range(0, 4);

        if (index == 0) mon.SetTransform(new Vector3(-monsterSize, y, 0));
        else if (index == 1) mon.SetTransform(new Vector3(screenWidth + monsterSize, y, 0));
        else if (index == 2) mon.SetTransform(new Vector3(x, -monsterSize, 0));
        else mon.SetTransform(new Vector3(x, screenHeight + monsterSize, 0));

        mon.Init(spawnDatas[spawnLevel]);
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTimer >= spawnDatas[spawnLevel].spawnTime)
        {
            //스폰할 몬스터 인덱스
            Spawn(spawnDatas[spawnLevel].spawnIndex);
            spawnTimer = 0;
        }
        spawnTimer += Time.deltaTime;
        spawnLevel = Mathf.Min((int)(PlayScene.Instance.gameTime / 10), spawnDatas.Count - 1);
    }

    public void ReturnPool(Monster mon)
    {
        poolManager.TakeToPool<Monster>(mon.id, mon);
    }
}

[System.Serializable]
public class SpawnData
{
    [SerializeField]
    public int spawnIndex;
    [SerializeField]
    public float spawnTime;
    [SerializeField]
    public int hp;
    [SerializeField]
    public float moveSpeed;
}