using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class potionHold : MonoBehaviour
{
    public bool hold;
    public float distance = 5f;
    RaycastHit2D hit;
    public Transform holdPoint;
    public float throwObject = 5;

    // Ќаправление, в котором находитс€ персонаж
    private int playerDirection = 1; // ѕо умолчанию направление вправо

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0)
        {
            // ќпредел€ем направление персонажа
            playerDirection = (int)Mathf.Sign(horizontalInput);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
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
                    // »спользуем сохраненное направление персонажа дл€ броска зель€
                    hit.collider.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(playerDirection, 1) * throwObject;
                }
            }
        }

        if (hold)
        {
            // ”станавливаем позицию зель€ на точке держани€
            hit.collider.gameObject.transform.position = holdPoint.position;

            // ѕоворачиваем зелье в зависимости от направлени€ персонажа
            if (playerDirection > 0)
            {
                hit.collider.gameObject.transform.localScale = new Vector2(Mathf.Abs(hit.collider.gameObject.transform.localScale.x), Mathf.Abs(hit.collider.gameObject.transform.localScale.y));
                hit.collider.gameObject.transform.rotation = Quaternion.Euler(0, 0, -22); // ѕоворачиваем обратно по оси Y
            }
            else
            {
                // ≈сли персонаж повернут влево, поворачиваем зелье на 180 градусов по оси Y
                hit.collider.gameObject.transform.rotation = Quaternion.Euler(0, 180, -22);
            }
        }


        // ѕозици€ точки, где будет держатьс€ зелье
        holdPoint.position = transform.position + new Vector3(playerDirection, 1f, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(0f, 0.3f, 0f), transform.position + new Vector3(Vector3.right.x * playerDirection * distance, 0.3f, 0f));
    }
}

