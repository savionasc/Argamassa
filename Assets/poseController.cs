using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Data;
using Mono.Data.SqliteClient;

//Class da Chita
public class Animalx{
	public GameObject chita;
	public bool pernaFrente = false;
	public bool pernaTras = false;
	public bool iniciado = false;
	public GameObject Body1,Body2,Body3, Body4,Body5,Body6;
	public Rigidbody rb1,rb2,rb3,rb4,rb5,rb6, torax;

    public NeuralNetwork net;
    //private bool initilized = false;

    public Animalx(){}
		
	public Animalx(GameObject chita){
		this.chita = chita;
		setBody ();
		setRigibody ();
	}

	void setBody(){
		this.Body1 = this.chita.transform.GetChild (3).gameObject;
		this.Body2 = this.chita.transform.GetChild (4).gameObject;
		this.Body3 = this.chita.transform.GetChild (5).gameObject;
		this.Body4 = this.chita.transform.GetChild (6).gameObject;
		this.Body5 = this.chita.transform.GetChild (7).gameObject;
		this.Body6 = this.chita.transform.GetChild (8).gameObject;
	}

	void setRigibody(){
		this.rb1 = Body1.GetComponent<Rigidbody>();
		this.rb2 = Body2.GetComponent<Rigidbody>();
		this.rb3 = Body3.GetComponent<Rigidbody>();
		this.rb4 = Body4.GetComponent<Rigidbody>();
		this.rb5 = Body5.GetComponent<Rigidbody>();
		this.rb6 = Body6.GetComponent<Rigidbody>();
        this.torax = this.chita.transform.GetChild(0).gameObject.GetComponent<Rigidbody>();
    }

    public void Init(NeuralNetwork net){//, Transform hex){
        //this.hex = hex;
        this.net = net;
        //initilized = true;
    }

}

public class poseController : MonoBehaviour{
    public GameObject largada;
    public GameObject chao;
    public GameObject chita;
    public bool visualizacao = false; 
    List<float> fitnesses;
    List<Animalx> animals;
    bool salvouFitness = false;

    private int[] ct = new int[700];
    private int evaluationTime= 440;
    private int ritmo = 30;

    private GameObject testeColisao;
    private int populationSize = 1;
    private int populationIterator = 0;
    private int generationNumber = 0;
    private int[] layers = new int[] { 4, 5, 5, 2 }; //1 input and 1 output
    private List<NeuralNetwork> nets;
    public bool abordagem = false;

    bool simulation = false;

    string urlDataBase = "URI=file:MasterSQLite.db";
    IDbCommand _command;

    void Awake(){
        chita = GameObject.Find("Chita");
        largada = GameObject.Find("Largada");
    }

    void Start(){
        animals = new List<Animalx>();
        fitnesses = new List<float>();
        ct[0] = 0;
        salvouFitness = false;
        inicializaDB();
        simulation = Physics.autoSimulation;
    }

    void InitArgamassaNeuralNetworks(){
        if (populationSize % 2 != 0){
            populationSize = 20;
        }
        nets = new List<NeuralNetwork>();
        for (int i = 0; i < populationSize; i++){
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate();
            nets.Add(net);
        }
    }

    private float conversao(float X, float Xmax, float Xmin, float Ymax, float Ymin){
        return Ymin+(((X-Xmin)/(Xmax-Xmin))*(Ymax-Ymin));
    }

    // Update is called once per frame
    void Update(){
        if (visualizacao == false) {
            if (populationIterator == populationSize - 1 || generationNumber == 0)
            {
                if (generationNumber == 0)
                {
                    InitArgamassaNeuralNetworks();
                    generationNumber++;
                    //Debug.Log(consulta(2));
                    nets[2].SetFitness(0f);
                }else{
                    nets.Sort();
                    print("Pior rede: " + nets[0].GetFitness());
                    print("Melhor rede: " + nets[nets.Count - 1].GetFitness());

                    string redes = "";
                    for (int i = 0; i < populationSize; i++)
                    {
                        redes += " " + nets[i].GetFitness() + "|" + nets[i].nome;
                    }
                    print("saida: " + redes);
                    for (int i = 0; i < populationSize / 2; i++)
                    {
                        nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)], false);
                        nets[i].Mutate();
                        nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)], true); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                    }

                    //nets[2] = rede;

                    for (int i = 0; i < populationSize; i++)
                    {
                        //print("Fit: " + nets[i].GetFitness());
                        nets[i].SetFitness(0f);
                    }

                    //fitnesses.Add(0f);
                    Text textGeracao = GameObject.Find("Geracao").GetComponent<Text>();
                    textGeracao.text = "Geração " + generationNumber;
                    populationIterator = 0;
                }
            }

            /*if (populationIterator == 2){
                //if (ia == 0){
                //    print(deletar());
                //    print(inserir(2));
                //    ia++;
                //}

                //Debug.Log(nets[2].consulta().ToString("0.000000000000"));
                Debug.Log(consulta(2));
                nets[2].SetFitness(0f);
            }*/

            Physics.autoSimulation = false;

            if (abordagem == false)
            {
                criarArgamassa();
                liberaCorpo(animals[0]);
                esticaPernaTrasVetor(animals[0]);
                esticaPernaFrenteVetor(animals[0]);
                //print(nets[0].atualizar());

                testeColisao = animals[0].chita.transform.GetChild(13).gameObject;

                for (int i = 1; i <= evaluationTime; i++)
                {
                    float[] inputs = new float[4];
                    if (animals[0].torax.transform.eulerAngles.z > 180)
                    {
                        inputs[3] = conversao(animals[0].torax.transform.eulerAngles.z - 360, 90f, -90f, 0.5f, -0.5f);
                    }else{
                        inputs[3] = conversao(animals[0].torax.transform.eulerAngles.z, 90f, -90f, 0.5f, -0.5f);
                    }

                    inputs[3] = conversao(inputs[3], 90f, -90f, 0.5f, -0.5f);
                    inputs[2] = animals[0].torax.transform.position.y - chao.transform.position.y /*Altura do tórax*/;
                    inputs[2] = conversao(inputs[2], -1.5f, -2.8f, 0.5f, -0.5f);
                    inputs[1] = animals[0].rb3.transform.position.y - chao.transform.position.y  /*Altura da pata da frente*/;
                    inputs[1] = conversao(inputs[1], -2f, -3.88f, 0.5f, -0.5f);
                    inputs[0] = animals[0].rb6.transform.position.y - chao.transform.position.y /*Altura da pata de trás*/;
                    inputs[0] = conversao(inputs[0], -2f, -3.88f, 0.5f, -0.5f);
                    //print("0: " + inputs[0] + " 1:" + inputs[1] + " 2:" + inputs[2] + " 3:" + inputs[3]);

                    float[] output = animals[0].net.FeedForward(inputs);
                    if (output[0] > 0.4f)
                    {
                        esticaPernaTrasVetor(animals[0]);
                    }

                    if (output[1] > 0.4f)
                    {
                        esticaPernaFrenteVetor(animals[0]);
                    }

                    if (i % ritmo == 0)
                    {
                        esticaPernaTrasVetor(animals[0]);
                    }

                    if ((i + (ritmo / 2)) % ritmo == 0)
                    {
                        esticaPernaFrenteVetor(animals[0]);
                    }
                    Physics.Simulate(Time.fixedDeltaTime);
                }
                if (testeColisao.GetComponent<colisao>().colidiu == true)
                {
                    print("Colidiu");
                    animals[0].net.SetFitness(-10);
                    testeColisao.GetComponent<MeshRenderer>().material.color = new Color(100, 0, 0, 0.5f);
                }else{
                    animals[0].net.SetFitness(animals[0].chita.transform.Find("Corpo (2)").transform.position.x - largada.transform.position.x);
                }
                fitnesses.Add((animals[0].net.GetFitness()));
                avaliar(0, animals[0]);
                GameObject.Destroy(animals[0].chita.gameObject);
                animals.Remove(animals[0]);
            }else{
                //Outro método
            }
            populationIterator++;
            Physics.autoSimulation = true;
        }
    }

    void FixedUpdate(){
        if (visualizacao){
            if (populationIterator == populationSize - 1 || generationNumber == 0){
                if (generationNumber == 0){
                    InitArgamassaNeuralNetworks();
                    generationNumber++;
                    //Debug.Log(consulta(2));
                    nets[2].SetFitness(0f);
                }else{
                    nets.Sort();
                    print("Pior rede: " + nets[0].GetFitness());
                    print("Melhor rede: " + nets[nets.Count - 1].GetFitness());

                    string redes = "";
                    for (int i = 0; i < populationSize; i++){
                        redes += " " + nets[i].GetFitness() + "|" + nets[i].nome;
                    }
                    print("saida: " + redes);
                    for (int i = 0; i < populationSize / 2; i++){
                        nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)], false);
                        nets[i].Mutate();
                        nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)], true); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                    }

                    for (int i = 0; i < populationSize; i++){
                        nets[i].SetFitness(0f);
                    }

                    Text textGeracao = GameObject.Find("Geracao").GetComponent<Text>();
                    textGeracao.text = "Geração " + generationNumber;
                    populationIterator = 0;
                }
            }

            if (abordagem == false){
                if (ct[0] == 0){
                    criarArgamassa();
                    liberaCorpo(animals[0]);
                    esticaPernaTrasVetor(animals[0]);
                    esticaPernaFrenteVetor(animals[0]);
                    salvouFitness = false;
                    testeColisao = animals[0].chita.transform.GetChild(13).gameObject;
                }
                ct[0]++;

                if (animals.Count > 0){
                    int i = 0;
                    float[] inputs = new float[4];
                    if (animals[0].torax.transform.eulerAngles.z > 180){
                        inputs[3] = animals[0].torax.transform.eulerAngles.z - 360;
                    }else{
                        inputs[3] = animals[0].torax.transform.eulerAngles.z;
                    }
                    inputs[3] = conversao(inputs[3], 90f, -90f, 0.5f, -0.5f);
                    inputs[2] = animals[0].torax.transform.position.y - chao.transform.position.y /*Altura do tórax*/;
                    inputs[2] = conversao(inputs[2], -1.5f, -2.8f, 0.5f, -0.5f);
                    inputs[1] = animals[0].rb3.transform.position.y - chao.transform.position.y  /*Altura da pata da frente*/;
                    inputs[1] = conversao(inputs[1], -2f, -3.88f, 0.5f, -0.5f);
                    inputs[0] = animals[0].rb6.transform.position.y - chao.transform.position.y /*Altura da pata de trás*/;
                    inputs[0] = conversao(inputs[0], -2f, -3.88f, 0.5f, -0.5f);
                    //print("0: " + inputs[0] + " 1:" + inputs[1] + " 2:" + inputs[2] + " 3:" + inputs[3]);

                    float[] output = animals[0].net.FeedForward(inputs);
                    if (output[0] > 0.4f){
                        esticaPernaTrasVetor(animals[0]);
                    }

                    if (output[1] > 0.4f){
                        esticaPernaFrenteVetor(animals[0]);
                    }

                    if (ct[i] % ritmo == 0){
                        esticaPernaTrasVetor(animals[0]);
                    }

                    if ((ct[i] + (ritmo / 2)) % ritmo == 0){
                        esticaPernaFrenteVetor(animals[0]);
                    }

                    if ((ct[i] > evaluationTime) && !salvouFitness){
                        if (testeColisao.GetComponent<colisao>().colidiu == true){
                            print("Colidiu");
                            //animals[0].net.SetFitness(-10);
                            animals[0].net.SetFitness(animals[0].chita.transform.Find("Corpo (2)").transform.position.x - largada.transform.position.x);
                            testeColisao.GetComponent<MeshRenderer>().material.color = new Color(100, 0, 0, 0.5f);
                        }else{
                            animals[0].net.SetFitness(animals[0].chita.transform.Find("Corpo (2)").transform.position.x - largada.transform.position.x);
                        }

                        fitnesses.Add((animals[0].net.GetFitness()));
                        avaliar(0, animals[0]);
                        GameObject.Destroy(animals[0].chita.gameObject);
                        animals.Remove(animals[0]);
                        salvouFitness = true;
                        populationIterator++;
                    }


                }

                if (ct[0] > evaluationTime + 1){
                    ct[0] = 0;
                }
            }
        }
    }

    public void criarArgamassa(){
        animals.Add(new Animalx(GameObject.Instantiate(chita)));
        // animals[animals.Count - 1].Init(nets[populationIterator]);//, hex.transform);
        animals[animals.Count - 1].Init(nets[populationIterator]);//, hex.transform);

        animals[animals.Count - 1].chita.transform.position = new Vector3(-2.16f, 0.16f, -(2f * animals.Count));
        //animals[animals.Count - 1].chita.transform.position = new Vector3(0.0f, 0.0f, -(2f * animals.Count));
        string name = "AAA" + animals.Count;
        animals[animals.Count - 1].chita.name = name;
    }

    public void liberaCorpo(Animalx animal){
        foreach (Transform child in animal.chita.transform){
            child.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void avaliar(int i, Animalx animal){
        //int cont,
        //Text txCt = GameObject.Find("TextA").GetComponent<Text>();
        //txCt.text = ("Cont: " + cont);
        if (!salvouFitness){
            Text tx = GameObject.Find("TextA" + (i + 0)).GetComponent<Text>();
            tx.text = ("Fit atual: " + (animal.chita.transform.Find("Corpo (2)").transform.position.x - largada.transform.position.x));
            Text txFts = GameObject.Find("TextA" + (i + 1)).GetComponent<Text>();
            txFts.text = "Fitnesses: ";
            foreach (float fit in fitnesses){
                txFts.text += fit + " ";
            }
        }
    }

    public void inicializaDB()
    {
        IDbConnection _connection = new SqliteConnection(urlDataBase);
        _command = _connection.CreateCommand();
        _connection.Open();
        //string sql = "CREATE TABLE IF NOT EXISTS highscores (name VARCHAR(20), score INT)";
        string sql = "CREATE TABLE IF NOT EXISTS redes_neurais (id int, layers VARCHAR(350), neurons VARCHAR(1550), weights VARCHAR(5550), visitou boolean)";
        _command.CommandText = sql;
        _command.ExecuteNonQuery();
    }

    public string inserir(int id)
    {
        //string sql = "INSERT INTO highscores (name, score) values('ME', 5000)";
        string sql = "INSERT INTO redes_neurais (id, layers, weights) values(" + id + ", '" + nets[2].GetSLayers() + "','" + nets[2].GetSWeights() + "')";
        _command.CommandText = sql;
        _command.ExecuteNonQuery();
        return ("Inseriu");
    }

    public void consulta(int rede)
    {
        string sql = "SELECT id, layers, weights" + " FROM redes_neurais";
        _command.CommandText = sql;
        IDataReader reader = _command.ExecuteReader();

        float[][][] weights = nets[rede].GetWeights();
        while (reader.Read())
        {
            /*
            //layers
            string layer = reader.GetString(1);
            string[] layerSplit = layer.Split(new string[] { " " }, StringSplitOptions.None);
            this.layers = new int[layerSplit.Length-1];
            for (int i = 0; i < layerSplit.Length; i++){
                int result;
                if (int.TryParse(layerSplit[i], out result)){
                    layers[i] = result;
                }
            }
            //neurons
            InitNeurons();*/

            //Falta iniciar os pesos
            //weights
            string weigh = reader.GetString(2);
            string[] weightSplit = weigh.Split(new string[] { " Y " }, StringSplitOptions.None);
            List<float[][]> weightsList = new List<float[][]>();
            for (int i = 0; i < weightSplit.Length - 1; i++)
            {
                List<float[]> layerWeightsList = new List<float[]>();
                int neuronsInPreviousLayer = layers[i];
                //4
                string[] xSplit = weightSplit[i].Split(new string[] { " X " }, StringSplitOptions.None);
                for (int j = 0; j < xSplit.Length - 1; j++)
                {
                    float[] neuronWeights = new float[neuronsInPreviousLayer]; //neruons weights
                                                                               //5
                    string[] ySplit = xSplit[j].Split(new string[] { " " }, StringSplitOptions.None);
                    for (int k = 0; k < ySplit.Length - 1; k++)
                    {
                        //4
                        float result;
                        if (float.TryParse(ySplit[k], out result))
                        {
                            neuronWeights[k] = result;
                            //retorno += "Peso: " + weights[i][j][k] + "\n";
                        }
                    }
                    layerWeightsList.Add(neuronWeights);
                }
                weightsList.Add(layerWeightsList.ToArray()); //add this layers weights converted into 2D array into weights list
            }
            weights = weightsList.ToArray(); //convert to 3D array
        }

        nets[rede].SetWeight(weights);
        nets[rede].nome = "XXXX";
        Debug.Log("Recuperou do banco");
    }

    public string atualizar()
    {
        //string sql = "UPDATE redes_neurais set id = 1 WHERE id != 0";
        //_command.CommandText = sql;
        //_command.ExecuteNonQuery();
        return ("   Atualizou!");
    }

    public string deletar()
    {
        string sql = "DELETE FROM redes_neurais WHERE id != -56420";
        _command.CommandText = sql;
        _command.ExecuteNonQuery();
        return ("Deletou tudo!");
    }


    public void esticaPernaFrenteVetor(Animalx animal){
        if (animal.pernaFrente == false){
            JointSpring springJ1 = animal.rb1.GetComponent<HingeJoint>().spring; springJ1.targetPosition = -45;
            animal.rb1.GetComponent<HingeJoint>().spring = springJ1;
            JointSpring springJ2 = animal.rb2.GetComponent<HingeJoint>().spring; springJ2.targetPosition = 0;
            animal.rb2.GetComponent<HingeJoint>().spring = springJ2;
            JointSpring springJ3 = animal.rb3.GetComponent<HingeJoint>().spring; springJ3.targetPosition = 0;
            animal.rb3.GetComponent<HingeJoint>().spring = springJ3;
        }else{
            JointSpring springJ1 = animal.rb1.GetComponent<HingeJoint>().spring; springJ1.targetPosition = 40;
            animal.rb1.GetComponent<HingeJoint>().spring = springJ1;
            JointSpring springJ2 = animal.rb2.GetComponent<HingeJoint>().spring; springJ2.targetPosition = -90;
            animal.rb2.GetComponent<HingeJoint>().spring = springJ2;
            JointSpring springJ3 = animal.rb3.GetComponent<HingeJoint>().spring; springJ3.targetPosition = 100;
            animal.rb3.GetComponent<HingeJoint>().spring = springJ3;
        }
        animal.pernaFrente = !animal.pernaFrente;
    }

    public void esticaPernaTrasVetor(Animalx animal){
        if (animal.pernaTras == false){
            JointSpring springJ1 = animal.rb4.GetComponent<HingeJoint>().spring; springJ1.targetPosition = -45;
            animal.rb4.GetComponent<HingeJoint>().spring = springJ1;
            JointSpring springJ2 = animal.rb5.GetComponent<HingeJoint>().spring; springJ2.targetPosition = 0;
            animal.rb5.GetComponent<HingeJoint>().spring = springJ2;
            JointSpring springJ3 = animal.rb6.GetComponent<HingeJoint>().spring; springJ3.targetPosition = 0;
            animal.rb6.GetComponent<HingeJoint>().spring = springJ3;
        }else{
            JointSpring springJ1 = animal.rb4.GetComponent<HingeJoint>().spring; springJ1.targetPosition = 40;
            animal.rb4.GetComponent<HingeJoint>().spring = springJ1;
            JointSpring springJ2 = animal.rb5.GetComponent<HingeJoint>().spring; springJ2.targetPosition = -90;
            animal.rb5.GetComponent<HingeJoint>().spring = springJ2;
            JointSpring springJ3 = animal.rb6.GetComponent<HingeJoint>().spring; springJ3.targetPosition = 100;
            animal.rb6.GetComponent<HingeJoint>().spring = springJ3;
        }
        animal.pernaTras = !animal.pernaTras;
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