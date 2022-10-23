using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int index = transform.GetSiblingIndex();
            Transform obj = transform.parent;
            Vector2 pos = Vector2.zero;

            for (int i = 0; i < obj.childCount; i++)
            {
                if(i != index)
                {
                    pos = obj.GetChild(i).GetComponent<Transform>().position;
                }
            }

            collision.gameObject.transform.position = pos;
        }
    }
}
