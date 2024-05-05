using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryMenu : MonoBehaviour
{
    public void Victory()
    {
        Time.timeScale = 0f;
        GetComponent<Collider2D>().enabled = true;
    }
}
