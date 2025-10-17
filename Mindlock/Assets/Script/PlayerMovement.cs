using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public Animator anim;
    private bool moving;

    public float moveSpeed;

    public Rigidbody2D rb;

    private float moveX;
    private float moveY;

    private Vector2 moveDirection;


    // zvuk krokù
    public AudioSource footstepSource;


    // Update is called once per frame
    void Update()
    {
        inputProccesing();
        Animate();
        WalkingSoundEffect();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void inputProccesing()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

    void Animate()
    {
        if (moveDirection.magnitude > 0.1f || moveDirection.magnitude < -0.1f)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        if (moving)
        {
            anim.SetFloat("X", moveX);
            anim.SetFloat("Y", moveY);
        }


            anim.SetBool("Moving", moving);
    }

    void WalkingSoundEffect()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            if (!footstepSource.isPlaying)
                footstepSource.Play();
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Pause();
        }
    }
}
