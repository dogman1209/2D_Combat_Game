using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_RightGroundCheck;		
	[SerializeField] private Transform m_LeftGroundCheck;		
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	public bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	bool fellFar = false;
	bool newJumpPress = false;

	bool dashJumping = false;
	bool landed = true;
	public bool canMove = true;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	public Rigidbody2D myRigidBody;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	void groundStagger() {

		canMove = true;

	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				landed = true;
				if(dashJumping){

					dashJumping = false;
					Invoke("groundStagger", 0.8f);

				}
				if (fellFar){

					canMove = false;
					Invoke("groundStagger", 1f);

				}
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
		Collider2D[] collidersL = Physics2D.OverlapCircleAll(m_LeftGroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < collidersL.Length; i++)
		{
			if (collidersL[i].gameObject != gameObject)
			{
				m_Grounded = true;
			}
		}
		Collider2D[] collidersR = Physics2D.OverlapCircleAll(m_RightGroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < collidersR.Length; i++)
		{
			if (collidersR[i].gameObject != gameObject)
			{
				m_Grounded = true;
			}
		}
	}

	//check to see if character is staggerable
	/*IEnumerator staggerable(){

		float myCounter = 0;

		while(true){

			if (m_grounded) {

				myCounter = 0;
				yield return new WaitForSeconds(0.02f);
				fellFar = false;

			} else {

				myCounter+= 0.1f;
				yield return new WaitForSeconds(0.1f);
				if (myCounter >= 30) {

					fellFar = true;

				}


			}
			

		}

	}*/

	void endJump(){

		newJumpPress = false;

	}

	void Update() {

		if (Input.GetButtonDown("Jump")){

            newJumpPress = true;
			Invoke("endJump", 0.05f);
            
        }
	 
	}

	void Start() {
        
        //StartCoroutine(staggerable());
		fellFar = false;

    }


	public void Move(float move, bool dash, bool jump, bool previousJump, bool dashActive, bool dashJump) {

		if (m_Grounded || m_AirControl) {

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			
			if (canMove){

				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			}

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}

		if (!m_Grounded && m_Rigidbody2D.velocity.y <= 2){

			if (!dashActive){

				if (landed){

				m_Rigidbody2D.AddForce(new Vector2(0f, -m_Rigidbody2D.velocity.y * 80));

			}

			
			landed = false;
			if (!dashActive){
				myRigidBody.gravityScale = 3f;
			}
			


			}
	
		}

		// If the player should jump...
		if (m_Grounded && jump && !previousJump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			if (!dashActive){
				myRigidBody.gravityScale = 1.25f;
			}
			

		}
		if (!m_Grounded && !jump && previousJump && landed){

			m_Rigidbody2D.AddForce(new Vector2(0f, -m_Rigidbody2D.velocity.y * 50));
			landed = false;
			if (!dashActive){
				myRigidBody.gravityScale = 3f;
			}


		}
		if (dash) {

			if (canMove){

			myRigidBody.velocity = new Vector2(0f, 0f);
			canMove = false;

			if (m_FacingRight){

				m_Rigidbody2D.AddForce(new Vector2(1100f, 0f));

			} else {

				m_Rigidbody2D.AddForce(new Vector2(-1100f, 0f));

			}

			}

		}
		//Dash Jump
		if (!m_Grounded && newJumpPress && dashJump){

			myRigidBody.velocity = new Vector2(0f, 0f);

			if (m_FacingRight){

				m_Rigidbody2D.AddForce(new Vector2(400f, 450f));

			} else {

				m_Rigidbody2D.AddForce(new Vector2(-400f, 450f));

			}
			dashActive = false;
			myRigidBody.gravityScale = 4;
			dashJumping = true;
			canMove = false;

		} 

	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
