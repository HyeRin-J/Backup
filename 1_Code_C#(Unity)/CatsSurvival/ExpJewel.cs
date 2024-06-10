using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ExpJewel : MonoBehaviour
{
    public int exp;
    public float moveSpeed = 1.0f;

    public Transform target = null;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.SqrMagnitude(PlayScene.Instance.player.transform.position - transform.position) <= PlayScene.Instance.expRange)
        {
            target = PlayScene.Instance.player.transform;
        }

        if(target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, PlayScene.Instance.player.transform.position, moveSpeed * Time.fixedDeltaTime);

            moveSpeed += 1.0f * Time.fixedDeltaTime;

            if (Vector3.SqrMagnitude(PlayScene.Instance.player.transform.position - transform.position) <= 0.01f)
            {
                PlayScene.Instance.AddExp(exp);
                Destroy(gameObject);
            }
        }
    }
}
