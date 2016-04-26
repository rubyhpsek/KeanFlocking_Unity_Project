using UnityEngine;


public class Agent : MonoBehaviour
{



    public Vector3 x;   // Create Vector3 for the position
    public Vector3 vel;   // Create Vector3 for the velocity
    public Vector3 accl;   // Create Vector3 for the acceleration
    public World world; // Reference to the World
    public AgentConfig config; //Reference to AgentConfig


    public GameObject targetObject;


    // Use this for initialization
    void Start()
    {
        world = FindObjectOfType<World>();

        config = FindObjectOfType<AgentConfig>();

        x = transform.position;

        vel = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));   // Add an initial Velocity to the agent to show that the velocity is changing;Randomly generate position in x and z plane no y plane velocity


        //*** debugWanderCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

    }  // Start function called once and before the update function

    // Update is called once per frame
    void Update()
    {

        // if state is flock, then flock or else flee
        float tm = Time.deltaTime;

        accl = combineB();   //combineB()+ cohesionB() + separationB();
        accl = Vector3.ClampMagnitude(accl, config.maxAccl);
        vel = vel + accl * tm;  // compute new velocity 
        vel = Vector3.ClampMagnitude(vel, config.maxVel);

        x = x + vel * tm;   // compute new position

        coverAround(ref x, -world.boundary, world.boundary);

        transform.position = x; // Modify the transform position to move the object


        if (vel.magnitude > 0)
            transform.LookAt(x + vel);           // To Rotate

    }

    Vector3 cohesionB()
    {
        // cohesion behavior:  creating Vectors of Radius(r) used in Cohesion()

        Vector3 r = new Vector3();
        int countinAgents = 0;  // Create a local variable

        // get or identify all my nearby neighbors inside radius RdC of this current agent from the World Class
        var neighbours = world.getNeighbours(this, config.RdC);   // pass in parameters of this and coefficient of Radius in AgentConfig class

        // no neighbors means no cohesion desire
        if (neighbours.Count == 0)                   // check if there is neighbours in vector
            return r;                             // if no neighbours identified return as 0 Vector

        // find the center of mass of all neighbours
        foreach (var agent in neighbours)    // use Iteration to find out the COM(center of mass) of neighbours
        {
            if (withinFieldOfView(agent.x))
            {
                r += agent.x;           // add position of agents
                countinAgents++;
            }
        }

        if (countinAgents == 0)
            return r;

        r /= countinAgents;         // divide the number of agents identified 

        // a vector from our position x towards the COM r
        r = r - this.x;

        r = Vector3.Normalize(r);   // Give a result of vector with magnitude of 1

        return r;
    }

    Vector3 separationB()
    {
        // separation behavior
        // steer in the oposite direction from each of our nearby neighbours

        Vector3 r = new Vector3();

        // get all neighbors and passing parameter of this agent and Radius of Separation
        var agents = world.getNeighbours(this, config.RdS);

        // no neighbors no separation is needed; check if there is any neighbours
        if (agents.Count == 0)
            return r;    // if no neighbours return as 0 vector.

        // add the contribution of each neighbor towards me
        foreach (var agent in agents)   // Iterate each of our neighbours
        {
            if (withinFieldOfView(agent.x))
            {
                Vector3 towardsMe = this.x - agent.x;   // Compute between arrow to ourselves by subtracting the my position with the distance from the neighbour

                // force contribution will vary inversly proportional to distance
                if (towardsMe.magnitude > 0)    // Check if Vector is 0
                {
                    r += towardsMe.normalized / towardsMe.magnitude;  // Add to the resulting radius which is the scaled distance; will not make it proportionate to the square of the distance , just the regular distance
                }
            }
        }

        return r.normalized;     // return the normalizex Vector
    }

    Vector3 alignmentB()
    {
        // alignment behavior
        // steer agent to match the direction and speed of neighboring agents

        Vector3 r = new Vector3();

        // get all neighbours form world space by passing this agent and the radius of Alignment from AgentConfig
        var agents = world.getNeighbours(this, config.RdA);

        // no neighbors means we do not have to align to anything
        if (agents.Count == 0)
            return r;

        // match direction and speed(which is the velocity) == match velocity
        // do this for all neighbours agents
        foreach (var agent in agents)   // Iterate all our neighboring agents
            if (withinFieldOfView(agent.x))
                r += agent.vel;   // Add their velocity to the radius of neighbour's radius

        return r.normalized;
    }

    virtual protected Vector3 combineB()
    {
        // combine all desires
        // weighted sum
        Vector3 r =
            config.KfAvoid * avoidAgentEnemies()
           + config.KfSeek * seekAgentEnemies()
                        + config.KfC * cohesionB()
                        + config.KfS * separationB()
                        + config.KfA * alignmentB()
                        + config.KfW * wanderB();
        // end results of all will be stored in r
        return r;
    }

    void coverAround(ref Vector3 vel, float min, float max)   // Keep all agents on screen:  cover or wrap around the edges of all agents inside the "neighbour radius"
    {
        vel.x = coverAroundFloat(vel.x, min, max);
        vel.y = coverAroundFloat(vel.y, min, max);
        vel.z = coverAroundFloat(vel.z, min, max);
    }

    float coverAroundFloat(float value, float min, float max)
    {

        //        min ------value-------- max
        if (value > max)
            value = min;
        else if (value < min)
            value = max;
        return value;
    }

    bool withinFieldOfView(Vector3 objectInView)      // check the position of the object will be taken as parameter
    {
        return Vector3.Angle(this.vel, objectInView - this.x) <= config.MaximumFieldOfViewAngle;   // this position, and relative position of the objectinview that we are interested in the scene towards our position; check if this angle is smaller than the maximumviewofangle that is defined in the agentconfig 
    }

    Vector3 wanderTrgt;      // wanderTarget position THE position we will follow
                             //***  GameObject debugWanderCube;  // visual cube at the wanderTarget 

    protected Vector3 wanderB()
    {
        // wander steer behavior; shake would be time independent

        float shake = config.WanderShake * Time.deltaTime;

        // add a small random vector to the target's position generate by the Binomial
        wanderTrgt += new Vector3(RandomBinomial() * shake, 0, RandomBinomial() * shake);

        // reproject the vector back to unit circle by nomalizing the wanderTrgt
        wanderTrgt = wanderTrgt.normalized;

        // increase length to be the same as the radius of the wander circle
        wanderTrgt *= config.WanderRadius;

        // position the target in front of the agent
        Vector3 targetInLocalArea = wanderTrgt + new Vector3(0, 0, config.WanderDistance);

        // project the target from local space to world space
        Vector3 targetInWorldArea = transform.TransformPoint(targetInLocalArea);

        // ***  debugWanderCube.transform.position = targetInWorldArea;// Just to test the Wander Behavior :    debugWanderCube.transform.position = targetInWorldArea;

        // steer towards it
        targetInWorldArea -= this.x;

        return targetInWorldArea.normalized;
    }

    float RandomBinomial()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }

    Vector3 avoidAgentEnemies()
    {
        // flee from all enemies behavior
        Vector3 r = new Vector3();

        var enemies = world.getPredators(this, config.RdAvoid);

        if (enemies.Count == 0)
            return r;

        foreach (var enemy in enemies)
        {
            r += flee(enemy.x);   // enemy inherited from agent , does have an x position
        }
        return r.normalized;
    }
    Vector3 seekAgentEnemies()
    {
        // flee from all enemies behavior
        Vector3 r = new Vector3();

        var enemies = world.getSeekers(this, config.RdSeek);

        if (enemies.Count == 0)
            return r;

        foreach (var enemy in enemies)
        {
            r += Seek(enemy.x);   // enemy inherited from agent , does have an x position
        }
        return r.normalized;
    }


    Vector3 flee(Vector3 target)
    {
        // run the opposite direction from the target
        Vector3 desiredVel = (x - target).normalized * config.maxVel;

        // steer our velocity
        return desiredVel - vel;
    }

    Vector3 Seek(Vector3 seekTarget)
    {
        Vector3 desiredVel = seekTarget - transform.position;
        desiredVel.Normalize();
        desiredVel *= config.maxVel; //* Time.deltaTime;
        if (x.magnitude > config.maxVel)
        {
            x = x.normalized * config.maxVel;
        }
        transform.up = desiredVel;
        return desiredVel - vel;


    }

}
