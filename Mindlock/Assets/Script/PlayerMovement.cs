using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    public Animator anim;
    private bool moving;

    public float moveSpeed;

    public Rigidbody2D rb;

    private float moveX;
    private float moveY;

    private Vector2 moveDirection;

    public bool canMove = true;

    // zvuk krokù
    public AudioSource footstepSource;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        inputProccesing();
        Animate();
        WalkingSoundEffect();
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Move();
        }
    }

    void inputProccesing()
    {
        if (canMove)
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");

            moveDirection = new Vector2(moveX, moveY).normalized;
        }
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
