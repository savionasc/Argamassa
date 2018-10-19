using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Argamassa : MonoBehaviour{
    public GameObject largada;
    public GameObject chao;
    public GameObject chita;
    public bool pernaFrente = false;
    public bool pernaTras = false;
    public bool iniciado = false;
    public GameObject Body1, Body2, Body3, Body4, Body5, Body6;
    public Rigidbody rb1, rb2, rb3, rb4, rb5, rb6, torax;

    private int ct; //= 0;
    private int ritmo; //= 20;

    public bool abordagem = true;
    public Argamassa() {
        ct = 0;
        ritmo = 20;
    }

    public Argamassa(GameObject chita){
        this.chita = chita;
        setBody();
        setRigibody();
        ct = 0;
        ritmo = 15;
    }

    void setBody(){
        this.Body1 = this.chita.transform.GetChild(3).gameObject;
        this.Body2 = this.chita.transform.GetChild(4).gameObject;
        this.Body3 = this.chita.transform.GetChild(5).gameObject;
        this.Body4 = this.chita.transform.GetChild(6).gameObject;
        this.Body5 = this.chita.transform.GetChild(7).gameObject;
        this.Body6 = this.chita.transform.GetChild(8).gameObject;
        GameObject tor = this.chita.transform.GetChild(0).gameObject;
        torax = tor.GetComponent<Rigidbody>();
    }

    void setRigibody(){
        this.rb1 = Body1.GetComponent<Rigidbody>();
        this.rb2 = Body2.GetComponent<Rigidbody>();
        this.rb3 = Body3.GetComponent<Rigidbody>();
        this.rb4 = Body4.GetComponent<Rigidbody>();
        this.rb5 = Body5.GetComponent<Rigidbody>();
        this.rb6 = Body6.GetComponent<Rigidbody>();
    }


    //Variaveis para rede neural
    private bool initilized = false;
    //private Transform hex;
    private NeuralNetwork net;

    public void Init(NeuralNetwork net){//, Transform hex){
        //this.hex = hex;
        this.net = net;
        initilized = true;
    }

    GameObject testeColisao;
    void Awake(){
        rb1 = Body1.GetComponent<Rigidbody>();
        rb2 = Body2.GetComponent<Rigidbody>();
        rb3 = Body3.GetComponent<Rigidbody>();
        rb4 = Body4.GetComponent<Rigidbody>();
        rb5 = Body5.GetComponent<Rigidbody>();
        rb6 = Body6.GetComponent<Rigidbody>();
        GameObject tor = this.chita.transform.GetChild(0).gameObject;
        torax = tor.GetComponent<Rigidbody>();
        testeColisao = this.chita.transform.GetChild(13).gameObject;
        //chita = GameObject.Find("Chita");
        //z = new Animalx ();
        //a = GameObject.Instantiate (chita);
        //a.transform.position = new Vector3 (-2.16f,0.16f,-5f);
    }

    // Use this for initialization
    void Start(){
        //print("teste");
        esticaPernaTras();
        esticaPernaFrente();
        esticaPernaTras();
        esticaPernaFrente();
    }

    

    //float temp = 0;
    void FixedUpdate(){
        if (initilized == true){
            //print("Colidiu? " + testeColisao.GetComponent<colisao>().colidiu);

            //Se for poses pre-programadas
            if (abordagem == false){
                //float[] inputs = new float[4];
                //Inicialmente são extraídas do personagem as entradas da rede neural
                //if (torax.transform.eulerAngles.z > 180){
                //    inputs[3] = torax.transform.eulerAngles.z - 360;
                //}else{
                //    inputs[3] = torax.transform.eulerAngles.z;
                //}
                //inputs[2] = torax.transform.position.y - chao.transform.position.y /*Altura do tórax*/;
                //inputs[1] = rb3.transform.position.y - chao.transform.position.y  /*Altura da pata da frente*/;
                //inputs[0] = rb6.transform.position.y - chao.transform.position.y /*Altura da pata de trás*/;

                //E por ultimo é gereda a saída e aplicada ao personagem
                //float[] output = net.FeedForward(inputs);
                //print("recebi uma euler : " + torax.transform.eulerAngles.z);//se maior que 180, ang-360
                //print("recebi uma entrada : " + inputs[0] + " : " + inputs[1] + " : " + inputs[2]);
                //print("recebi uma saida: " + output[0] + " : " + output[1]);
                ct++;
                //print(ct);

                if (ct % ritmo == 0){
                    esticaPernaTras();
                }
                if ((ct + ritmo / 2) % ritmo == 0){
                    esticaPernaFrente();
                }
                //if (output[0] > 0.4f){
                    //esticaPernaTras();
                ///}
                //if (output[1] > 0.4f){
                    //esticaPernaFrente();
                //}

                //aqui são usadas as funções de movimentação

                //rBody.velocity = 2.5f * transform.up;
                //rBody.angularVelocity = 500f * output[0];

                //Por ultimo é adicionado um fitness
                //net.AddFitness((1f - Mathf.Abs(inputs[0])));
                //ne/t.SetFitness(torax.position.x - largada.transform.position.x);
            }else {
                float[] inputs = new float[4];
                //Inicialmente são extraídas do personagem as entradas da rede neural

                if (torax.transform.eulerAngles.z > 180){
                    inputs[3] = torax.transform.eulerAngles.z - 360;
                }else{
                    inputs[3] = torax.transform.eulerAngles.z;
                }
                inputs[2] = torax.transform.position.y - chao.transform.position.y /*Altura do tórax*/;
                inputs[1] = rb3.transform.position.y - chao.transform.position.y  /*Altura da pata da frente*/;
                inputs[0] = rb6.transform.position.y - chao.transform.position.y /*Altura da pata de trás*/;

                //E por ultimo é gereda a saída e aplicada ao personagem
                float[] output = net.FeedForward(inputs);

                esticaPernaFrente(output[0] * 180, output[1] * 180, output[2] * 180);
                esticaPernaTras(output[3] * 180, output[4] * 180, output[5] * 180);

                net.SetFitness(torax.position.x - largada.transform.position.x);
            }
            if (testeColisao.GetComponent<colisao>().colidiu == true){
                //initilized = false;
                net.SetFitness(-10);
                testeColisao.GetComponent<MeshRenderer>().material.color = new Color(100, 0, 0, 0.5f);
            }

        }
    }

    public void esticaPernaFrente(){
        if (pernaFrente == false){
            JointSpring springJ1 = rb1.GetComponent<HingeJoint>().spring; springJ1.targetPosition = -45;
            rb1.GetComponent<HingeJoint>().spring = springJ1;
            JointSpring springJ2 = rb2.GetComponent<HingeJoint>().spring; springJ2.targetPosition = 0;
            rb2.GetComponent<HingeJoint>().spring = springJ2;
            JointSpring springJ3 = rb3.GetComponent<HingeJoint>().spring; springJ3.targetPosition = 0;
            rb3.GetComponent<HingeJoint>().spring = springJ3;
        }else{
            JointSpring springJ1 = rb1.GetComponent<HingeJoint>().spring; springJ1.targetPosition = 40;
            rb1.GetComponent<HingeJoint>().spring = springJ1;
            JointSpring springJ2 = rb2.GetComponent<HingeJoint>().spring; springJ2.targetPosition = -90;
            rb2.GetComponent<HingeJoint>().spring = springJ2;
            JointSpring springJ3 = rb3.GetComponent<HingeJoint>().spring; springJ3.targetPosition = 100;
            rb3.GetComponent<HingeJoint>().spring = springJ3;
        }
        pernaFrente = !pernaFrente;
    }

    public void esticaPernaFrente(float a, float b, float c){
        a = (360 + a) % 360;
        b = (360 + b) % 360;
        c = (360 + c) % 360;
        //print("X: "+a+" : "+b+" : "+c);
        JointSpring springJ1 = rb1.GetComponent<HingeJoint>().spring; springJ1.targetPosition = Mathf.Floor(a);
        rb1.GetComponent<HingeJoint>().spring = springJ1;
        JointSpring springJ2 = rb2.GetComponent<HingeJoint>().spring; springJ2.targetPosition = Mathf.Floor(b);
        rb2.GetComponent<HingeJoint>().spring = springJ2;
        JointSpring springJ3 = rb3.GetComponent<HingeJoint>().spring; springJ3.targetPosition = Mathf.Floor(c);
        rb3.GetComponent<HingeJoint>().spring = springJ3;
    }

    public void esticaPernaTras(){
        if (pernaTras == false){
            JointSpring springJ1 = rb4.GetComponent<HingeJoint>().spring; springJ1.targetPosition = -45;
            rb4.GetComponent<HingeJoint>().spring = springJ1;
            JointSpring springJ2 = rb5.GetComponent<HingeJoint>().spring; springJ2.targetPosition = 0;
            rb5.GetComponent<HingeJoint>().spring = springJ2;
            JointSpring springJ3 = rb6.GetComponent<HingeJoint>().spring; springJ3.targetPosition = 0;
            rb6.GetComponent<HingeJoint>().spring = springJ3;
        }else{
            JointSpring springJ1 = rb4.GetComponent<HingeJoint>().spring; springJ1.targetPosition = 40;
            rb4.GetComponent<HingeJoint>().spring = springJ1;
            JointSpring springJ2 = rb5.GetComponent<HingeJoint>().spring; springJ2.targetPosition = -90;
            rb5.GetComponent<HingeJoint>().spring = springJ2;
            JointSpring springJ3 = rb6.GetComponent<HingeJoint>().spring; springJ3.targetPosition = 100;
            rb6.GetComponent<HingeJoint>().spring = springJ3;
        }
        pernaTras = !pernaTras;
    }

    public void esticaPernaTras(float d, float e, float f){
        d = (360 + d) % 360;
        e = (360 + e) % 360;
        f = (360 + f) % 360;
        //print("Y: " + d + " : " + e + " : " + f);
        JointSpring springJ1 = rb4.GetComponent<HingeJoint>().spring; springJ1.targetPosition = Mathf.Floor(d);
        rb4.GetComponent<HingeJoint>().spring = springJ1;
        JointSpring springJ2 = rb5.GetComponent<HingeJoint>().spring; springJ2.targetPosition = Mathf.Floor(e);
        rb5.GetComponent<HingeJoint>().spring = springJ2;
        JointSpring springJ3 = rb6.GetComponent<HingeJoint>().spring; springJ3.targetPosition = Mathf.Floor(f);
        rb6.GetComponent<HingeJoint>().spring = springJ3;
    }
}