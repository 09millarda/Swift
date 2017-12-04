using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public Rigidbody2D rb;
    public int gravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

	void FixedUpdate()
	{
		Vector2 blockPos = this.transform.position;     // block position
		Vector2 center = new Vector2(0f, 0f);       // center position
		Vector2 dir = (blockPos - center).normalized;       // Vector between block and center

        rb.AddForce(-dir * gravity);
	}
}
