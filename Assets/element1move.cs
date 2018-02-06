using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class element1move : MonoBehaviour {

    gamemanager manager;
    float speed = 2f;
    float time = 0;
	// Use this for initialization
	void Start () {
        manager = transform.Find("/Canvas").GetComponent<gamemanager>();

    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(-Time.deltaTime * 550 / speed, 0, 0));
        time += Time.deltaTime;

        if (transform.position.x <= -100)
        {
            manager.DeleteElement(gameObject);
            Destroy(gameObject);
            manager.retry();
        }
    }

    public void setspeed(float num)
    {
        speed = num;
    }
}
