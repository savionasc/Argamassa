using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colisao : MonoBehaviour {
    public bool colidiu = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("Chao"))
        {
            colidiu = true;
        }
        //print("colidiu com: " + collision.gameObject.name);

    }
}
