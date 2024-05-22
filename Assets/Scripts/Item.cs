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

    private Collider2D _collider;
    [SerializeField] private ContactFilter2D _filter;
    private List<Collider2D> _collidedObjects = new List<Collider2D>(1);
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    [SerializeField] Type type;

    //Collidable code: https://www.youtube.com/watch?v=R_DPVlJK8o8
    private void Update()
    {
        _collider.OverlapCollider(_filter, _collidedObjects);

        if (_collidedObjects.Count > 0)
        {
            foreach (var obj in _collidedObjects)
            {
                if (obj.gameObject.CompareTag("Player"))
                {
                    if (Input.GetKeyDown(KeyCode.E))
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
                        //Delete Item After Collision and Log What You Consumed
                        SpawnItem.Instance.drinkSFX.Play();
                        Debug.Log("You Consumed The: " + type.ToString());
                        GameManager.Instance.DeleteItem(gameObject);
                    }
                }
            }
        }
    }


}
