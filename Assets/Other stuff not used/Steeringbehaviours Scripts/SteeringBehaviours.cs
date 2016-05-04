using System.Collections.Generic;
using UnityEngine;
//using Boo.Lang;

public class SteeringBehaviours : MonoBehaviour
{

    Rigidbody2D rb;
    Vector3 desiredVelocity, _target;
    public float maxSpeed, arriveRadius = 0.5f, slowradius = 3.0f, maxPrediction = 5f, rotationSpeed = 2f, wallAvoidDistance = 2f, distanceToAvoid = 2f, wanderRate, wanderRadius, wanderOffset, wanderOrientation, maxAcceleration;
    public LayerMask whatToAvoid;
    public const float TIMETOARRIVE = 0.25f, TIMETOARRIVEDYN = 0.1F;
    public GameObject targetObject;


    public enum AgentState { Seek, Flee, Arrive, Persue, Align, Face, Wander }
    public AgentState aiState;
    public GameObject showTarget;


    //  private float rotationSpeed;

    //  Vector3 target;


    //flocking
    List<GameObject> tagged = new List<GameObject>();
    public float tagRadius = 2f, seperationWeight, cohesionWeight, alignWeight, wanderWeight;
    public float maxforceMag = 10f;
    private bool flocking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // _target = targetObject.transform.position;
    }

    public Vector3 Seek(Vector3 seekTarget)
    {
        //showTarget.transform.position = seekTarget;
        desiredVelocity = seekTarget - transform.position;
        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed; //* Time.deltaTime;
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        // float zRot = Mathf.Atan2(desiredVelocity.y, desiredVelocity.x) * Mathf.Rad2Deg;
        transform.up = desiredVelocity;
        // Vector3 vel = new Vector3(rb.velocity.x, rb.velocity.y, 0);
        //    return desiredVelocity - vel;
        return desiredVelocity - (Vector3)rb.velocity;// 

        //  return (desiredVelocity - _target.Velocity);

    }



    public Vector3 Flee(Vector3 fleeTarget)
    {
        desiredVelocity = transform.position - _target;
        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed; //* Time.deltaTime;
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        // float zRot = Mathf.Atan2(desiredVelocity.y, desiredVelocity.x) * Mathf.Rad2Deg;
        transform.up = desiredVelocity;
        // Vector3 vel = new Vector3(rb.velocity.x, rb.velocity.y, 0);
        //    return desiredVelocity - vel;
        return desiredVelocity;
        //return (desiredVelocity - rb.velocity);
        //  return (desiredVelocity - _target.Velocity);

    }

    Vector3 Arrive(Vector3 arriveTarget)
    {
        Vector3 direction = arriveTarget - transform.position;
        float dist = direction.magnitude;
        if (dist < arriveRadius)
        {
            return Vector3.zero;
        }
        direction /= TIMETOARRIVE;
        if (direction.magnitude > maxSpeed)
        {
            direction.Normalize();
            direction *= maxSpeed;

        }
        return direction;
    }

    Vector3 DynamicArrive(Vector3 dynArriveTarget)
    {
        float targetSpeed;
        Vector3 direction = dynArriveTarget - transform.position;
        float dist = direction.magnitude;
        if (dist < arriveRadius)
        {
            return Vector3.zero;
            //return -rb.velocity;
        }

        if (dist > slowradius)
        {
            targetSpeed = maxSpeed;
        }
        else
        {
            targetSpeed = maxSpeed * dist / slowradius;
        }
        direction.Normalize();
        direction *= targetSpeed;

        direction = direction - (Vector3)rb.velocity;
        direction /= TIMETOARRIVEDYN;

        if (direction.magnitude > maxSpeed)
        {
            direction.Normalize();
            direction *= maxSpeed;
        }
        return direction;

    }

    //pursue similar to seek 

    public Vector3 Persue(GameObject targetObject)
    {
        float prediction;
        Vector3 direction = targetObject.transform.position - transform.position;
        float dist = direction.magnitude;
        float speed = rb.velocity.magnitude;
        if (speed <= dist / maxPrediction)
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = dist / speed;
        }
        Vector3 newTarget = targetObject.transform.position;
        Vector3 targetVel = new Vector3();
        Rigidbody2D targetRB = targetObject.GetComponent<Rigidbody2D>();
        if (targetRB != null)
        {
            targetVel = targetRB.velocity;
        }
        newTarget += targetVel * prediction;
        showTarget.transform.position = newTarget;
        return Seek(newTarget);

    }

    //public Vector3 Evade(GameObject targetObject)
    //{
    //    float prediction;
    //    Vector3 direction = targetObject.transform.position - transform.position;
    //    float dist = direction.magnitude;
    //    float speed = rb.velocity.magnitude;
    //    if (speed <= dist / maxPrediction)
    //    {
    //        prediction = maxPrediction;
    //    }
    //    else
    //    {
    //        prediction = dist / speed;
    //    }
    //    Vector3 newTarget = targetObject.transform.position;
    //    Vector3 targetVel = new Vector3();
    //    Rigidbody2D targetRB = targetObject.GetComponent<Rigidbody2D>();
    //    if (targetRB != null)
    //    {
    //        targetVel = targetRB.velocity;
    //    }
    //    newTarget += targetVel * prediction;
    //    showTarget.transform.position = newTarget;
    //    return DynamicArrive(newTarget);

    //}

    public void Face(Vector3 faceThis)
    {
        Vector3 dir = faceThis - transform.position;
        float rot_z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Align(Quaternion.Euler(0f, 0f, rot_z - 90));


    }

    public void Align(Quaternion target)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);

    }
    Vector3 Wander()
    {
        float jitterTimeSlice = wanderRate * Time.deltaTime;
        Vector3 wanderTarget = new Vector3();
        Vector3 ToAdd = UnityEngine.Random.insideUnitCircle * jitterTimeSlice;
        wanderTarget += ToAdd;
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 localTarget = wanderTarget + Vector3.up * wanderOffset;
        Vector3 worldTarget = transform.TransformPoint(localTarget);
        Face(worldTarget);
        return ((worldTarget - transform.position).normalized * maxSpeed);
    }

    Vector3 AltWander()
    {
        wanderOrientation += RandomBinomial() * wanderRate;
        float targetOrientation = wanderOrientation + transform.rotation.z;
        Vector3 wanderTarget = transform.position + wanderOffset * transform.up;

        wanderTarget += wanderRadius * (Quaternion.Euler(0, 0, targetOrientation) * transform.up);


        Face(wanderTarget);
        return maxAcceleration * transform.up;

    }

    float RandomBinomial()
    {
        return Random.value - Random.value;
    }

    Vector3 WallAvoid()
    {
        RaycastHit2D detector = Physics2D.Raycast(transform.position, rb.velocity.normalized * wallAvoidDistance, wallAvoidDistance, whatToAvoid);


        if (!detector)
        {
            return Vector3.zero;
        }

        Vector3 avoidTarget = detector.point + detector.normal * distanceToAvoid;
        return Seek(avoidTarget);
    }


    public Vector3 Seperation()
    {
        Vector3 steeringForce = Vector3.zero;
        //iterrate thru the tagged game objects
        for (int i = 0; i < tagged.Count; i++)
        {
            //store entity locally
            GameObject entity = tagged[i];

            if (entity != null)
            {
                //get Vector between objects
                Vector3 toEntity = transform.position - entity.transform.position;
                //adjust the force based on distance
                steeringForce += toEntity.normalized / toEntity.magnitude;
            }
        }
        return steeringForce;
    }

    public Vector3 Cohesion()
    {
        Vector3 steeringForce = Vector3.zero;
        Vector3 centreOfMass = Vector3.zero;   // shortcut vector(0,0,0)
        int taggedCount = 0;
        //iterate thru the entities
        foreach (GameObject entity in tagged)
        {
            // check if it is our boid, and ignore if it is
            if (entity != gameObject)
            {
                centreOfMass += entity.transform.position;
                // count how many boids we've checked
                taggedCount++;
            }
        }

        //if we have considered at least one boid
        if (taggedCount > 0)
        {
            centreOfMass /= taggedCount;
            // if we are already at the centre of mass, we use squmagnitude because it's let processing
            if (centreOfMass.sqrMagnitude == 0)
            {
                // just return nothing
                return Vector3.zero;
            }
            else
            {
                // if we arent at the centre Of Mass , seek towards it
                steeringForce = Vector3.Normalize(Seek(centreOfMass));

            }
        }
        return steeringForce;
    }

    public Vector3 Alignment()
    {
        //initialise steering force
        Vector3 steeringForce = Vector3.zero;
        //initialise to keep track of the tagged objects
        int taggedCount = 0;
        //iterate thru the tagged List
        foreach (GameObject entity in tagged)
        {
            // check if the current entity isnt this gameObject
            if (entity != gameObject)
            {
                //add the facing direction of the entity to the steering force (Vector3.up for 2D, Vector3.forward for 3D)
                steeringForce += entity.transform.up;
                //count the boid
                taggedCount++;

            }
        }
        //if we assessed at least one
        if (taggedCount > 0)
        {
            //average out the steering force
            steeringForce /= taggedCount;
            //adjust for the direction of the gameObject we're working on
            steeringForce -= transform.up;

        }
        return steeringForce;
    }
    /// <summary>
    /// Acumulates the force.
    /// </summary>
    /// <returns><c>true</c>, if force was acumulated,<c>false</c>otherwise,</returns>
    /// <param name="runningTotal">Running total, -will be updated by reference</param>
    /// <param name="force">the force to check</param>

    private bool AccumulateForce(ref Vector3 runningTotal, Vector3 force)
    {
        //how much force has been accumulated so far?
        float soFar = runningTotal.magnitude;

        //how much force 'budget' do we have left?
        float remaining = maxforceMag - soFar;

        //if we're out of allowed force, just exit
        if (remaining <= 0)
        {
            return false;
        }
        //calculate the magnitude of the force we want to add
        float toAdd = force.magnitude;

        // check if we have room to accomodate this force
        if (toAdd < remaining)
        {
            //we have enough remaining force budget left so just add teh force to the total
            runningTotal += force;
        }
        else
        {
            //if not were going to truncate the force, and then add it
            runningTotal += force.normalized * remaining;
        }
        return true;
    }

    private Vector3 CalculateWeightedPrioritized()
    {
        //populate the tagged List, within the tagRadius
        TagNeighboursSimple(tagRadius);
        // initialise the forces
        Vector3 force = Vector3.zero;
        Vector3 steeringForce = Vector3.zero;

        if (tagged.Count > 0)
        {
            //Seperation
            //adjust the Seperation force by the weight
            force = Seperation() * seperationWeight;
            //check if the result of seperation * weight has used up our force budget, and if it has - return the steering
            if (!AccumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }

            force = Alignment() * alignWeight;
            if (!AccumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }

            force = Cohesion() * cohesionWeight;
            if (!AccumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }

            force = Seek(_target) * wanderWeight;
            if (!AccumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }


        }
        return steeringForce;
    }



    private int TagNeighboursSimple(float inRange)
    {
        tagged.Clear();
        GameObject[] steerables = GameObject.FindGameObjectsWithTag("boid");
        foreach (GameObject steerable in steerables)
        {
            //check if it's this boid;
            if (steerable != gameObject)
            {
                // check if it's within target radius
                if ((transform.position - steerable.transform.position).magnitude < inRange)
                {
                    // if it is add it to the target list
                    tagged.Add(steerable);
                }
            }
        }
        return tagged.Count;
    }

    void Update()
    {
        if (rb.isKinematic)
        {
            switch (aiState)
            {
                case AgentState.Seek:
                    transform.position += Seek(_target) * Time.deltaTime;
                    // rb.AddForce(Seek(_target));
                    break;

                case AgentState.Flee:
                    transform.position += Flee(_target) * Time.deltaTime;
                    //rb.AddForce(Flee(_target));
                    break;

                case AgentState.Arrive:
                    transform.position += Arrive(_target) * Time.deltaTime;
                    //rb.AddForce(Flee(_target));
                    break;

                    // case AgentState.DynamicArrive:
                    //   transform.position += DynamicArrive(_target) * Time.deltaTime;
                    //rb.AddForce(Flee(_target));
                    // break;
            }
        }
        // transform.position += Flee();
        _target = targetObject.transform.position;
        // float step = maxSpeed * Time.deltaTime;
        //  transform.position = Vector3.MoveTowards(transform.position, Seek(),step);

    }








    void FixedUpdate()
    {
        if (!rb.isKinematic)
        {
            if (!flocking)
            {
                //    _target = targetObject.transform.position;
                switch (aiState)
                {
                    case AgentState.Seek:
                        rb.AddForce(Seek(_target));
                        break;

                    case AgentState.Flee:
                        rb.AddForce(Flee(_target));
                        break;

                    case AgentState.Arrive:
                        rb.AddForce(DynamicArrive(_target));
                        break;

                    //  case AgentState.DynamicArrive:
                    //   rb.AddForce(DynamicArrive(_target));
                    //  break;

                    case AgentState.Persue:
                        rb.AddForce(Persue(targetObject));
                        break;

                    case AgentState.Align:
                        Align(targetObject.transform.rotation);
                        break;

                    case AgentState.Face:
                        Face(targetObject.transform.position);
                        break;

                    case AgentState.Wander:
                        rb.AddForce(AltWander());
                        break;


                }
                rb.AddForce(WallAvoid());
            }
            else
            {
                rb.AddForce(CalculateWeightedPrioritized());

            }
            // rb.AddForce(Seek());
            //  _target = targetObject.transform.position;

        }
    }
}



//}

//}

//}
