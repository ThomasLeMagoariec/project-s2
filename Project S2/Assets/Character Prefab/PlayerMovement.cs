
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    [Header ("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool canJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;


    [Header("Ground check")]
    public LayerMask whatIsGround;
    public float groundDistance;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitSlope;
    private Vector3 slopeMoveDirection;

    [Header("Other")]
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    

    //Animations
    private Animator animator;
    private string currentAnimation;

    
    
    
    
    private Vector3 moveDirection;
    private Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        Walking,
        Sprinting,
        Air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        animator = GetComponent<Animator>();

        canJump = true;
        Debug.Log("Script is running");

        ChangeAnimation("Idle Normal");
    }

    private void FixedUpdate() //Better for physics stuff
    {
        grounded = Physics.CheckSphere(transform.position, groundDistance, whatIsGround);
    }

    private void DebugCode()
    {
        Debug.Log($"On Slope: {onSlope()}, Slope Angle: {Vector3.Angle(Vector3.up, slopeHit.normal)}");
        Debug.Log($"Force Applied: {moveDirection * moveSpeed}, Slope Force: {GetSlopeMoveDirection() * moveSpeed}");
        Debug.DrawLine(transform.position, transform.position + slopeMoveDirection, Color.blue);
    }


    private void Update()
    {
        //DebugCode();
        MyInput();
        SpeedCOntrol();
        StateHandler();
        
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        if(grounded) rb.drag = groundDrag;
        else rb.drag = 0;
        
        MovePlayer();
        CheckAnimation();
    }

    
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        if(Input.GetKey(jumpKey) && canJump && grounded)
        {
            canJump = false;
            ChangeAnimation("Jump Start");
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(grounded && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ChangeAnimation("Attack 1");
        }
    }

    private void StateHandler()
    {
        if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.Sprinting;
            moveSpeed = sprintSpeed;
        }
        else if(grounded)
        {
            state = MovementState.Walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.Air;
        }
        
    }

    private void MovePlayer()
    {

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if(onSlope() && grounded) rb.AddForce(GetSlopeMoveDirection().normalized * moveSpeed * 10f , ForceMode.Force);
        else if(grounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f , ForceMode.Force);
        else if(!grounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f  * airMultiplier, ForceMode.Force);

        rb.useGravity = !onSlope();
    }

    private void SpeedCOntrol()
    {
        if(onSlope() && !exitSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
            
        }
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if(flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }

        
    }


    public void Jump()
    {
        exitSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        exitSlope = false;
        canJump = true;
    }



    private bool onSlope()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out slopeHit, 0.6f)) //Checks if is on a surface
            if (slopeHit.normal != Vector3.up && Vector3.Angle(Vector3.up, slopeHit.normal) <= maxSlopeAngle) // Checks if surface isn't horizontal && is walkable (aka under not steeper than max slope)
                return true;
        
        return false;

    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }
    
    
    private void CheckAnimation()
    {
        if(currentAnimation == "Attack 1" || currentAnimation == "Jump Start")
        {
            return;
        }

        if(currentAnimation == "Jump Air")
        {
            if(grounded)
                ChangeAnimation("Jump End");
            return;
        }
        if(verticalInput == 1)
        {
            ChangeAnimation("Move Forward");
        }else if(verticalInput == -1)
        {
            ChangeAnimation("Move Backward");
        }else if(horizontalInput == 1)
        {
            ChangeAnimation("Move Right");
        }else if(horizontalInput == -1)
        {
            ChangeAnimation("Move Left");
        }else
        {
            ChangeAnimation("Idle Battle");
        }
    }


    public void ChangeAnimation(string animation, float crossfade = 0.2f, float time = 0f)
    {
        if(time > 0 )
            StartCoroutine(Wait());
        else
            Validate();
        
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time - crossfade);
            Validate();
        }


        void Validate()
        {
            if(currentAnimation != animation)
            {
                currentAnimation = animation;
                if(currentAnimation == "")
                    CheckAnimation();
                else
                    animator.CrossFade(animation, crossfade);
            }
        }
        
    }
    
}
