using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public MainControl mainControl;

    // On collision
	void OnCollisionEnter2D(Collision2D collision)
    {
        string collidedTag = collision.gameObject.tag;      // get tag of gameobject player collided with

        switch(collidedTag)
        {
            case "Start":   // on full circle
                mainControl.fullCircle();
                break;
            case "Block":
                mainControl.endGame();   // end the game
                break;
        }
    }
}
