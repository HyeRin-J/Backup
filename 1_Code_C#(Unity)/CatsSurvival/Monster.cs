using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Redcode.Pools;

public class Monster : MonoBehaviour, IPoolObject
{
    public string id;

    public new Rigidbody2D rigidbody;
    public Animator animator;

    public float moveSpeed;

    [SerializeField] int life = 300;

    public Transform target;

    public MonsterSpawner spawner;
    public GameObject expJewel;

    public void SetTransform(Vector3 pos)
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(pos);
        position.z = 0;
        transform.position = position;
        transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
    }

    public async UniTaskVoid MoveToTarget()
    {
        while (life > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.fixedDeltaTime);
            rigidbody.velocity = Vector2.zero;

            if (transform.position.x > target.position.x)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        await UniTask.Delay(1000);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SetDamage(10);
        }
    }


    public void SetDamage(int damage)
    {
        Vector3 velocity = (target.position - transform.position).normalized * 0.5f;
        rigidbody.AddForce(-velocity, ForceMode2D.Impulse);

        life -= damage;

        if (life <= 0)
        {
            life = 0;
            animator.SetTrigger("Dead");
            GetComponent<Collider2D>().enabled = false;
            rigidbody.simulated = false;
        }
    }

    public void Dead()
    {
        PlayScene.Instance.KillMonster();
        Instantiate(expJewel, transform.position, Quaternion.identity).GetComponent<ExpJewel>().exp = spawner.spawnLevel * 2;
        spawner.ReturnPool(this);
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void Init(SpawnData spawnData)
    {
        life = spawnData.hp;
        moveSpeed = spawnData.moveSpeed;
        rigidbody.simulated = true;
        if (target == null) target = PlayScene.Instance.player.transform;
        GetComponent<Collider2D>().enabled = true;
        animator.Play("Move");
        MoveToTarget().Forget();
    }

    public void OnCreatedInPool()
    {
        spawner = FindObjectOfType<MonsterSpawner>();
    }

    public void OnGettingFromPool()
    {
    }
}
