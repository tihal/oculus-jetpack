using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour
{
    
    //Check to see whether we can continue flying up
    private bool fuel;

    //Float to track how much time has passed since we took off
    private float time = 0.0f;

    //How much time we have to fly before we "run out of fuel"
    public float flyTime = 3f;

    //Modifier to change how fast we descend after running out of fuel
    public float fallVelocity = -5f;


    //Reference our Character Controller on the Oculus prefab
    private CharacterController character;

    //Reference the gravity equation from OVRPlayerController script from Oculus Integration
    private float cancelGravity;

    //Declare our new upwards velocity for the Jetpack
    public float liftVelocity = 10f;

    //Create a new Vector3 to set our new Jetpack velocity
    private Vector3 moveDirection = Vector3.zero;

    //Flag to determine if we should descend slowly or if we should be affected by gravity
    private bool slowFall = false;


    //Declare two floats to reference the float value of the Oculus hand triggers
    private float JetpackLeft = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
    private float JetpackRight = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);

    // Start is called before the first frame update
    void Start()
    {
        //Set character to our Character Controller component
        character = GetComponent<CharacterController>();

        //Set cancelGravity equal to the gravity equation from OVRPlayerController
        cancelGravity = ((Physics.gravity.y * (GetComponent<OVRPlayerController>().GravityModifier * 0.002f)));
    }

    // Update is called once per frame
    void Update()
    {
        //Continually re-declare our float values for JetpackLeft and JetpackRight
        JetpackLeft = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
        JetpackRight = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);

        //Call Jetpack function
        newJetpack();

        //Calculate our new Character Controller move velocity
        character.Move(moveDirection * Time.deltaTime);

    }

    public void newJetpack()
    {
        //Set moveDirection back to 0
        moveDirection = Vector3.zero;

        if (time > flyTime)
        {
            //Run out of fuel after our designated fly time
            fuel = false;
        }

        //Check to see if both hand triggers are grabbed
        if (JetpackLeft > 0.9 && JetpackRight > 0.9 && fuel)
        {
            //start fuel timer
            time += Time.deltaTime;

            //Negate FallSpeed calculated in OVRPlayerController script
            GetComponent<OVRPlayerController>().FallSpeed = cancelGravity;

            //Increment y velocity on our Vector3 to create upward velocity
            moveDirection.y += liftVelocity;

            //Set slowFall to true 
            slowFall = true;

        }


        //If character is back on the ground, set slowFall back to false.  
        if (character.isGrounded)
        {
            slowFall = false;
            Debug.Log("slowFall value set to: " + slowFall);

            time = 0.0f;
            fuel = true;
        }

        //If slowFall is still true (meaning we're still in the air) then negate the gravity equation from OVRPlayerController
        if (slowFall)
        {
            //If you just add this line, you'll slowly drift down to the ground
            GetComponent<OVRPlayerController>().FallSpeed = cancelGravity;

            //Add this if you want to fall faster once you run out of fuel
            if (!fuel)
            {
                moveDirection.y += fallVelocity; //Fall velocity has to be a negative number
            }

        }

    }

}
