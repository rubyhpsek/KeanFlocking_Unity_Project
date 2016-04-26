using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public Transform agentPrefab;

    public int numOfAgents;

    public List<Agent> agents;      //  Keep track of all the agents

    public float boundary;       // boundaries for all the combine movements of all agents

    public float spawnr;      //  radius of spawn

    public List<Predator> predators;   // Keep track of  all predators
    public List<Seeker> seekers;


    // Use this for initialization
    void Start()
    {
        agents = new List<Agent>();
        spawn(agentPrefab, numOfAgents);

        agents.AddRange(FindObjectsOfType<Agent>());
        predators.AddRange(FindObjectsOfType<Predator>());
        seekers.AddRange(FindObjectsOfType<Seeker>());
    }

    // Update is called once per frame
    void Update()
    {
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

    public List<Agent> getNeighbours(Agent agent, float radius)
    {
        // return null;

        List<Agent> r = new List<Agent>();

        foreach (var otherAgent in agents)   // Iterate all agents
        {

            if (otherAgent == agent)     // If no one is a neighbour of itself
                continue;

            if (Vector3.Distance(agent.x, otherAgent.x) <= radius)  // if the distance of agent found is less or equal to the radius of the "Neighbour-Radius"
            {
                r.Add(otherAgent);      // if fit the requirement as previous line , then add the agent as neighbour's radius
            }
        }

        return r;          // We return the list
    }//  We list getNeighbours()   function


    public List<Predator> getPredators(Agent agent, float radius)
    {

        List<Predator> r = new List<Predator>();

        foreach (var predator in predators)
        {

            if (Vector3.Distance(agent.x, predator.x) <= radius)
            {
                r.Add(predator);
            }
        }

        return r;
    }
    public List<Seeker> getSeekers(Agent agent, float radius)
    {

        List<Seeker> r = new List<Seeker>();

        foreach (var seeker in seekers)
        {

            if (Vector3.Distance(agent.x, seeker.x) <= radius)
            {
                r.Add(seeker);
            }
        }

        return r;
    }


}

