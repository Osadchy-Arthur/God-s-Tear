using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class potionHold : MonoBehaviour
{

    public float distance = 5f;
    RaycastHit2D hit;
    public Transform holdPoint;
    public float throwObject = 5;

    // Íàïðàâëåíèå, â êîòîðîì íàõîäèòñÿ ïåðñîíàæ
    private int playerDirection = 1; // Ïî óìîë÷àíèþ íàïðàâëåíèå âïðàâî

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0)
        {
            // Îïðåäåëÿåì íàïðàâëåíèå ïåðñîíàæà
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
                    // Èñïîëüçóåì ñîõðàíåííîå íàïðàâëåíèå ïåðñîíàæà äëÿ áðîñêà çåëüÿ
                    hit.collider.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(playerDirection, 1) * throwObject;
                }
            }
        }

        if (hold)
        {
            // Óñòàíàâëèâàåì ïîçèöèþ çåëüÿ íà òî÷êå äåðæàíèÿ
            hit.collider.gameObject.transform.position = holdPoint.position;

            // Ïîâîðà÷èâàåì çåëüå â çàâèñèìîñòè îò íàïðàâëåíèÿ ïåðñîíàæà
            if (playerDirection > 0)
            {
                hit.collider.gameObject.transform.localScale = new Vector2(Mathf.Abs(hit.collider.gameObject.transform.localScale.x), Mathf.Abs(hit.collider.gameObject.transform.localScale.y));
                hit.collider.gameObject.transform.rotation = Quaternion.Euler(0, 0, -22); // Ïîâîðà÷èâàåì îáðàòíî ïî îñè Y
            }
            else
            {
                // Åñëè ïåðñîíàæ ïîâåðíóò âëåâî, ïîâîðà÷èâàåì çåëüå íà 180 ãðàäóñîâ ïî îñè Y
                hit.collider.gameObject.transform.rotation = Quaternion.Euler(0, 180, -22);
            }
        }


        // Ïîçèöèÿ òî÷êè, ãäå áóäåò äåðæàòüñÿ çåëüå
        holdPoint.position = transform.position + new Vector3(playerDirection, 1f, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(0f, 0.3f, 0f), transform.position + new Vector3(Vector3.right.x * playerDirection * distance, 0.3f, 0f));
    }
}

