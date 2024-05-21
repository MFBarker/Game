using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Item : MonoBehaviour
{
    enum Type
    {
        Death,
        Minus10,
        Minus50,
        Minus100,
        Plus50
    }

    [SerializeField] Type type;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //check if collided object is the player and you are interacting with the item
        if (collision.gameObject.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            switch (type)
            {
                case Type.Death:
                    //If you interact with item, kill player
                    GameManager.Instance.KillPlayer();
                    break;
                case Type.Minus10:
                    //minus 10 seconds
                    GameManager.Instance.TimeReduction(10.0f);
                    break;
                case Type.Minus50:
                    //minus 50 seconds
                    GameManager.Instance.TimeReduction(50.0f);
                    break;
                case Type.Minus100:
                    //minus 100 seconds
                    GameManager.Instance.TimeReduction(100.0f);
                    break;
                case Type.Plus50:
                    //plus 50 seconds
                    GameManager.Instance.TimeAddition(50.0f);
                    break;
                default:
                    break;
            }
        }
    }
}
