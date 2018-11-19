using System.Collections.Generic;
using System;

/// <summary>
/// Neural Network C# (Unsupervised)
/// </summary>
public class NeuralNetwork : IComparable<NeuralNetwork>{
    private int[] layers; //layers
    private string sLayer = "";
    private float[][] neurons; //neuron matix
    private float[][][] weights = null; //weight matrix
    private string sWeight = "";
    private float fitness; //fitness of the network
    //private static string[] m_HardCodedStrings = new string[]
    //         {
    //             "A","B","C","D","E","F","G","H","I","J","K","L","M","n","O","P","Q","R","S","T","U","V", "W", "X", "Y","Z"
    //         };
    //public string nome = "";

    //public string randdd(){
    //    return m_HardCodedStrings[UnityEngine.Random.Range(0, m_HardCodedStrings.Length)]
    //        + UnityEngine.Random.Range(0, 9)
    //        + m_HardCodedStrings[UnityEngine.Random.Range(0, m_HardCodedStrings.Length)];
    //}

    //public void carregarRedeNeural(){

    //}
    /// <summary>
    /// Initilizes and neural network with random weights
    /// </summary>
    /// <param name="layers">layers to the neural network</param>
    public NeuralNetwork(int[] layers){
        //nome = randdd();
        //deep copy of layers of this network 
        this.layers = new int[layers.Length];
        sLayer = "";
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
            sLayer += this.layers[i] + " ";
        }

        //generate matrix
        InitNeurons();
        InitWeights();
    }

    /// <summary>
    /// Deep copy constructor 
    /// </summary>
    /// <param name="copyNetwork">Network to deep copy</param>
    public NeuralNetwork(NeuralNetwork copyNetwork){//, bool flag){
        //if (flag){
        //    this.nome = copyNetwork.nome;
        //}
        //else {
        //    this.nome = copyNetwork.nome + UnityEngine.Random.Range(0, 30);
        //}
        this.layers = new int[copyNetwork.layers.Length];
        sLayer = "";
        for (int i = 0; i < copyNetwork.layers.Length; i++){
            this.layers[i] = copyNetwork.layers[i];
            sLayer += this.layers[i] + " ";
        }
        InitNeurons();
        InitWeights();
        CopyWeights(copyNetwork.weights);
    }

    private void CopyWeights(float[][][] copyWeights){
        sWeight = "";
        for (int i = 0; i < weights.Length; i++){
            for (int j = 0; j < weights[i].Length; j++){
                for (int k = 0; k < weights[i][j].Length; k++){
                    weights[i][j][k] = copyWeights[i][j][k];
                    sWeight += weights[i][j][k] + " ";
                }
                sWeight += " X ";
            }
            sWeight += " Y ";
        }
    }

    /// <summary>
    /// Create neuron matrix
    /// </summary>
    private void InitNeurons(){
        //Neuron Initilization
        List<float[]> neuronsList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++){ //run through all layers
            neuronsList.Add(new float[layers[i]]); //add layer to neuron list
        }
        neurons = neuronsList.ToArray(); //convert list to array
    }

    /// <summary>
    /// Create weights matrix.
    /// </summary>
    private void InitWeights(){
        List<float[][]> weightsList = new List<float[][]>(); //weights list which will later will converted into a weights 3D array
        sWeight = "";

        //itterate over all neurons that have a weight connection
        for (int i = 1; i < layers.Length; i++){
            List<float[]> layerWeightsList = new List<float[]>(); //layer weight list for this current layer (will be converted to 2D array)

            int neuronsInPreviousLayer = layers[i - 1];

            //itterate over all neurons in this current layer
            for (int j = 0; j < neurons[i].Length; j++){
                float[] neuronWeights = new float[neuronsInPreviousLayer]; //neruons weights

                //itterate over all neurons in the previous layer and set the weights randomly between 0.5f and -0.5
                for (int k = 0; k < neuronsInPreviousLayer; k++){
                    //give random weights to neuron weights
                    neuronWeights[k] = Truncar(UnityEngine.Random.Range(-0.5f, 0.5f));
                    sWeight += neuronWeights[k] + " ";
                }

                layerWeightsList.Add(neuronWeights); //add neuron weights of this current layer to layer weights
                sWeight += " X ";
            }

            weightsList.Add(layerWeightsList.ToArray()); //add this layers weights converted into 2D array into weights list
            sWeight += " Y ";
        }

        weights = weightsList.ToArray(); //convert to 3D array
    }

    /// <summary>
    /// Feed forward this neural network with a given input array
    /// </summary>
    /// <param name="inputs">Inputs to network</param>
    /// <returns></returns>
    public float[] FeedForward(float[] inputs){
        //Add inputs to the neuron matrix
        for (int i = 0; i < inputs.Length; i++){
            neurons[0][i] = inputs[i];
        }

        //itterate over all neurons and compute feedforward values 
        for (int i = 1; i < layers.Length; i++){
            for (int j = 0; j < neurons[i].Length; j++){
                float value = 0f;
                for (int k = 0; k < neurons[i - 1].Length; k++){
                    value += weights[i - 1][j][k] * neurons[i - 1][k]; //sum off all weights connections of this neuron weight their values in previous layer
                }
                neurons[i][j] = (float)Math.Tanh(value); //Hyperbolic tangent activation
            }
        }
        return neurons[neurons.Length - 1]; //return output layer
    }
    private float Truncar(float numero){
        // float truncated = (float)(Math.Truncate((double)f*100.0) / 100.0);
        // float rounded = (float)(Math.Round((double)f, 2);
        return (float)(Math.Truncate((double)numero * 100.0) / 100000.0);
    }
    /// <summary>
    /// Mutate neural network weights
    /// </summary>
    public void Mutate(){
        sWeight = "";
        for (int i = 0; i < weights.Length; i++){
            for (int j = 0; j < weights[i].Length; j++){
                for (int k = 0; k < weights[i][j].Length; k++){
                    float weight = weights[i][j][k];

                    //mutate weight value 
                    float randomNumber = UnityEngine.Random.Range(0f, 100f);
                    randomNumber = Truncar(randomNumber);

                    if (randomNumber <= 2f){ //if 1
                      //flip sign of weight
                        weight *= -1f;
                    }else if (randomNumber <= 4f){ //if 2
                      //pick random weight between -1 and 1
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                        weight = Truncar(weight);
                    }
                    else if (randomNumber <= 6f){ //if 3
                      //randomly increase by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                        weight *= factor;
                        weight = Truncar(weight);
                    }
                    else if (randomNumber <= 8f){ //if 4
                      //randomly decrease by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f);
                        weight *= factor;
                        weight = Truncar(weight);
                    }

                    weights[i][j][k] = weight;
                    sWeight += weights[i][j][k] + " ";
                }
                sWeight += " X ";
            }
            sWeight += " Y ";
        }
    }

    public void AddFitness(float fit){
        fitness += fit;
    }

    public void SetFitness(float fit)
    {
        fitness = fit;
    }

    public void SetWeight(float[][][] weights){
        if (this.weights == null){
            InitWeights();
        }
        CopyWeights(weights);
    }

    public float GetFitness()
    {
        return fitness;
    }

    public string GetSLayers()
    {
        return sLayer;
    }

    public string GetSWeights(){
        return sWeight;
    }

    public float[][][] GetWeights(){
        return weights;
    }

    public float[][] GetNeurons(){
        return neurons;
    }




    /// <summary>
    /// Compare two neural networks and sort based on fitness
    /// </summary>
    /// <param name="other">Network to be compared to</param>
    /// <returns></returns>
    public int CompareTo(NeuralNetwork other){
        if (other == null) return 1;

        if (fitness > other.fitness)
            return 1;
        else if (fitness < other.fitness)
            return -1;
        else
            return 0;
    }
}
