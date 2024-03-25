using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    enum Type
    {
        Statue,
        Pillar_N,
        Pillar_W,
        Pillar_E
    }

    [SerializeField] Type type;
    [SerializeField] GameObject glow;

    private Collider2D _collider;
    [SerializeField] private ContactFilter2D _filter;
    private List<Collider2D> _collidedObjects = new List<Collider2D>(1);
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.gameStartEvent.Subscribe(PillarDectivate);
        _collider = GetComponent<Collider2D>();

        if (glow != null) 
        {
            glow.SetActive(false);
        }
    }

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
                            case Type.Statue:
                                GameManager.Instance.statueActivated = true;
                                print("Statue Activated");
                                break;
                            case Type.Pillar_N:
                                GameManager.Instance.PillarActivate("Pillar_N", glow);
                                GameManager.Instance.pillarNorthActivated = true;
                                break;
                            case Type.Pillar_W:
                                GameManager.Instance.PillarActivate("Pillar_W", glow);
                                GameManager.Instance.pillarWestActivated = true;
                                break;
                            case Type.Pillar_E:
                                GameManager.Instance.PillarActivate("Pillar_E", glow);
                                GameManager.Instance.pillarEastActivated = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }

    public void PillarDectivate()
    {
        if (glow != null)
        {
            glow.SetActive(false);
        }
    }

}
