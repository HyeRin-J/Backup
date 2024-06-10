using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] hits;
    public Transform nearTarget;

    private void FixedUpdate()
    {
        hits = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        if (hits.Length > 0)
        {
            nearTarget = hits[0].transform;
        }
        else
        {
            nearTarget = null;
        }
    }
}
