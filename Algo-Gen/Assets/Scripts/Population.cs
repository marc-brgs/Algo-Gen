using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population
{
    private GameManager GM = GameManager.instance;
    
    public List<List<Vector3>> individus;

    public Population(List<Vector3> villes, int taillePopulation, bool init=false)
    {
        individus = new List<List<Vector3>>();
        for (int i = 0; i < taillePopulation; i++)
        {
            if (init) { 
                List<Vector3> nouveauChemin = GM.GenererIndividu(villes);
                individus.Add(nouveauChemin);
            }
            else
            {
                individus.Add(null);
            }
        }
    }

    public List<Vector3> GetFittest()
    {
        List<Vector3> fittest = individus[0];
        for (int i = 0; i < individus.Count; i++)
        {
            if (GM.GetFitness(individus[i]) > GM.GetFitness(fittest))
            {
                fittest = individus[i];
            }
        }
        return fittest;
    }
}