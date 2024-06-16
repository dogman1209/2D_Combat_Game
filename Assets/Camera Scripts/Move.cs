using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Move : MonoBehaviour
{

    public GameObject myCamera;
    public GameObject myCharacter;

    Vector2 playerPosition;
    float targetPositionX = 0;
    float targetPositionY = 0;
    float constrainLimitX = 0.05f; 
    float constrainLimitY = 0.5f; 
    bool myGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        
        playerPosition = new Vector2(0, 0);

    }

    // Update is called once per frame
    void Update()
    {

        playerPosition = myCharacter.transform.position;

    }

    void FixedUpdate() {
        
        if (myCharacter.GetComponent<PlayerMovement>().noLookahead){

            targetPositionX = targetPositionX + Mathf.Clamp((playerPosition.x - transform.position.x) * Time.fixedDeltaTime, -constrainLimitX, constrainLimitX) * 3.5f;

        } else {

            if (myCharacter.GetComponent<PlayerMovement>().bIsMovingRight){

                targetPositionX = (targetPositionX + Mathf.Pow(Mathf.Clamp((playerPosition.x + 3 - transform.position.x) * Time.fixedDeltaTime, -constrainLimitX, constrainLimitX) * 12, 3));
                
            } else {

                targetPositionX = (targetPositionX + Mathf.Pow(Mathf.Clamp((playerPosition.x - 3 - transform.position.x) * Time.fixedDeltaTime, -constrainLimitX, constrainLimitX) * 12, 3));

            }

        }

        

        if (myCharacter.GetComponent<CharacterController2D>().m_Grounded){

            targetPositionY = targetPositionY + Mathf.Clamp((playerPosition.y + 2 - transform.position.y) * Time.fixedDeltaTime, -constrainLimitY, constrainLimitY) * 5;
            myGrounded = true;
            

        } else {

            if (myCharacter.GetComponent<CharacterController2D>().myRigidBody.velocity.y < 0 && ((transform.position.y - playerPosition.y) > 2 || !myGrounded)){

                if (myCharacter.GetComponent<PlayerMovement>().groundChecking){

                    targetPositionY = targetPositionY + Mathf.Clamp((playerPosition.y + 2 - transform.position.y) * Time.fixedDeltaTime, -constrainLimitY, constrainLimitY) * 2.5f;

                } else {

                    targetPositionY = targetPositionY + Mathf.Clamp((playerPosition.y - 5 - transform.position.y) * Time.fixedDeltaTime, -constrainLimitY, constrainLimitY) * 3;

                }
                

                
                myGrounded = false;

                

            } else if((playerPosition.y - transform.position.y) > 3 || !myGrounded){

                targetPositionY = targetPositionY + Mathf.Clamp((playerPosition.y + 2 - transform.position.y) * Time.fixedDeltaTime, -constrainLimitY, constrainLimitY) * 3;
                myGrounded = false;

            }

        }

        myCamera.transform.position = new Vector3(targetPositionX, targetPositionY, -1);

    }
}
