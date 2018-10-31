using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerArgamassa : MonoBehaviour {

    public GameObject chitaPrefab;

    private bool isTraning = false;
    private int populationSize = 1;
    private int generationNumber = 0;
    private int[] layers = new int[] { 4, 5, 5, 6 }; //1 input and 1 output
    private List<NeuralNetwork> nets;
    private bool leftMouseDown = false;
    private List<Argamassa> animalsList = null;

    int geracao = 0;
    Text textGeracao;

    void Timer()
    {
        isTraning = false;
    }


    void Update()
    {
        if (isTraning == false){
            if (generationNumber == 0)
            {
                InitArgamassaNeuralNetworks();
            }
            else
            {
                nets.Sort();
                //mostrar fitness de todoas as geracoes
                //permitir salvar as melhores redes
                //buscar no google o pq as mesmas chitas tem comportamentos diferentes nas novas inicializacoes
                for (int i = 0; i < populationSize / 2; i++)
                {
                    nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]);//, false);
                    nets[i].Mutate();

                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]);//, true); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                }

                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f);
                }
            }


            generationNumber++;

            isTraning = true;
            Invoke("Timer", 15f);
            textGeracao = GameObject.Find("Geracao").GetComponent<Text>();
            textGeracao.text = "Geração " + geracao++;
            CreateArgamassaBodies();
        }


        if (Input.GetMouseButtonDown(0))
        {
            leftMouseDown = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            leftMouseDown = false;
        }

        if (leftMouseDown == true)
        {
            //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }


    private void CreateArgamassaBodies()
    {

        if (animalsList != null)
        {
            for (int i = 0; i < animalsList.Count; i++)
            {
                GameObject.Destroy(animalsList[i].gameObject);
            }

        }

        animalsList = new List<Argamassa>();

        for (int i = 0; i < populationSize; i++){
            Argamassa argamassa = ((GameObject)Instantiate(chitaPrefab, new Vector3(-2.16f, 0.16f, -(2f * i)), chitaPrefab.transform.rotation)).GetComponent<Argamassa>();
            print("i: "+argamassa);
            argamassa.Init(nets[i]);//, hex.transform);
            animalsList.Add(argamassa);
        }

        //
        /*animalsList.Add(new Argamassa(GameObject.Instantiate(chitaPrefab)));

        animalsList[animalsList.Count - 1].chitaPrefab.transform.position = new Vector3(-2.16f, 0.16f, -(2f * animalsList.Count));
        string name = "AAA" + animalsList.Count;
        animalsList[animalsList.Count - 1].chita.name = name;
        float r = Random.Range(0.0f, 1f);
        float g = Random.Range(0.0f, 1f);
        float b = Random.Range(0.0f, 1f);
        float t = 0;
        foreach (Transform child in animalsList[animalsList.Count - 1].chita.transform)
            child.GetComponent<MeshRenderer>().material.color = new Color(r, g, b, t);
        //num++;
        print("Tamanho do vetor: " + animalsList.Count);

        //deleção
        *//*if (animals != null)
        {
            for (int i = 0; i < animals.Count; i++)
            {
                GameObject.Destroy(animals[i].gameObject);
            }

        }*//*

        animals = new List<Argamassa>();

        for (int i = 0; i < populationSize; i++)
        {
            Argamassa argamassa = ((GameObject)Instantiate(chita, new Vector3(-2.16f, 0.16f, UnityEngine.Random.Range(-10f, 10f)), boomerPrefab.transform.rotation)).GetComponent<Argamassa>();
            argamassa.Init(nets[i], hex.transform);
            animals.Add(argamassa);
        }*/
    }


    void InitArgamassaNeuralNetworks()
    {
        //population must be even, just setting it to 20 incase it's not
        if (populationSize % 2 != 0)
        {
            populationSize = 20;
        }

        nets = new List<NeuralNetwork>();


        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate();
            nets.Add(net);
        }
    }
}
