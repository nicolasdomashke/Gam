using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer sprite;
    [SerializeField] private float speedMultiplier = 2f;
    void Start()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (InactiveQuestsStruct.currentQuest == null) {
            float xSpeed = Input.GetAxisRaw("Horizontal");
            float ySpeed = Input.GetAxisRaw("Vertical");

            rigidBody.velocity = new Vector2(xSpeed, ySpeed) * speedMultiplier;
            animator.SetFloat("speed", rigidBody.velocity.magnitude);
            if (xSpeed < -.01f)
            {
                sprite.flipX = true;
            }
            else if (xSpeed > .01f)
            {
                sprite.flipX = false;
            }
        }
    }
}
