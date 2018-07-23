using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour {

    World world;

	// Use this for initialization
	void Start () {
        world = GameManager.Instance.World;
    }
	
	// Update is called once per frame
	void Update () {

        //If it is stunned ignore the input
        if (world.thePlayer.Stunned)
        {
            return;
        }
        world.thePlayer.UpdateXMovement(Input.GetAxisRaw("Horizontal"));

        if (Input.GetButtonDown("Jump")){
            world.thePlayer.Jump();
        }
    }
}
