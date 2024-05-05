using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public int HP = 100;
    public float speed;

    public int positionOfPatrol;
    public Transform point;
    bool movingRight;

    Transform player;
    public float stoppingDistance;

    bool chill = false;
    bool angry = false;
    bool goBack = false;

    public float attackRange = 0.5f; 

    public int damage = 10; 
    public float attackRate = 0.5f;
    private float nextAttackTime = 0f;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, point.position) < positionOfPatrol && angry == false)
        {
            chill = true;
        }

        if (Vector2.Distance(transform.position, player.position) < stoppingDistance)
        {
            angry = true;
            chill = false;
            goBack = false;
        }

        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            goBack = true;
            angry = false;
        }

        if (chill == true)
        {
            Chill();
        }
        else if (angry == true)
        {
            Angry();
        }
        else if (goBack == true)
        {
            GoBack();
        }

        if (Time.time >= nextAttackTime)
        {
            if (Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                AttackPlayer();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Chill()
    {
        if (transform.position.x > point.position.x + positionOfPatrol)
        {
            movingRight = false;
        }
        else if (transform.position.x < point.position.x - positionOfPatrol)
        {
            movingRight = true;
        }

        if (movingRight)
        {
            transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
        }
        else
        {
            transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
        }

        speed = 3;
    }

    void Angry()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        speed = 4;
    }

    void GoBack()
    {
        transform.position = Vector2.MoveTowards(transform.position, point.position, speed * Time.deltaTime);
        speed = 3;
    }

    void AttackPlayer()
    {
       
        Hero playerHero = player.GetComponent<Hero>();

        if (playerHero != null)
        {
            playerHero.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;

        // play hurt animation

        if (HP <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Enemy deid!");

        // die animation

        // disable the enemy
        gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        AudioManager.instance.Play("Victory");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
