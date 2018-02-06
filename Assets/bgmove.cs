using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgmove : MonoBehaviour {

    float speed = 2f;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(-Time.deltaTime * 550 / speed, 0, 0));
        if (transform.position.x <= -371.5f)
        {
            transform.position = new Vector3(371.5f, transform.position.y, 0);
        }
    }

    public void setspeed(float num)
    {
        speed = num;
    }
}
