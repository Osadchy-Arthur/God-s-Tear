using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int lives = 5;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float attackRange = 0.5f;
    public int attackDamage = 40;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    public LayerMask enemyLayers;
    public Transform attackPoint;
    private bool isGrounded = false;


    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    public bool hold;
    public float distance = 5f;
    RaycastHit2D hit;
    public Transform holdPoint;
    public float throwObject = 5;

    // Направление, в котором находится персонаж
    private int playerDirection = 1; // По умолчанию направление вправо


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        if (Input.GetButton("Horizontal"))
            Run();
        if (isGrounded && Input.GetButtonDown("Jump"))
            Jump();
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }

        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            HoldThrowPotion();
        }

        if (hold)
        {
            // Устанавливаем позицию зелья на точке держания
            hit.collider.gameObject.transform.position = holdPoint.position;
        }
    }

    private void HoldThrowPotion()
    {

        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0)
        {
            // Определяем направление персонажа
            playerDirection = (int)Mathf.Sign(horizontalInput);
        }

        if (!hold)
        {
            Physics2D.queriesStartInColliders = false;
            hit = Physics2D.Raycast(transform.position + new Vector3(0f, 0.3f, 0f), Vector2.right * playerDirection, distance);

            if (hit.collider != null && hit.collider.tag == "Potion")
            {
                hold = true;
            }
        }
        else
        {
            hold = false;

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
            {
                // Используем сохраненное направление персонажа для броска зелья
                hit.collider.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(playerDirection, 1) * throwObject;
            }
        }


        

    }

    private void Run()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 dir = transform.right * horizontalInput;

        // Перемещаем игрока
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

        // Устанавливаем позицию attackPoint с учетом направления движения игрока
        float attackPointOffset = Mathf.Sign(horizontalInput) * attackRange;
        attackPoint.position = transform.position + new Vector3(attackPointOffset, 1f, 0f);

        // Поворачиваем спрайт игрока и attackPoint в зависимости от направления движения
        sprite.flipX = horizontalInput < 0.0f;

        if (horizontalInput != 0)
        {
            // Определяем направление персонажа
            playerDirection = (int)Mathf.Sign(horizontalInput);
        }

        if (hold)
        {
            // Поворачиваем зелье в зависимости от направления персонажа
            if (playerDirection > 0)
            {
                hit.collider.gameObject.transform.localScale = new Vector2(Mathf.Abs(hit.collider.gameObject.transform.localScale.x), Mathf.Abs(hit.collider.gameObject.transform.localScale.y));
                hit.collider.gameObject.transform.rotation = Quaternion.Euler(0, 0, -22); // Поворачиваем обратно по оси Y
            }
            else
            {
                // Если персонаж повернут влево, поворачиваем зелье на 180 градусов по оси Y
                hit.collider.gameObject.transform.rotation = Quaternion.Euler(0, 180, -22);
            }
        }


        // Позиция точки, где будет держаться зелье
        holdPoint.position = transform.position + new Vector3(playerDirection, 1f, 0f);
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrounded = collider.Length > 1;
    }

    public void TakeDamage(int damageAmount)
    {
        lives -= damageAmount;
        Debug.Log("Player has taken " + damageAmount + " damage. Lives remaining: " + lives);
        if (lives <= 0)
        {
            Die();
        }
    }

    private void Attack()
    {
        // play animation
        // animator.SetTrigger("Attack");

        //detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //damage then
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    // risuem radius ataki dla ydobstva
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void Die()
    {
        
        Debug.Log("Player has died!");
        this.enabled = false;
        
    }

}
