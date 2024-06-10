using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] new Rigidbody2D rigidbody;
    [SerializeField] Animator animator;
    [SerializeField] Animation hitAnimation;
    [SerializeField] new Collider2D collider;

    public GameObject hpBar;
    public GameObject hpBarBack;

    public int attDmg = 10;
    public float moveSpeed;
    public float attackSpeed;
    public float attRange = 3;

    private int flickerCount = 0;
    private int life = 300;
    private int lifeMax = 300;

    public Vector2 inputValue;

    public Scanner scanner;

    private void Start()
    {
        scanner.scanRange = attRange;
        AttackInterval().Forget();
    }

    void OnMove(InputValue inputVal)
    {
        inputValue = inputVal.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = inputValue * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            SetDamage(10);
            flickerCount = 5;
            Flickering().Forget();
        }
    }

    void SetDamage(int damage)
    {
        life -= damage;
        flickerCount = 5;
        if (life <= 0)
        {
            // 게임 오버
            life = 0;

        }

        hpBar.transform.DOScaleX((float)life / lifeMax * 0.5f, 0.2f);        
    }

    public void Attack()
    {
        foreach (var target in scanner.hits)
        {
            target.transform.GetComponent<Monster>().SetDamage(attDmg);
        }
    }

    async UniTaskVoid AttackInterval()
    {
        while (true)
        {
            animator.SetTrigger("Attack");
            await UniTask.Delay((int)(1000 * attackSpeed));
        }
    }

    async UniTaskVoid Flickering()
    {
        while (flickerCount > 0)
        {
            hitAnimation.Play();
            collider.enabled = false;

            flickerCount--;
            await UniTask.Delay(100);
        }

        collider.enabled = true;
    }

    private void LateUpdate()
    {
        if (inputValue.x > 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        else if (inputValue.x < 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        animator.SetFloat("MoveSpeed", inputValue.magnitude);
        hpBarBack.transform.position = transform.position + new Vector3(0, 0.96f, 0);
    }
}
