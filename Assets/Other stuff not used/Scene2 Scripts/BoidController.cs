using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// these define the flock's behavior
/// </summary>
public class BoidController : MonoBehaviour
{
    public float minVelocity = 5;
    public float maxVelocity = 20;
    public float randomness = 1;
    public int flockSize = 200;
    public BoidFlocking prefab;
    public int numOfAgents;
    public Transform target;
    public Transform BirdFlockingprefab;
    public float spawnr;

    internal Vector3 flockCenter;
    internal Vector3 flockVelocity;

    List<BoidFlocking> boids = new List<BoidFlocking>();

    void Start()

    {
        spawn(BirdFlockingprefab, numOfAgents);
        for (int i = 0; i < flockSize; i++)
        {
            BoidFlocking boid = Instantiate(prefab, transform.position, transform.rotation) as BoidFlocking;
            boid.transform.parent = transform;
            boid.transform.localPosition = new Vector3(
                            Random.value * GetComponent<Collider>().bounds.size.x,
                            Random.value * GetComponent<Collider>().bounds.size.y,
                            Random.value * GetComponent<Collider>().bounds.size.z) - GetComponent<Collider>().bounds.extents;
            boid.controller = this;
            boids.Add(boid);
        }
    }

    void Update()
    {
        Vector3 center = Vector3.zero;
        Vector3 velocity = Vector3.zero;
        foreach (BoidFlocking boid in boids)
        {
            center += boid.transform.localPosition;
            velocity += boid.GetComponent<Rigidbody>().velocity;
        }
        flockCenter = center / flockSize;
        flockVelocity = velocity / flockSize;
    }

    void spawn(Transform prefab, int ns)
    {
        for (int i = 0; i < ns; i++)
        {
            var obj = Instantiate(prefab,
                                  new Vector3(Random.Range(-spawnr, spawnr), 0, Random.Range(-spawnr, spawnr)),
                                  Quaternion.identity);



        }
    }
}