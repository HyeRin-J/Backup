using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    [SerializeField] new Collider2D collider;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area")) return;

        Vector3 playerPos = PlayScene.Instance.player.transform.position;
        Vector3 pos = transform.position;

        float x = Mathf.Abs(playerPos.x - pos.x);
        float y = Mathf.Abs(playerPos.y - pos.y);

        Vector3 playerDir = PlayScene.Instance.player.GetComponent<Player>().inputValue;
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;

        switch(transform.tag)
        {
            case "Ground":
                if(x > y)
                {
                    transform.Translate(Vector3.right * dirX * 40);
                }
                else if(x < y)
                {
                    transform.Translate(Vector3.up * dirY * 40);
                }
                break;
            case "Monster":
                if(collider.enabled)
                {
                    transform.Translate(playerDir * 20 + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0));
                }
                break;
        }
    }
}
