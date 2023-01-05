using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public int numVille = 100;
    
    public int popSize = 50;
    
    public GameObject cityPrefab;

    private List<Vector3> villes;
    public float tauxMutation = 0.015f;
    public int tailleTournoi = 5;
    public bool elitisme = true;
    
    private int generation = 0;
    private List<Vector3> currentCities = new List<Vector3>();
    private List<Vector3> bestCities = new List<Vector3>();
    private int bestGen = 0;
    private float bestFitness = 100000f;
    
    [SerializeField] private LineRenderer currentLine;
    [SerializeField] private LineRenderer bestLine;

    public float MAX_WIDTH;
    public float MAX_HEIGHT;

    private Population pop;
    private List<Vector3> currentPopFittest;
    private List<Vector3> fittest;

    public TextMeshProUGUI textBestGen;
    public TextMeshProUGUI textBestFitness;
    public TextMeshProUGUI textCurrentGen;
    
    private void Awake() 
    {
        if (instance != null && instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            instance = this;
        } 
    }
    
    // Start is called before the first frame update
    void Start()
    {
        InitiatePopulation();

        pop = new Population(villes, popSize, true);
        pop = EvolePopulation(pop);
        generation++;
    }

    // Update is called once per frame
    void Update()
    {
        pop = EvolePopulation(pop);
        generation++;

        currentPopFittest = pop.individus[Random.Range(0, pop.individus.Count)];
        if (GetFitness(currentPopFittest) > GetFitness(fittest))
        {
            fittest = pop.GetFittest();
            bestFitness = GetFitness(fittest);
            bestGen = generation;
            textBestGen.text = "BEST GEN: " + bestGen;
            textBestFitness.text = "BEST FITNESS: " + bestFitness;
        }
        textCurrentGen.text = "CURRENT GEN: " + generation;
            
        UpdateLines();
    }

    private void UpdateLines()
    {
        currentLine.positionCount = numVille;
        currentLine.SetPositions(currentPopFittest.ToArray());
        
        bestLine.positionCount = numVille;
        bestLine.SetPositions(fittest.ToArray());
    }
    
    private void InitiatePopulation()
    {
        for (int i = 0; i < numVille; i++)
        {
            float xPos = Random.Range(-MAX_WIDTH, MAX_WIDTH);
            float zPos = Random.Range(-MAX_HEIGHT, MAX_HEIGHT);

            Instantiate(cityPrefab, new Vector3(xPos, 0f, zPos), new Quaternion(0f, 0f, 0f, 0f)); // cities cubes
            
            currentCities.Add(new Vector3(xPos, 0f, zPos));
        }
        
        bestCities = currentCities;
        villes = currentCities;
        
        // Affichage
        currentLine.positionCount = numVille;
        currentLine.SetPositions(currentCities.ToArray());
        
        bestLine.positionCount = numVille;
        bestLine.SetPositions(bestCities.ToArray());
    }
    
    public Population EvolePopulation(Population pop)
    {
        Population newPop = new Population(villes, pop.individus.Count);
        int elitismeOffset = 0;
        if (elitisme)
        {
            newPop.individus[0] = pop.GetFittest();
            elitismeOffset = 1;
        }
        
        // Selection et crossover
        for (int i = elitismeOffset; i < newPop.individus.Count; i++)
        {
            List<Vector3> parent1 = SelectionTournoi(pop);
            // parent1[parent1.Count - 1] = new Vector3(0f, 0f, 0f);
            List<Vector3> parent2 = SelectionTournoi(pop);
            List<Vector3> enfant = Crossover(parent1, parent2);
            newPop.individus[i] = enfant;
        }
        
        // Mutation
        for (int i = elitismeOffset; i < newPop.individus.Count; i++)
        {
            Mutation(newPop.individus[i]);
        }
        
        return newPop;
    }

    private List<Vector3> Crossover(List<Vector3> parent1, List<Vector3> parent2)
    {
        List<Vector3?> enfant = new List<Vector3?>();

        /*Debug.Log("PARENT1:");
        for (int i = 0; i < parent1.Count; i++)
        {
            Debug.Log(parent1[i].ToString());
        }

        Debug.Log("PARENT2:");
        for (int i = 0; i < parent2.Count; i++)
        {
            Debug.Log(parent2[i].ToString());
        }*/
        
        Vector3? nullableVec = null;
        
        for (int i = 0; i < villes.Count; i++)
        {
            enfant.Add(nullableVec);
        }

        int startPos = Random.Range(0, parent1.Count);
        int endPos = Random.Range(0, parent1.Count);

        for (int i = 0; i < enfant.Count; i++)
        {
            if (startPos < endPos && i > startPos && i < endPos)
            {
                enfant[i] = parent1[i];
            }
            else if(startPos > endPos)
            {
                if (!(i < startPos && i > endPos))
                {
                    enfant[i] = parent1[i];
                }
            }
        }

        for (int i = 0; i < parent2.Count; i++)
        {
            if (!(enfant.Contains(parent2[i])))
            {
                for (int j = 0; j < enfant.Count; j++)
                {
                    if (enfant[j] == null)
                    {
                        enfant[j] = parent2[i];
                        break;
                    }
                }
            }
        }

        /*Debug.Log("ENFANT");
        for (int i = 0; i < enfant.Count; i++)
        {
            Debug.Log(enfant[i].ToString());
        }*/
        
        return enfant.Cast<Vector3>().ToList(); // Cast to non nullable Vector3
    }
    
    private void Mutation(List<Vector3> individu)
    {
        if (!(Random.Range(0f, 1f) < tauxMutation) && tauxMutation < 1f) return;
        
        // Double permutation
        if (tauxMutation >= 1f)
        {
            int ville1Index = Random.Range(0, individu.Count);
            int ville1IndexPlus = Random.Range(0, individu.Count);
            
            int ville2Index = Random.Range(0, individu.Count);
            int ville2IndexPlus = Random.Range(0, individu.Count);
            
            (individu[ville1Index], individu[ville2Index]) = (individu[ville2Index], individu[ville1Index]);
            (individu[ville1IndexPlus], individu[ville2IndexPlus]) = (individu[ville2IndexPlus], individu[ville1IndexPlus]);
        }
        else // simple permutation
        {
            int ville1Index = Random.Range(0, individu.Count);
            int ville2Index = Random.Range(0, individu.Count);
            (individu[ville1Index], individu[ville2Index]) = (individu[ville2Index], individu[ville1Index]);
        }
    }

    /**
     * Return the best solution from a sample of the current population
     */
    private List<Vector3> SelectionTournoi(Population pop)
    {
        Population tournoi = new Population(villes, tailleTournoi);
        
        for (int i = 0; i < tailleTournoi; i++)
        {
            int villeIndex = Random.Range(0, pop.individus.Count);
            tournoi.individus[i] = pop.individus[villeIndex];
            // Debug.Log("SELECTION: " + tournoi.individus[i].ToString());
        }

        return tournoi.GetFittest();
    }
    
    private List<Vector3> SelectionRoulette(Population pop)
    {
        // Calcule la somme totale des fitness
        float totalFitness = 0;
        for(int i = 0; i < pop.individus.Count; i++)
        {
            totalFitness += GetFitness(pop.individus[i]);
        }

        // Sélectionne numIndividuals individus par roulette
        List<Vector3> selectedIndividuals = new List<Vector3>();

        // Génère un nombre aléatoire compris entre 0 et la somme totale des fitness
        float randomFitness = Random.Range(0, totalFitness);

        // Parcoure la liste des individus et sélectionne l'individu actuel si sa fitness cumulée dépasse
        // le nombre aléatoire généré
        float cumulativeFitness = 0;
        for(int i = 0; i < pop.individus.Count; i++) {
            cumulativeFitness += GetFitness(pop.individus[i]);
            if (cumulativeFitness > randomFitness)
            { 
                selectedIndividuals = pop.individus[i];
                break;
            }
        }

        return selectedIndividuals;
    }
    
    /*
     * Return score of path which is equivalent to 1 / total length of the path
     */
    public float GetFitness(List<Vector3> individu)
    {
        if (individu == null) return 0f;
        float pathLength = 0f;
        for (int i = 0; i < individu.Count - 1; i++)
        {
            pathLength += Vector3.Distance(individu[i], individu[i + 1]);
        }
        
        // Close path loop
        if (individu.Count > 1)
        {
            pathLength += Vector3.Distance(individu[individu.Count - 1], individu[0]); 
        }
        
        return 1 / pathLength;
    }
    
    /**
     * Shuffle
     */
    public List<Vector3> GenererIndividu(List<Vector3> villes)
    {
        List<Vector3> vTemp = villes;
        for (int i = 0; i < vTemp.Count; i++) {
            Vector3 temp = vTemp[i];
            int randomIndex = Random.Range(i, vTemp.Count);
            vTemp[i] = vTemp[randomIndex];
            vTemp[randomIndex] = temp;
        }

        return vTemp;
    }
}
