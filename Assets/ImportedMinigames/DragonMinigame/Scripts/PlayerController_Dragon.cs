using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Dragon : MonoBehaviour
{
    public float speed;
    private float moveInput;

    public float jumpForce;

    private int extraJumps;
    public int extraJumpsValue;

    private bool facingRight = true;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isGrounded;
    public Transform GroundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        extraJumps = extraJumpsValue;

    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, whatIsGround);

        if (rb.velocity.y != 0 && !Physics2D.OverlapCircle(GroundCheck.position, checkRadius, whatIsGround))
        {
            anim.SetBool("IsJumping", true);
        }
        else
        {
            anim.SetBool("IsJumping", false);
        }

        moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if (rb.velocity.x >= 2 || rb.velocity.x <= -2)
        {
            anim.SetBool("IsRunning", true);
        }
        else
        {
            anim.SetBool("IsRunning", false);
        }

        if (facingRight == false && moveInput > 0)
        {
            Flip();
        }
        else if (facingRight == true && moveInput < 0)
        {
            Flip();
        }
    }

    private void Update()
    {
        if (isGrounded == true)
        {
            extraJumps = extraJumpsValue;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) && extraJumps > 0)
        {
            rb.velocity = Vector2.up * jumpForce;
            extraJumps--;
        }
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) && extraJumps == 0 && isGrounded == true)
        {
            rb.velocity = Vector2.up * jumpForce;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("fireball"))
        {
            SceneMessenger.instance.LoadNewScene("Overworld", false);
            //Destroy(collision.gameObject);
            //loadscene

        }
        if (collision.gameObject.CompareTag("end"))
        {
            SceneMessenger.instance.LoadNewScene("Overworld", true);
            //loadscene
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
}
