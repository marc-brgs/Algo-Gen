using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int popSize = 10;
    public GameObject cityPrefab;
    
    private int gen = 0;
    private List<Vector3> currentCities = new List<Vector3>();
    private List<Vector3> bestCities = new List<Vector3>();
    private int bestGen = 0;
    private float bestFitness = 100000f;
    
    [SerializeField] private LineRenderer currentLine;
    [SerializeField] private LineRenderer bestLine;

    public float MAX_WIDTH;
    public float MAX_HEIGHT;
    
    // Start is called before the first frame update
    void Start()
    {
        InitiatePopulation();
        Debug.Log(fitness());
    }

    // Update is called once per frame
    void Update()
    {
        // Sélection
        List<int> selectedCitiesIndex;
        selectedCitiesIndex = Selection();
        
        // Croisement
        Croisement(selectedCitiesIndex);
        
        // Mutation
        Mutation();
        
        // Evaluation de la solution finale
        float currentFitness = fitness();
        if (currentFitness < bestFitness)
        {
            bestFitness = fitness();
            bestGen = gen;

            bestCities = currentCities;
            bestLine.SetPositions(bestCities.ToArray());
        }
        
        currentLine.SetPositions(currentCities.ToArray()); // Update line renderer
        gen++;
    }

    private void InitiatePopulation()
    {
        for (int i = 0; i < popSize; i++)
        {
            float xPos = Random.Range(-MAX_WIDTH, MAX_WIDTH);
            float zPos = Random.Range(-MAX_HEIGHT, MAX_HEIGHT);

            Instantiate(cityPrefab, new Vector3(xPos, 0f, zPos), new Quaternion(0f, 0f, 0f, 0f)); // cities cubes
            
            currentCities.Add(new Vector3(xPos, 0f, zPos));
        }

        bestCities = currentCities;
        
        currentLine.positionCount = popSize;
        currentLine.SetPositions(currentCities.ToArray());
        
        bestLine.positionCount = popSize;
        bestLine.SetPositions(bestCities.ToArray());
    }

    /*
     * Return total length of the path
     */
    private float fitness()
    {
        float pathLength = 0f;
        for (int i = 0; i < popSize - 1; i++)
        {
            pathLength += Vector3.Distance(currentCities[i], currentCities[i + 1]);
        }
        
        // Close path loop
        if (popSize > 1)
        {
            pathLength += Vector3.Distance(currentCities[popSize - 1], currentCities[0]); 
        }
        

        return pathLength;
    }

    private List<int> Selection()
    {
        // Random
        int indexP1 = Random.Range(0, currentCities.Count);
        int indexP2 = Random.Range(0, currentCities.Count);

        return new List<int>(new []{indexP1, indexP2});
    }

    // Permute les 2 index de currentCities
    private void Croisement(List<int> selectedCitiesIndex)
    {
        (currentCities[selectedCitiesIndex[0]], currentCities[selectedCitiesIndex[1]]) = (currentCities[selectedCitiesIndex[1]], currentCities[selectedCitiesIndex[0]]);
    }

    private void Mutation()
    {
        
    }
}
