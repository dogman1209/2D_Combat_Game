using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 50;
    public CharacterController2D controller;
    float horizontalInput = 0;
    public GameObject groundCollider;
    public bool jump = false;
    bool previousJump = false;
    public bool bIsMovingRight = false;
    public bool noLookahead = true;
    public bool groundChecking = false;

    bool oneHit = false;
    bool twoHit = false;
    bool threeHit = false;
    bool dash = false;
    bool canDash = true;
    bool dashActive = false;
    bool dashJump = false;
    IEnumerator dashFunction(){

        while(true){

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetAxis("Dash") == 1){

                if (canDash){
                    controller.myRigidBody.gravityScale = 0;
                    dash = true;
                    dashActive = true;
                    dashJump = true;
                    canDash = false;
                    yield return new WaitForSeconds(0.05f);
                    controller.myRigidBody.gravityScale = 0;
                    yield return new WaitForSeconds(0.05f);
                    controller.myRigidBody.gravityScale = 0;
                    dash = false;
                    controller.canMove = true;
                    controller.myRigidBody.gravityScale = 3;
                    yield return new WaitForSeconds(0.2f);
                    dashJump = false;
                    dashActive = false;
                    yield return new WaitForSeconds(0.4f);
                    while (!controller.m_Grounded){
                        yield return new WaitForSeconds(0.02f);
                    }
                    canDash = true;
                    

                } else {
                    yield return new WaitForSeconds(0.02f);
                } 
            } else {

                yield return new WaitForSeconds(0.02f);

            }

        }

    }

    // Start is called before the first frame update
    void Start() {
        
        StartCoroutine(dashFunction());

    }

    // Update is called once per frame
    void Update() {
        
        horizontalInput = Input.GetAxisRaw("Horizontal") * speed;

        if (Input.GetAxisRaw("Horizontal") > 0){

            bIsMovingRight = true;
            noLookahead = false;

        } else if (Input.GetAxisRaw("Horizontal") < 0){ 

            bIsMovingRight = false;
            noLookahead = false;

        } else {

            noLookahead = true;

        }

        if (Input.GetButton("Jump")){

            jump = true;
            
        }

         

    }

    void FixedUpdate() {

        controller.Move(horizontalInput * Time.fixedDeltaTime, dash, jump, previousJump, dashActive, dashJump);
        if (controller.m_Grounded){

            previousJump = false;

        }
        previousJump = jump;
        jump = false;
        dash = false;


        if (!controller.m_Grounded){

            for (int i = -1; i < 1; i++){

                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x - i*4, transform.position.y - 3), new Vector2(transform.position.x - i*4, transform.position.y - 10), 4f);

            if (hit.collider != null){
                
                groundChecking = true;
                oneHit = true;
                twoHit = true;
                threeHit = true;

            } else {

                if (i == -1){

                    oneHit = false;

                } else  if (i == 0){

                    twoHit = false;

                } else {

                    threeHit = false;

                }

            }

            if (!oneHit && !twoHit && !threeHit) {

                groundChecking = false;

            }

            }
            

        } else {

            groundChecking = false;

        }
    
    }
}
