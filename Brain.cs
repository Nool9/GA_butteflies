using UnityEngine;

// acts on instructions from dna (wing movement), keeps track of fitness 

public class Brain : MonoBehaviour
{
    public GameObject leftWing;
    public GameObject rightWing;
    public DNA dna;
    public int dnaLenght;
    public bool drawRays;

    public float minimumFlapAngle;
    public float maximumFlapAngle;
    public float minimumFlapRate;
    public float maximumFlapRate;

    public float timeAlive;
    public float distanceTraveled;
    public float groundDetectionDistance;

    private float flapAngle;
    private float flapRate;

    private bool alive;
    
    private Vector3 spawnPosistion;

    private float flapForce;
    private float time;

    private int selfMask;

    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spawnPosistion = transform.position;
    }

    // create DNA, set inital flap angle and rate from genes
    public void Init()
    {
        alive = true;
        
        dna = new DNA(dnaLenght, minimumFlapAngle, maximumFlapAngle, minimumFlapRate, maximumFlapRate);

        flapAngle = dna.GetGene(0);
        flapRate = dna.GetGene(3);
    }

    // update fitness values
    private void Update()
    {
        if (alive)
        {
            timeAlive = PopulationManager.elapsed;
            distanceTraveled = Vector3.Distance(transform.position, spawnPosistion);
        }
    }

    private void FixedUpdate()
    {
        if (alive)
        {
            FlapWings();
            CollisionDetection();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        alive = false;
    }

    // check for floor/ ceiling, if detected adjust flap angle and flap rate from genes
    private void CollisionDetection()
    {
        // exclude self from raycast hits
        int selfMask = 1 << 8;
        selfMask = ~selfMask;
        RaycastHit hit;

        // debug ray
        if (drawRays) Debug.DrawRay(transform.position, -transform.forward * groundDetectionDistance, Color.red);

        if (Physics.Raycast(transform.position, -transform.forward, out hit, groundDetectionDistance, selfMask))
        {
            if (!hit.collider.gameObject.CompareTag("Butterfly"))
            {
                flapAngle = dna.GetGene(1);
                flapRate = dna.GetGene(4);
            }
        }

        // debug ray
        if (drawRays) Debug.DrawRay(transform.position, transform.forward * groundDetectionDistance, Color.blue);

        if (Physics.Raycast(transform.position, transform.forward, out hit, groundDetectionDistance, selfMask))
        {
            if (!hit.collider.gameObject.CompareTag("Butterfly"))
            {
                flapAngle = dna.GetGene(2);
                flapRate = dna.GetGene(5);
            }
        }
    }

    // fly, add lift 
    private void FlapWings()
    {   
        time += Time.deltaTime;
        float phase = Mathf.Sin(time / flapRate);
        leftWing.transform.localRotation = Quaternion.Euler(new Vector3(0, phase * flapAngle, 0));
        rightWing.transform.localRotation = Quaternion.Euler(new Vector3(0, (phase * flapAngle) * -1, 0));

        if (phase >= 0.5f)
        {
            CalculateFlapForce();
            rb.AddForce(transform.forward * flapForce,ForceMode.Force);
        }
    }

    // calculate lift 
    private void CalculateFlapForce()
    {
        flapForce = (flapAngle / flapRate) / 3666;
    }
}
