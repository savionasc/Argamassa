using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Copia : MonoBehaviour {
	public GameObject[] listaG;
	public Rigidbody[] listaR;
	public Vector3 posicao;
	Script s;
	Rigidbody[] listaROriginal;
	// Use this for initialization
	void Start () {/*
		listaG = new GameObject[14];
		listaR = new Rigidbody[14];
		for(int i = 0; i < 14; i++){
			listaG[i] = GameObject.CreatePrimitive (PrimitiveType.Cube);
			listaR[i] = listaG[i].AddComponent<Rigidbody>();
		}
		//GameObject a = GameObject.CreatePrimitive (PrimitiveType.Cube);
		//GameObject b = GameObject.CreatePrimitive (PrimitiveType.Cube);
		//listaG = new GameObject[11];
		//
		//listaG [0] = a;
		//listaG [1] = b;

		//i = i + 2f;
		//listaG[1].transform.position = new Vector3 (i, i, 0);

		//listaR[1].mass = 5; // Set the GO's mass to 5 via the Rigidbody.
		//listaR[2].mass = 5; // Set the GO's mass to 5 via the Rigidbody.

		//listaG [1].transform.GetComponent<Rigidbody> ().AddForce (Vector3.up*200);

		listaROriginal = new Rigidbody[14];
		string[] nomes = {"Corpo (2)", "Corpo (1)", "Corpo", "CoxaD", "CanelaD", "PeD", "CoxaE", "CanelaE", "PeE", "CoxaE (1)", "CanelaE (1)", "PeE (1)", "CoxaD (1)", "CanelaD (1)"};

		for (int i = 0; i < 14; i++) {
			listaROriginal[i] = GameObject.Find (nomes[i]).transform.GetComponent<Rigidbody>();
		}
		//s = new Script ();
		///s = GameObject.Find ("GameController").GetComponent<Script>();

		//Script temp = GameObject.Find ("GameController").GetComponent<Script> ();
		//s.name = temp.name;
		//for(int z = 0; z < 1; z = z++){
		int[] corpos = {-1, 0, 1, 0, 3, 4, 2, 6, 7, 0, 9, 10, 0, 12};
		for (int i=0;i<14;i++) {
			//listaR[i].transform.position = listaROriginal[i].transform.position;
			listaR[i].transform.position = new Vector3 (listaROriginal[i].transform.position.x, listaROriginal[i].transform.position.y, listaROriginal[i].transform.position.z - 5);
			listaR[i].transform.localScale = listaROriginal[i].transform.localScale;
			listaR[i].transform.rotation = listaROriginal[i].transform.rotation;
			listaR[i].isKinematic = listaROriginal[i].isKinematic;
			listaR[i].name = "1 "+nomes[i];
			if (i != 0) {
				listaR[i].gameObject.AddComponent<HingeJoint> ();
				listaR[i].gameObject.GetComponent<HingeJoint> ().spring = listaROriginal [i].gameObject.GetComponent<HingeJoint> ().spring;
				listaR[i].gameObject.GetComponent<HingeJoint> ().useSpring = true;
				listaR[i].GetComponent<HingeJoint> ().connectedBody = listaR[corpos[i]].gameObject.GetComponent<Rigidbody> ();
				listaR[i].GetComponent<HingeJoint> ().axis = new Vector3 (0, 0, 1);
			} else {
				listaR[0].constraints = listaROriginal [0].constraints;
			}
			listaG[i].GetComponent<Renderer>().material.color = new Color(0.5f,1,0);
			//}
		}
	*/}
	
	// Update is called once per frame
	void Update () {
		
	}
}
