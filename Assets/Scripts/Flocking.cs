using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour {
    // Booleans that allow user to toggle the four forces individually
    public bool flock_centering = true;
    public bool velocity_matching = true;
    public bool collision_avoidance = true;
    public bool wandering = true;

    // User-editable number of creatures
    public int NumCreatures = MinCreatues;
    private static int MinCreatues = 30;
    private static int MaxCreatues = 100;

    // Flocking Objects
    public GameObject bird;
    public GameObject bird1;
    public Material gold;
    public bool trail = true;
    
    // Creature information
    private GameObject[] creatures;
    private Vector3[] velocities;
    private Vector3[] forces;

    // Clamp the creature velocities to some reasonable range
    private static float min_v = -1f;
    private static float max_v = 1f;

    // Keep all the creatures confined to World Box (50 by 10 by 50)
    private static float min_x = -25f;
    private static float max_x = 25f ;
    private static float min_y = -5f;
    private static float max_y = 5f;
    private static float min_z = -25f;
    private static float max_z = 25f;

    // Random seed
    private int seed = 4803;

    // Start is called before the first frame update
    void Start() {
        Init();
    }

    // Initializes all variables needed for flocking simulation
    void Init() {
        Random.InitState(seed);

        creatures = new GameObject[MaxCreatues];
        float scale = 1f;

        for (int i = 0; i < MaxCreatues; i++) {
            float chance = Random.Range(0f, 1f);
            if (chance >= 0.5f) {
                creatures[i] = (GameObject) Instantiate(bird);
                scale = 0.2f;
            } else {
                creatures[i] = (GameObject) Instantiate(bird1);
                scale = 0.5f;
            }

            creatures[i].transform.position = new Vector3(Random.Range(min_x, 
                max_x), Random.Range(min_y, max_y), Random.Range(min_z, max_z));
            creatures[i].transform.localScale 
                = new Vector3(scale, scale, scale);

            var tr = creatures[i].AddComponent<TrailRenderer>();
            tr.startWidth = 0.1f;
            tr.endWidth = 0.05f;
            tr.time = 5f;
            tr.material = gold;
            tr.enabled = false;

            if (i >= NumCreatures) {
                creatures[i].SetActive(false);
            }
        }

        velocities = new Vector3[MaxCreatues];
        for (int i = 0; i < MaxCreatues; i++) {
            velocities[i] = new Vector3(Random.Range(min_v, max_v), 
                Random.Range(min_v, max_v), Random.Range(min_v, max_v));
        }

        forces = new Vector3[MaxCreatues];
        for (int i = 0; i < MaxCreatues; i++) {
            forces[i] = new Vector3(0f, 0f, 0f);
        }
    }

    // Leaves a gold line trail for a given boid
    void Leave_Trail(int i) {
        if (trail) {
            creatures[i].GetComponent<TrailRenderer>().enabled = true;
        } else {
            creatures[i].GetComponent<TrailRenderer>().enabled = false;
        }
    }

    // Creatures should never exceed high value and fall below low value
    void Limit_Creatures() {
        if (NumCreatures < MinCreatues) {
            NumCreatures = MinCreatues;
        } else if (NumCreatures > MaxCreatues) {
            NumCreatures = MaxCreatues;
        }
    }

    // When the user presses the space bar, 
    // cause the creatures to scatter to random locations in the World Box
    void Space_Press() {
        if (Input.GetKeyDown("space")) {
            for (int i = 0; i < NumCreatures; i++) {
                creatures[i].transform.position = new Vector3(Random.
                    Range(min_x, max_x), Random.Range(min_y, max_y), 
                    Random.Range(min_z, max_z));
            }
        }
    }

    // Finds distance between 2 boids
    float Distance(Vector3 p, Vector3 pj) {
        float px = p[0];
        float py = p[1];
        float pz = p[2];

        float pjx = pj[0];
        float pjy = pj[1];
        float pjz = pj[2];

        float d =  Mathf.Sqrt(Mathf.Pow(px - pjx, 2) + Mathf.Pow(py - pjy, 2) + 
            Mathf.Pow(pz - pjz, 2));

        return d;
    }

    // Calculates the weight of a boid based on its neighbours in given radius
    float Weight(Vector3 p, Vector3 pj, float dmax) {
        float E = 0.01f; // small value epsilon to valid division by 0
        float d = Distance(p, pj);

        // Using an inverse square weight or linear weight based on chance
        float w;
        float chance = Random.Range(0f, 1f);
        if (chance < 0.5) {
            w = 1f / (Mathf.Pow(d, 2) + E);
        } else {
            w = Mathf.Max(dmax - d, 0);
        }

        return w;
    }

    // Calculates the flock centering force of given boid
    Vector3 Flock_Centering_Force(int i) {
        Vector3 ffc = new Vector3(0f, 0f, 0f);

        if (!flock_centering) {
            return ffc;
        }

        float rfc = 5.5f;
        Vector3 p = creatures[i].transform.position;

        for (int j = 0; j < NumCreatures; j++) {
            Vector3 pj = creatures[j].transform.position;

            if (j != i && Distance(p, pj) <= rfc) {
                ffc += Weight(p, pj, rfc) * (pj - p) / Weight(p, pj, rfc);
            }
        }

        return ffc;
    }
    
    // Calculates the collision avoidance force of given boid
    Vector3 Collision_Avoidance_Force(int i) {
        Vector3 fca = new Vector3(0f, 0f, 0f);

        if (!collision_avoidance) {
            return fca;
        }

        float rca = 5f;
        Vector3 p = creatures[i].transform.position;

        for (int j = 0; j < NumCreatures; j++) {
            Vector3 pj = creatures[j].transform.position;
            if (j != i && Distance(p, pj) <= rca) {
                fca += Weight(p, pj, rca) * (p - pj);
            }
        }

        return fca;
    }

    // Calculates the velocity matching force of given boid
    Vector3 Velocity_Matching_Force(int i) {
        Vector3 fvm = new Vector3(0f, 0f, 0f);

        if (!velocity_matching) {
            return fvm;
        }

        float rvm = 5.4f;
        Vector3 p = creatures[i].transform.position;
        Vector3 v = velocities[i];

        for (int j = 0; j < NumCreatures; j++) {
            Vector3 pj = creatures[j].transform.position;
            Vector3 vj = velocities[j];
            
            if (j != i && Distance(p, pj) <= rvm) {
                fvm += Weight(p, pj, rvm) * (vj - v);
            }
        }

        return fvm;
    }

    // Calculates the flock centering force of given boid
    Vector3 Wandering_Force(int i) {
        Vector3 fw = new Vector3(0f, 0f, 0f);

        if (!wandering) {
            return fw;
        }

        fw += new Vector3(Random.Range(min_y, max_y), 
            Random.Range(min_y, max_y), Random.Range(min_y, max_y));

        return fw;
    }

    // Calculates sum of all forces and store with boid
    void Calculate_Forces(int i) {
        // Weights for each force are scalars of choice
        float wfc = 0.3f;
        float wvm = 0.3f;
        float wca = 0.25f;
        float ww = 0.15f;

        // fall
        forces[i] = wfc * Flock_Centering_Force(i) + 
            wvm * Velocity_Matching_Force(i) + 
            wca * Collision_Avoidance_Force(i) +
            ww * Wandering_Force(i);
    }

    // Updated positions of boids based on new velocity
    void Simulation() {
        for (int i = 0; i < MaxCreatues; i++) {
            if (i < NumCreatures) {
                creatures[i].SetActive(true);
                Calculate_Forces(i);
                Leave_Trail(i);
            } else {
                creatures[i].SetActive(false);
            }
        }

        for (int j = 0; j < NumCreatures; j++) {
            Vector3 v = velocities[j];
            velocities[j] += Time.deltaTime * forces[j];
            velocities[j] = Vector3.ClampMagnitude(velocities[j], max_v);
            creatures[j].transform.position += Time.deltaTime * 
                ((v + velocities[j]) / 2f);
            
            // Make sure creatues don't go out of bounds
            float x = creatures[j].transform.position[0];
            float y = creatures[j].transform.position[1];
            float z = creatures[j].transform.position[2];
            if (x <= min_x || x >= max_x) {
                velocities[j][0] *= -1;
            } 
            if (y <= min_y || y >= max_y) {
                velocities[j][1] *= -1;
            } 
            if (z <= min_x || z >= max_z) {
                velocities[j][2] *= -1;
            }
        }
    }
    
    // Update is called once per frame
    void Update() {
        Limit_Creatures();
        Simulation();
        Space_Press();
    }
}
