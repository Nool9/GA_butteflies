using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// keep track of population, breed new populations, display stats

public class PopulationManager : MonoBehaviour
{
    public GameObject butterflyPrefab;
    public float simulationSpeed;
    public int populationSize;
    public int trialTime;

    public Transform xMinBorder;
    private float xMinSpawn;
    public Transform xMaxBorder;
    private float xMaxSpawn;
    public Transform yMinBorder;
    private float yMinSpawn;
    public Transform yMaxBorder;
    private float yMaxSpawn;
    public Transform zMinBorder;
    private float zMinSpawn;
    public Transform zMaxBorder;
    private float zMaxSpawn;

    private int generation;
    private float averageFitness;
    private float bestFitness;
    private float totalFitness;

    public int selectionFactor;

    public static float elapsed; 

    List<GameObject> population = new List<GameObject>();

    GUIStyle gUIStyle = new GUIStyle();


    // draw stats
    void OnGUI()
    {
        gUIStyle.fontSize = 20;
        gUIStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 100, 20), "Generation: " + generation, gUIStyle);
        GUI.Label(new Rect(10, 30, 100, 20), "Trial Time: " + (int)elapsed, gUIStyle);
        GUI.Label(new Rect(10, 50, 100, 20), "Population: " + population.Count, gUIStyle);
        GUI.Label(new Rect(10, 70, 100, 20), "Average Fitness: " + averageFitness, gUIStyle);
        GUI.Label(new Rect(10, 90, 100, 20), "Best Fitness: " + bestFitness, gUIStyle);
    }

    // set area, simulation speed and breed generation 0
    void Start()
    {
        SetSpawnArea();
        Time.timeScale = simulationSpeed;

        for (int i = 0; i < populationSize; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(xMinSpawn, xMaxSpawn), Random.Range(yMinSpawn, yMaxSpawn), Random.Range(zMinSpawn, zMaxSpawn));
            GameObject buttefly = Instantiate(butterflyPrefab, spawnPos, butterflyPrefab.transform.rotation);
            buttefly.GetComponent<Brain>().Init();
            population.Add(buttefly);
        }
    }

    // get positions for spawn area
    private void SetSpawnArea()
    {
        xMinSpawn = xMinBorder.position.x;
        xMaxSpawn = xMaxBorder.position.x;

        yMinSpawn = yMinBorder.position.y;
        yMaxSpawn = yMaxBorder.position.y;

        zMinSpawn = zMinBorder.position.z;
        zMaxSpawn = zMaxBorder.position.z;
    }

    // sort population by fitness, calculate fitness stats, breed new population using "Breed"
    private void BreedNewPopulation()
    {     
        List<GameObject> sortedList = population.OrderByDescending(o => o.GetComponent<Brain>().timeAlive + o.GetComponent<Brain>().distanceTraveled).ToList();

        CalculateAverageFitness();
        bestFitness = sortedList[0].GetComponent<Brain>().timeAlive + sortedList[0].GetComponent<Brain>().distanceTraveled;

        population.Clear();

        // get top X% of population, crossbreed
        for (int i = 0; i < sortedList.Count / selectionFactor; i++)
        {
            for (int j = 0; j < selectionFactor / 2; j++)
            {
                population.Add(Breed(sortedList[i], sortedList[i + 1]));
                population.Add(Breed(sortedList[i + 1], sortedList[i]));
            }
        }

        // destroy old population
        for (int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }

        generation++;
    }

    private void CalculateAverageFitness()
    {
        for (int i = 0; i < population.Count; i++)
        {
            totalFitness += population[i].GetComponent<Brain>().timeAlive;
        }

        averageFitness = totalFitness / population.Count;
        totalFitness = 0;
    }

    // combine parent genes or mutate randomly
    private GameObject Breed(GameObject mother, GameObject father)
    {
        Vector3 spawnPos = new Vector3(Random.Range(xMinSpawn, xMaxSpawn), Random.Range(yMinSpawn, yMaxSpawn), Random.Range(zMinSpawn, zMaxSpawn));
        GameObject child = Instantiate(butterflyPrefab, spawnPos, butterflyPrefab.transform.rotation);
        child.GetComponent<Brain>().Init();

        // mutate
        if (Random.Range(0,100) == 1)
        {
            child.GetComponent<Brain>().dna.Mutate();
        }

        // combine
        else
        {
            child.GetComponent<Brain>().dna.Combine(mother.GetComponent<Brain>().dna, father.GetComponent<Brain>().dna);
        }

        return child;
    }

    // check trial time and breed new population if elapsed
    void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed >= trialTime)
        {    
            BreedNewPopulation();
            elapsed = 0;
        }
    }
}
