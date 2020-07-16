using System.Collections.Generic;
using UnityEngine;

// stores instructions that get passed by between generations and are interpreted by Brain

public class DNA 
{
    private float minimumFlapAngle;
    private float maximumFlapAngle;

    private float minimumFlapRate;
    private float maximumFlapRate;

    List<float> genes = new List<float>();
    private int dnaLenght;

    // set dna parameters (initialised from brain)
    public DNA(int l, float minFA, float maxFA, float minFR, float maxFR)
    {
        dnaLenght = l;

        minimumFlapAngle = minFA;
        maximumFlapAngle = maxFA;

        minimumFlapRate = minFR;
        maximumFlapRate = maxFR;

        SetRandomSequence();
    }

    // set initial random values for each gene
    private void SetRandomSequence()
    {
        for(int i = 0; i < dnaLenght/2; i++)
        {
            genes.Add(Random.Range(minimumFlapAngle, maximumFlapAngle));
        }

        for(int i = dnaLenght/2; i <dnaLenght; i++)
        {
            genes.Add(Random.Range(minimumFlapRate, maximumFlapRate));
        }
    }

    // swap genes - 50% chance to get gene from mother of father
    public void Combine(DNA mother, DNA father)
    {
        for(int i =0; i < dnaLenght; i++)
        {
            if(Random.Range(0,10) >= 5)
            {
                genes[i] = mother.genes[i];
            }
            else
            {
                genes[i] = father.genes[i];
            }
        }
    }

    // mutate random gene
    public void Mutate()
    {
        if (Random.Range(0, 11) == 5)
        {
            genes[Random.Range(0, dnaLenght / 2)] = Random.Range(minimumFlapAngle, maximumFlapAngle);
        }
        else
        {
            genes[Random.Range(dnaLenght / 2, dnaLenght)] = Random.Range(minimumFlapRate, maximumFlapRate);
        }
    }

    // get gene 
    public float GetGene(int genepos)
    {
        return genes[genepos];
    }
}
