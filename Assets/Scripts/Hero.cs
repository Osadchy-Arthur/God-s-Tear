using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hero : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int lives = 5;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float attackRange = 0.5f;
    public int attackDamage = 40;
    public float attackRate = 0.5f;
    private float nextAttackTime = 0f;
    public LayerMask enemyLayers;
    public Transform attackPoint;
    private bool isGrounded = false;
    public Joystick joystick;


    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private bool hold;
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
        if (joystick.Horizontal != 0)
            Run();
        if (isGrounded && joystick.Vertical > 0.5f)
            Jump();

        if (hold)
        {
            // Устанавливаем позицию зелья на точке держания
            hit.collider.gameObject.transform.position = holdPoint.position;
        }
    }

/*
    private void Run()
    {
    
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 dir = transform.right * joystick.Horizontal;

        // 
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

        // 
        float attackPointOffset = Mathf.Sign(horizontalInput) * attackRange;
        attackPoint.position = transform.position + new Vector3(attackPointOffset, 1f, 0f);

        // 
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
*/

    private void Run()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 dir = transform.right * joystick.Horizontal;

        // Move the character
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

        // Update attack point position
        float attackPointOffset = Mathf.Sign(joystick.Horizontal) * attackRange;
        attackPoint.position = transform.position + new Vector3(attackPointOffset, 1f, 0f);

        // Update holding point position
        holdPoint.position = transform.position + new Vector3(playerDirection, 1f, 0f);

        // Flip character sprite based on input
        sprite.flipX = horizontalInput < 0.0f;

        if (horizontalInput != 0)
        {
            // Update the player's direction
            playerDirection = (int)Mathf.Sign(horizontalInput);
        }

        if (hold)
        {
            // Rotate the held object based on the player's direction
            hit.collider.gameObject.transform.localScale = new Vector2(playerDirection * Mathf.Abs(hit.collider.gameObject.transform.localScale.x), Mathf.Abs(hit.collider.gameObject.transform.localScale.y));
            hit.collider.gameObject.transform.rotation = Quaternion.Euler(0, 0, playerDirection > 0 ? -22 : 158); // Rotate based on player's direction
        }
    }

    private void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;
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
        AudioManager.instance.Play("Damage");
        if (lives <= 0)
        {
            Die();
        }
    }

    public void Attack()
    {
        if (Time.time < nextAttackTime)
            return;
        nextAttackTime = Time.time + 1f / attackRate;
        // play animation
        // animator.SetTrigger("Attack");

        //detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //damage then
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }

        AudioManager.instance.Play("Attack");
    }


    public void HoldThrowPotion()
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
        AudioManager.instance.Play("Death");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
