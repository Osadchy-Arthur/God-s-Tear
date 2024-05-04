using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameObject nearestLoot; // Ближайший предмет для подбора

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // Если нажата клавиша H
        {
            PickUpNearestLoot(); // Вызываем метод для подбора ближайшего предмета
        }
    }

    void PickUpNearestLoot()
    {
        GameObject[] lootObjects = GameObject.FindGameObjectsWithTag("loot"); // Находим все объекты с тегом "loot"
        float nearestDistance = Mathf.Infinity; // Начальное значение расстояния до ближайшего предмета

        foreach (GameObject lootObject in lootObjects)
        {
            float distance = Vector3.Distance(transform.position, lootObject.transform.position); // Расстояние до текущего предмета

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestLoot = lootObject; // Обновляем ближайший предмет
            }
        }

        if (nearestLoot != null)
        {
            Debug.Log("Подобран предмет: " + nearestLoot.name);
            Destroy(nearestLoot); // Удаляем подобранный предмет из сцены
        }
        else
        {
            Debug.Log("Подходящих предметов не найдено.");
        }
    }
}
