using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        [SerializeField] public float speed;

        private Animator animator;
        private Rigidbody2D rb;

        private void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();

            GameManager.Instance.respawnEvent.Subscribe(resetPos);
        }

        private void resetPos(GameObject spawn)
        {
            rb.position = spawn.transform.position;
        }

        private void Update()
        {
            //check dir inputs
            bool left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            bool right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
            bool up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            bool down = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

            Vector2 dir = Vector2.zero;
            //Horizontal
            if (left)
            {
                dir.x = -1;
                animator.SetFloat("moveX", dir.x);

                animator.SetFloat("lastX", dir.x);
                animator.SetFloat("lastY", 0);
            }
            else if (right)
            {
                dir.x = 1;
                animator.SetFloat("moveX", dir.x);

                animator.SetFloat("lastX", dir.x);
                animator.SetFloat("lastY", 0);
            }
            else
            {
                animator.SetFloat("moveX", 0);
            }
            //Vertical
            if (up)
            {
                dir.y = 1;
                animator.SetFloat("moveY", dir.y);

                animator.SetFloat("lastX", 0);
                animator.SetFloat("lastY", dir.y);
            }
            else if (down)
            {
                dir.y = -1;
                animator.SetFloat("moveY", dir.y);

                animator.SetFloat("lastX", 0);
                animator.SetFloat("lastY", dir.y);
            }
            else
            {
                animator.SetFloat("moveY", 0);
            }

            rb.velocity = speed * dir;
            if (GameManager.Instance.canMove == true && (left || right || up || down)) //allowed to move and movement key is pressed
            {

                animator.SetFloat("Speed", rb.velocity.magnitude);
            }
            else 
            {
                rb.velocity = Vector3.zero;
                animator.SetFloat("Speed", 0);
            }

            
        }

    }
}
