using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Data;
using Mono.Data.SqliteClient;
//using UnityEditor;

//Class da Chita
public class Animalx{
	public GameObject chita;
	public bool pernaFrente = false;
	public bool pernaTras = false;
	public bool iniciado = true;
	public GameObject CoxaDB, CanelaDB, PeDB, CoxaEB, CanelaEB, PeEB;
	public Rigidbody CoxaDrb, CanelaDrb, PeDrb, CoxaErb, CanelaErb, PeErb, torax;

    public Animalx(){}
		
	public Animalx(GameObject chita){
		this.chita = chita;
		setBody ();
		setRigibody ();
	}

	void setBody(){
		this.CoxaDB = this.chita.transform.GetChild (3).gameObject;
		this.CanelaDB = this.chita.transform.GetChild (4).gameObject;
		this.PeDB = this.chita.transform.GetChild (5).gameObject;
		this.CoxaEB = this.chita.transform.GetChild (6).gameObject;
		this.CanelaEB = this.chita.transform.GetChild (7).gameObject;
		this.PeEB = this.chita.transform.GetChild (8).gameObject;
	}

	void setRigibody(){
		this.CoxaDrb = CoxaDB.GetComponent<Rigidbody>();
		this.CanelaDrb = CanelaDB.GetComponent<Rigidbody>();
		this.PeDrb = PeDB.GetComponent<Rigidbody>();
		this.CoxaErb = CoxaEB.GetComponent<Rigidbody>();
		this.CanelaErb = CanelaEB.GetComponent<Rigidbody>();
		this.PeErb = PeEB.GetComponent<Rigidbody>();
        this.torax = this.chita.transform.GetChild(0).gameObject.GetComponent<Rigidbody>();
    }
}

public class poseController : MonoBehaviour {
    public GameObject largada;
    public GameObject chao;
    public GameObject chita;
    List<float> fitnesses;
    List<Animalx> animals;
    bool salvouFitness = false;
    public bool verMelhorDaGeração = false;

    private int ct = 0;
    public int evaluationTime = 440;
    public bool visualizacao = false;
    public bool abordagem = false;
    public bool novasRedes = false;
    public Vector3 velocidade = new Vector3(3f, 0f, 0f);
    public int ritmo = 36;

    private GameObject testeColisao;
    private int populationSize = 1;
    private int populationIterator = 0;
    private int generationNumber = 0;
    private List<NeuralNetwork> nets;
    private int[] layers;
    NeuralNetwork bestNet;

    string urlDataBase = "URI=file:MasterSQLite.db";
    IDbCommand _command;

    //true, true para visualizar o melhor com visualizacao == true antes de mostrar o melhor
    //true, false para visualizar o melhor com visualizacao == false antes de mostrar o melhor
    public bool[] oncebest = { false, false };
    public bool best = false;
    public Animalx bestAnimalx;
    public int limite = 22;
    public int numPressFrente;
    public int numPressTras;
    void Awake() {
        numPressFrente = limite;
        numPressTras = limite;
        chita = GameObject.Find("Chita");
        largada = GameObject.Find("Largada");
    }

    void Start() {
        animals = new List<Animalx>();
        fitnesses = new List<float>();
        salvouFitness = false;
        inicializaDB();
        if (!abordagem){
            layers = new int[] { 4, 5, 5, 2 }; //1 input and 1 output
        }else{
            layers = new int[] { 4, 5, 5, 6 }; //1 input and 1 output
        }
        bestNet = new NeuralNetwork(layers);
        if (best) {
            bestNet.SetWeight(consulta(21));
        }
    }

    void InitArgamassaNeuralNetworks() {
        if (populationSize % 2 != 0) {
            populationSize = 22;
        }
        nets = new List<NeuralNetwork>();
        for (int i = 0; i < populationSize; i++) {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate();
            nets.Add(net);
        }
    }

    private float conversao(float X, float Xmax, float Xmin, float Ymax, float Ymin) {
        return Ymin + (((X - Xmin) / (Xmax - Xmin)) * (Ymax - Ymin));
    }

    private void visualizarMelhor(){
        oncebest[0] = true;
        if (visualizacao){
            oncebest[1] = true;
        }else{
            oncebest[1] = false;
        }
        best = true;
        visualizacao = true;
        ct = 0;
        bestNet = nets[nets.Count - 1];
    }

    void iteracaoRedeNeural(){
        if (populationIterator == populationSize || generationNumber == 0){ //Verificar se não desregulou o fitness na visualização
            if (generationNumber == 0){
                InitArgamassaNeuralNetworks();
                generationNumber++;
                if (!novasRedes){
                    for (int i = 0; i < populationSize; i++){
                        nets[i].SetWeight(consulta(i));
                    }
                    print("Recuperou redes.");
                }
                novasRedes = false;
            }else{
                nets.Sort();
                fitnesses.Sort();
                avaliarTotal();
                print("avaliarTotal");

                if (verMelhorDaGeração && !visualizacao)
                    visualizarMelhor();

                //print("Pior rede: " + nets[0].GetFitness());
                //print("Melhor rede: " + nets[nets.Count - 1].GetFitness());
                print("ciclo");
                string redes = "";
                for (int i = 0; i < populationSize; i++){
                    redes += " " + nets[i].GetFitness(); //+ "|" + nets[i].nome;
                }
                print("saida: " + redes);

                print(deletar());

                for (int i = 0; i < populationSize; i++){
                    inserir(i);
                    nets[i].SetFitness(0f);
                }

                for (int i = 0; i < populationSize / 2; i++){
                    nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]);//, false);
                    nets[i].Mutate();
                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]);//, true); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                }

                generationNumber++;
                Text textGeracao = GameObject.Find("Geracao").GetComponent<Text>();
                textGeracao.text = "Geração " + generationNumber;
                populationIterator = 0;
                //nets[2].SetWeight(consulta(19));
            }
            //Debug.Log(consulta(19));
        }

        /*if (populationIterator == 19){
            //if (ia == 0){
            //    print(deletar());
            //    print(inserir(19));
            //    ia++;
            //}

            //Debug.Log(nets[19].consulta().ToString("0.000000000000"));
            Debug.Log(consulta(19));
            nets[19].SetFitness(0f);
        }*/
    }

    void instanciacaoPersonagem(){
        criarArgamassa();
        liberaCorpo(animals[0]);
        esticaPernaTrasVetor(animals[0]);
        esticaPernaFrenteVetor(animals[0]);
        testeColisao = animals[0].chita.transform.GetChild(13).gameObject;

    }

    void entradaSaidaRede(){
        if (animals.Count > 0){
            float[] inputs = new float[4];
            if (animals[0].torax.transform.eulerAngles.z > 180){
                inputs[3] = animals[0].torax.transform.eulerAngles.z - 360;
            }else{
                inputs[3] = animals[0].torax.transform.eulerAngles.z;
            }

            inputs[3] = conversao(inputs[3], 90f, -90f, 0.5f, -0.5f);
            inputs[2] = animals[0].torax.transform.position.y - chao.transform.position.y /*Altura do tórax*/;
            inputs[2] = conversao(inputs[2], 3.7f, 0.7f, 0.5f, -0.5f);
            inputs[1] = animals[0].PeDrb.transform.position.y - chao.transform.position.y  /*Altura da pata da frente*/;
            inputs[1] = conversao(inputs[1], 3.3f, -0.3f, 0.5f, -0.5f);
            inputs[0] = animals[0].PeErb.transform.position.y - chao.transform.position.y /*Altura da pata de trás*/;
            inputs[0] = conversao(inputs[0], 3.3f, -0.3f, 0.5f, -0.5f);

            float[] output = nets[populationIterator].FeedForward(inputs);
            if (!abordagem){
                if (output[0] > 0f){
                    esticaPernaTrasVetor(animals[0]);
                    numPressTras--;
                }

                if (output[1] > 0f){
                    esticaPernaFrenteVetor(animals[0]);
                    numPressFrente--;
                }

                if (ct % ritmo == 0f){
                    esticaPernaTrasVetor(animals[0]);
                }

                if ((ct + (ritmo / 2)) % ritmo == 0){
                    esticaPernaFrenteVetor(animals[0]);
                }
            }
            else
            {
                esticaPernaFrente(animals[0], output[0] * 180, output[1] * 180, output[2] * 180);
                esticaPernaTras(animals[0], output[3] * 180, output[4] * 180, output[5] * 180);
            }
        }
    }

    float calcularErro(int press, int limite, float punicao) {
        if (press > limite){
            return -(Mathf.Pow((press-limite), 2))-punicao;
        }
        return 0;
    }

    // Update is called once per frame
    void Update() {
        if (visualizacao == false) {
            iteracaoRedeNeural();

            Physics.autoSimulation = false;

            //if (abordagem == false){
            instanciacaoPersonagem();
            if (ct == 0){
                Component[] hingeJoints = animals[0].chita.GetComponentsInChildren<HingeJoint>();
                for (int i = 0; i < hingeJoints.Length; i++)
                    hingeJoints[i].GetComponent<Rigidbody>().velocity = velocidade;
            }

            for (ct = 1; ct <= evaluationTime; ct++){
                entradaSaidaRede();
                Physics.Simulate(Time.fixedDeltaTime);
            }

            float dist = animals[0].chita.transform.Find("Torax").transform.position.x - largada.transform.position.x;
            if (numPressFrente < 0 || numPressTras < 0) {
                float erro1 = calcularErro(-(numPressFrente - limite), limite, -100);
                float erro2 = calcularErro(-(numPressTras - limite), limite, -100);
                nets[populationIterator].SetFitness(dist + erro1 + erro2);
            }
            else if (testeColisao.GetComponent<colisao>().colidiu == true){
                print("Eliminado");
                nets[populationIterator].SetFitness(-100f);
                testeColisao.GetComponent<MeshRenderer>().material.color = new Color(100, 0, 0, 0.5f);
            }else{
                nets[populationIterator].SetFitness(dist);
            }
            //nets[populationIterator].SetFitness(numPressFrente + numPressTras);
            fitnesses.Add((nets[populationIterator].GetFitness()));
            //avaliar();
            GameObject.Destroy(animals[0].chita.gameObject);
            animals.Remove(animals[0]);
            //} else {
            //Outro método
            //}
            populationIterator++;
            Physics.autoSimulation = true;
            ct = 0;
        }
    }
    void FixedUpdate() {
        if (visualizacao) {
            if (best) {
                if (ct == 0) {
                    criarArgamassa(true);
                    liberaCorpo(bestAnimalx);
                    //print(bestAnimalx.net.GetSWeights());
                    esticaPernaTrasVetor(bestAnimalx);
                    esticaPernaFrenteVetor(bestAnimalx);
                    salvouFitness = false;
                    testeColisao = bestAnimalx.chita.transform.GetChild(13).gameObject;
                    print("Entrou com ct: "+ct);
                }
                ct++;
                //print("meu ct++: "+ct);
                //if (abordagem == false){
                if (ct == 1 && bestAnimalx.chita != null){
                    Component[] hingeJoints = bestAnimalx.chita.GetComponentsInChildren<HingeJoint>();
                    for (int i = 0; i < hingeJoints.Length; i++)
                        hingeJoints[i].GetComponent<Rigidbody>().velocity = velocidade;
                }

                if (bestAnimalx.chita != null){
                    float[] inputs = new float[4];
                    if (bestAnimalx.torax.transform.eulerAngles.z > 180){
                        inputs[3] = bestAnimalx.torax.transform.eulerAngles.z - 360;
                    }else{
                        inputs[3] = bestAnimalx.torax.transform.eulerAngles.z;
                    }
                    inputs[3] = conversao(inputs[3], 90f, -90f, 0.5f, -0.5f);
                    inputs[2] = bestAnimalx.torax.transform.position.y - chao.transform.position.y /*Altura do tórax*/;
                    inputs[2] = conversao(inputs[2], 3.7f, 0.7f, 0.5f, -0.5f);
                    inputs[1] = bestAnimalx.PeDrb.transform.position.y - chao.transform.position.y  /*Altura da pata da frente*/;
                    inputs[1] = conversao(inputs[1], 3.3f, -0.3f, 0.5f, -0.5f);
                    inputs[0] = bestAnimalx.PeErb.transform.position.y - chao.transform.position.y /*Altura da pata de trás*/;
                    inputs[0] = conversao(inputs[0], 3.3f, -0.3f, 0.5f, -0.5f);

                    float[] output = bestNet.FeedForward(inputs);
                    if (output[0] > 0f){
                        esticaPernaTrasVetor(bestAnimalx);
                    }

                    if (output[1] > 0f){
                        esticaPernaFrenteVetor(bestAnimalx);
                    }

                    if (ct % ritmo == 0){
                        esticaPernaTrasVetor(bestAnimalx);
                    }

                    if ((ct + (ritmo / 2)) % ritmo == 0){
                        esticaPernaFrenteVetor(bestAnimalx);
                    }

                    if ((ct > evaluationTime) && !salvouFitness){
                        if (testeColisao.GetComponent<colisao>().colidiu == true){
                            print("Colidiu");
                            bestNet.SetFitness(-100f);
                            //testeColisao.GetComponent<MeshRenderer>().material.color = new Color(100, 0, 0, 0.5f);
                        }else{
                            bestNet.SetFitness((bestAnimalx.chita.transform.Find("Torax").transform.position.x - largada.transform.position.x));
                        }
                        fitnesses.Add(bestNet.GetFitness());
                        //avaliar();
                        print("Fitness da best: "+ bestNet.GetFitness());
                        GameObject.Destroy(bestAnimalx.chita.gameObject);
                        salvouFitness = true;
                    }
                }

                if (ct > evaluationTime + 1){
                    ct = 0;
                    if (oncebest[0]){
                        best = false;
                        if (!oncebest[1]){
                            visualizacao = false;
                        }
                    }
                }
            } else {
                iteracaoRedeNeural();
                //if (abordagem == false){
                    if (ct == 0){
                        instanciacaoPersonagem();
                        salvouFitness = false;
                    }
                    ct++;
                    if (animals.Count > 0) {
                        if (ct == 1){
                        Component[] hingeJoints = animals[0].chita.GetComponentsInChildren<HingeJoint>();
                        for (int i = 0; i < hingeJoints.Length; i++)
                            hingeJoints[i].GetComponent<Rigidbody>().velocity = velocidade;
                    }
                        //EditorApplication.isPaused = true;

                        if ((ct > evaluationTime) && !salvouFitness){
                            float dist = animals[0].chita.transform.Find("Torax").transform.position.x - largada.transform.position.x;
                            if (numPressFrente < 0 || numPressTras < 0){
                                float erro1 = calcularErro(-(numPressFrente - limite), limite, -100);
                                float erro2 = calcularErro(-(numPressTras - limite), limite, -100);
                                nets[populationIterator].SetFitness(dist + erro1 + erro2);
                            }else if (testeColisao.GetComponent<colisao>().colidiu == true){
                                print("Colidiu");
                                nets[populationIterator].SetFitness(-100);
                                //animals[0].net.SetFitness(animals[0].chita.transform.Find("Torax").transform.position.x - largada.transform.position.x);
                                testeColisao.GetComponent<MeshRenderer>().material.color = new Color(100, 0, 0, 0.5f);
                            }else{
                                nets[populationIterator].SetFitness(dist);
                            }
                            //nets[populationIterator].SetFitness(numPressFrente+numPressTras);
                            fitnesses.Add((nets[populationIterator].GetFitness()));
                            //avaliarTotal();
                            GameObject.Destroy(animals[0].chita.gameObject);
                            animals.Remove(animals[0]);
                            //salvouFitness = true;
                            populationIterator++;
                        }else{
                            entradaSaidaRede();
                        }
                }

                    if (ct > evaluationTime + 1){
                        ct = 0;
                    }
                //}
            }
        }
    }

    public void criarArgamassa(bool melhor = false) {
        numPressTras = limite;
        numPressFrente = limite;
        if (melhor) {
            print("X - Melhor");
            bestNet.SetWeight(consulta(21));
            bestAnimalx = (new Animalx(GameObject.Instantiate(chita)));
            bestAnimalx.chita.transform.position = new Vector3(-2.16f, -0.67f, -(2f * 1));
            bestAnimalx.chita.name = "Melhor";
        } else {
            animals.Add(new Animalx(GameObject.Instantiate(chita)));
            animals[animals.Count - 1].chita.transform.position = new Vector3(-2.16f, -0.67f, -(2f * animals.Count));
            animals[animals.Count - 1].chita.name = "AAA" + populationIterator;
        }
    }

    public void liberaCorpo(Animalx animal) {
        foreach (Transform child in animal.chita.transform) {
            child.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void avaliar() {
        int i = 0;
        if (!salvouFitness) {
            Text tx = GameObject.Find("TextA" + (i + 0)).GetComponent<Text>();
            tx.text = ("Fit atual: " + fitnesses[fitnesses.Count - 1]);
            Text txFts = GameObject.Find("TextA" + (i + 1)).GetComponent<Text>();
            txFts.text += " " + fitnesses[fitnesses.Count - 1];
            if (populationIterator == populationSize - 2) {
                txFts.text += "\n|G: " + (generationNumber+1)+ "|\n";
            }
        }
    }

    public void avaliarTotal(){
        Text tx = GameObject.Find("TextA0").GetComponent<Text>();
        tx.text = ("Fit atual: " + fitnesses[fitnesses.Count - 1]);
        Text txFts = GameObject.Find("TextA1").GetComponent<Text>();
        for (int i = 0; i < populationSize; i++) {
            txFts.text += " " + fitnesses[i];
            if (i%4 == 3) {
                txFts.text += "\n";
            }
        }
        txFts.text += "\n|G: " + (generationNumber + 1) + "|\n";
        fitnesses.Clear();
    }


    public void inicializaDB(){
        IDbConnection _connection = new SqliteConnection(urlDataBase);
        _command = _connection.CreateCommand();
        _connection.Open();
        //string sql = "CREATE TABLE IF NOT EXISTS highscores (name VARCHAR(20), score INT)";
        string sql = "CREATE TABLE IF NOT EXISTS redes_neurais (id int, layers VARCHAR(350), neurons VARCHAR(1550), weights VARCHAR(5550), visitou boolean)";
        _command.CommandText = sql;
        _command.ExecuteNonQuery();
    }

    public string inserir(int id){
        string sql = "INSERT INTO redes_neurais (id, layers, weights) values(" + id + ", '" + nets[id].GetSLayers() + "','" + nets[id].GetSWeights() + "')";
        print("X: " + id + ", '" + nets[id].GetSWeights());
        _command.CommandText = sql;
        _command.ExecuteNonQuery();
        print("Rede inserida: " + nets[id].GetSWeights());
        return ("Inseriu");
    }

    public string consultaPesos(int rede){
        string sql = "SELECT weights" + " FROM redes_neurais where id = " + rede;
        _command.CommandText = sql;
        IDataReader reader = _command.ExecuteReader();
        while (reader.Read()){
            return reader.GetString(2);
        }

        return null;
    }
    public float[][][] consulta(int rede){
        string sql = "SELECT id, layers, weights" + " FROM redes_neurais where id = "+rede;
        _command.CommandText = sql;
        IDataReader reader = _command.ExecuteReader();

        float[][][] weights = null;
        while (reader.Read()){
            string weigh = reader.GetString(2);
            string[] weightSplit = weigh.Split(new string[] { " Y " }, StringSplitOptions.None);
            List<float[][]> weightsList = new List<float[][]>();
            for (int i = 0; i < weightSplit.Length - 1; i++){
                List<float[]> layerWeightsList = new List<float[]>();
                int neuronsInPreviousLayer = layers[i];
                //4
                string[] xSplit = weightSplit[i].Split(new string[] { " X " }, StringSplitOptions.None);
                for (int j = 0; j < xSplit.Length - 1; j++){
                    float[] neuronWeights = new float[neuronsInPreviousLayer]; //neruons weights
                                                                               //5
                    string[] ySplit = xSplit[j].Split(new string[] { " " }, StringSplitOptions.None);
                    for (int k = 0; k < ySplit.Length - 1; k++){
                        //4
                        float result;
                        if (float.TryParse(ySplit[k], out result)){
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
        //Debug.Log("Recuperou do banco");

        return weights;
    }

    public string atualizar()
    {
        //string sql = "UPDATE redes_neurais set id = 1 WHERE id != 0";
        //_command.CommandText = sql;
        //_command.ExecuteNonQuery();
        return ("Atualizou!");
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
            JointSpring springJ1 = animal.CoxaDrb.GetComponent<HingeJoint>().spring; springJ1.targetPosition = -45;
            animal.CoxaDrb.GetComponent<HingeJoint>().spring = springJ1;
            JointSpring springJ2 = animal.CanelaDrb.GetComponent<HingeJoint>().spring; springJ2.targetPosition = 0;
            animal.CanelaDrb.GetComponent<HingeJoint>().spring = springJ2;
            JointSpring springJ3 = animal.PeDrb.GetComponent<HingeJoint>().spring; springJ3.targetPosition = 0;
            animal.PeDrb.GetComponent<HingeJoint>().spring = springJ3;
        }else{
            JointSpring springJ1 = animal.CoxaDrb.GetComponent<HingeJoint>().spring; springJ1.targetPosition = 40;
            animal.CoxaDrb.GetComponent<HingeJoint>().spring = springJ1;
            JointSpring springJ2 = animal.CanelaDrb.GetComponent<HingeJoint>().spring; springJ2.targetPosition = -90;
            animal.CanelaDrb.GetComponent<HingeJoint>().spring = springJ2;
            JointSpring springJ3 = animal.PeDrb.GetComponent<HingeJoint>().spring; springJ3.targetPosition = 100;
            animal.PeDrb.GetComponent<HingeJoint>().spring = springJ3;
        }
        animal.pernaFrente = !animal.pernaFrente;
    }

    public void esticaPernaTrasVetor(Animalx animal){
        if (animal.pernaTras == false){
            JointSpring springJ1 = animal.CoxaErb.GetComponent<HingeJoint>().spring; springJ1.targetPosition = -45;
            animal.CoxaErb.GetComponent<HingeJoint>().spring = springJ1;
            JointSpring springJ2 = animal.CanelaErb.GetComponent<HingeJoint>().spring; springJ2.targetPosition = 0;
            animal.CanelaErb.GetComponent<HingeJoint>().spring = springJ2;
            JointSpring springJ3 = animal.PeErb.GetComponent<HingeJoint>().spring; springJ3.targetPosition = 0;
            animal.PeErb.GetComponent<HingeJoint>().spring = springJ3;
        }else{
            JointSpring springJ1 = animal.CoxaErb.GetComponent<HingeJoint>().spring; springJ1.targetPosition = 40;
            animal.CoxaErb.GetComponent<HingeJoint>().spring = springJ1;
            JointSpring springJ2 = animal.CanelaErb.GetComponent<HingeJoint>().spring; springJ2.targetPosition = -90;
            animal.CanelaErb.GetComponent<HingeJoint>().spring = springJ2;
            JointSpring springJ3 = animal.PeErb.GetComponent<HingeJoint>().spring; springJ3.targetPosition = 100;
            animal.PeErb.GetComponent<HingeJoint>().spring = springJ3;
        }
        animal.pernaTras = !animal.pernaTras;
    }

    public void esticaPernaFrente(Animalx animal, float coxa, float canela, float pe){
        coxa = (360 + coxa) % 360;
        canela = (360 + canela) % 360;
        pe = (360 + pe) % 360;
        JointSpring springJ1 = animal.CoxaDrb.GetComponent<HingeJoint>().spring; springJ1.targetPosition = Mathf.Floor(coxa);
        animal.CoxaDrb.GetComponent<HingeJoint>().spring = springJ1;
        JointSpring springJ2 = animal.CanelaDrb.GetComponent<HingeJoint>().spring; springJ2.targetPosition = Mathf.Floor(canela);
        animal.CanelaDrb.GetComponent<HingeJoint>().spring = springJ2;
        JointSpring springJ3 = animal.PeDrb.GetComponent<HingeJoint>().spring; springJ3.targetPosition = Mathf.Floor(pe);
        animal.PeDrb.GetComponent<HingeJoint>().spring = springJ3;
    }

    public void esticaPernaTras(Animalx animal, float coxa, float canela, float pe){
        coxa = (360 + coxa) % 360;
        canela = (360 + canela) % 360;
        pe = (360 + pe) % 360;
        JointSpring springJ1 = animal.CoxaErb.GetComponent<HingeJoint>().spring; springJ1.targetPosition = Mathf.Floor(coxa);
        animal.CoxaErb.GetComponent<HingeJoint>().spring = springJ1;
        JointSpring springJ2 = animal.CanelaErb.GetComponent<HingeJoint>().spring; springJ2.targetPosition = Mathf.Floor(canela);
        animal.CanelaErb.GetComponent<HingeJoint>().spring = springJ2;
        JointSpring springJ3 = animal.PeErb.GetComponent<HingeJoint>().spring; springJ3.targetPosition = Mathf.Floor(pe);
        animal.PeErb.GetComponent<HingeJoint>().spring = springJ3;
    }

    //void FixedUpdate(){
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