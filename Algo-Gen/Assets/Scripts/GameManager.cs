using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int popSize = 10;
    public GameObject cityPrefab;
    
    private List<Vector3> cities;
    [SerializeField] private LineRenderer lineRenderer;

    public float MAX_WIDTH;
    public float MAX_HEIGHT;
    
    // Start is called before the first frame update
    void Start()
    {
        cities = new List<Vector3>();
        InitiatePopulation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitiatePopulation()
    {
        for (int i = 0; i < popSize; i++)
        {
            float xPos = Random.Range(-MAX_WIDTH, MAX_WIDTH);
            float zPos = Random.Range(-MAX_HEIGHT, MAX_HEIGHT);

            Instantiate(cityPrefab, new Vector3(xPos, 0f, zPos), new Quaternion(0f, 0f, 0f, 0f));
            
            cities.Add(new Vector3(xPos, 0f, zPos));
        }
        lineRenderer.positionCount = popSize;
        lineRenderer.SetPositions(cities.ToArray());
    }
}
