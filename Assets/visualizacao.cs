using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visualizacao : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    //void FixedUpdate()
    //{
    //    //print(Time.fixedDeltaTime);
    //    //print(Time.deltaTime);

    //    //cont++;
    //    //if (cont % 3 == 0)
    //    //{

    //    if (ct[0] == 0)
    //    {
    //        criarArgamassa();
    //        liberaCorpo(animals[0]);

    //        //criarEsfera();
    //        //liberaEsfera(esferas[0]);

    //        //animals[0].pernaTras = false;
    //        //animals[0].pernaFrente = false;
    //        esticaPernaTrasVetor(animals[0]);
    //        esticaPernaFrenteVetor(animals[0]);
    //        //esticaPernaTrasVetor(animals[0]);
    //        //esticaPernaFrenteVetor(animals[0]);

    //        salvouFitness = false;
    //    }

    //    ct[0]++;

    //    if (animals.Count > 0)
    //    {
    //        for (int i = 0; i < animals.Count; i++)
    //        {
    //            //avaliar(ct[i], i, animals[i], esferas[i]);
    //            avaliar(ct[i], i, animals[i]);

    //            //ct[i]++;
    //            //print(ct[i]);

    //            if (!salvouFitness)
    //            {
    //                if (ct[i] % ritmo == 0)
    //                {
    //                    esticaPernaTrasVetor(animals[i]);
    //                    //animals[0].chita.transform.Find("Corpo (2)").GetComponent<Rigidbody>().AddForce(3000, 0, 0);
    //                }
    //                if ((ct[i] + (ritmo / 2)) % ritmo == 0)
    //                {
    //                    esticaPernaFrenteVetor(animals[i]);
    //                    //animals[0].chita.transform.Find("Corpo (2)").GetComponent<Rigidbody>().AddForce(0, 3000, 0);
    //                    //animals[0].chita.transform.Find("Corpo (2)").GetComponent<Rigidbody>().maxAngularVelocity = Mathf.Infinity;
    //                    //animals[0].chita.transform.Find("Corpo (2)").GetComponent<Rigidbody>().AddTorque(0, 0, 100);
    //                    //animals[0].chita.transform.Find("CanelaD (1)").GetComponent<HingeJoint>().useSpring = false;
    //                    //animals[0].chita.transform.Find("CanelaD (1)").GetComponent<Rigidbody>().maxAngularVelocity = Mathf.Infinity;
    //                    //animals[0].chita.transform.Find("CanelaD (1)").GetComponent<Rigidbody>().AddTorque(0, 0, 100);
    //                }
    //            }

    //            if ((ct[i] > evaluationTime) && !salvouFitness)
    //            {
    //                fitnesses.Add((animals[i].chita.transform.Find("Corpo (2)").transform.position.x - largada.transform.position.x));
    //                //fitnessesEsf.Add((esferas[i].transform.position.x - largada.transform.position.x));
    //                salvouFitness = true;

    //                GameObject.Destroy(animals[i].chita.gameObject);
    //                animals.Remove(animals[i]);

    //                //GameObject.Destroy(esferas[i].gameObject);
    //                //esferas.Remove(esferas[i]);
    //            }
    //        }
    //    }
    //    //chao.transform.position = new Vector3(Body1.transform.position.x,-3.88f,0.0f);

    //    if (ct[0] > evaluationTime + 1) //+ 50)
    //    {
    //        ct[0] = 0;
    //    }
    //    //}

    //    //if ((cont+3) % 3 == 0)
    //    //{
    //    //Physics.Simulate(Time.fixedDeltaTime);
    //    //Physics.Simulate(0.02f);
    //    //}
    //}
}
