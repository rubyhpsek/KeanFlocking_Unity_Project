using System.Collections;
using UnityEngine;

public class FSM : MonoBehaviour
{

    //Rigidbody2D rb;
    //Vector3 desiredVelocity, _target;
    //public float maxSpeed, arriveRadius = 0.5f, slowradius = 3.0f, maxPrediction = 5f, rotationSpeed = 2f, wallAvoidDistance = 2f, distanceToAvoid = 2f, wanderRate, wanderRadius, wanderOffset, wanderOrientation, maxAcceleration;
    //public LayerMask whatToAvoid;
    //public const float TIMETOARRIVE = 0.25f, TIMETOARRIVEDYN = 0.1F;
    //public GameObject targetObject;

    //private bool flocking = false;

    public enum State
    {
        Initialize,
        Seeker,
        Persue,
        Flee,
    }

    public State aiState;     // show dropdown menu for ai State in Inspector
    public GameObject showTarget;   //  Display show target  in Inspector



    private State _state;  // local variable that represents our state
    private object seekTarget;




    // Use this for initialization
    IEnumerator Start()
    {

        //  rb = GetComponent<Rigidbody2D>();
        _state = State.Initialize;
        while (true)
        {
            switch (_state)
            {
                case State.Initialize:
                    Seek();
                    break;
                case State.Persue:
                    Persue();
                    break;
                case State.Flee:
                    Flee();
                    break;
            }

            yield return 0;
        }

    }

    //private void Seeker()
    //{
    //    //throw new NotImplementedException();
    //    Debug.Log("Seek function --Show it is working!");


    //}


    //private void Seek()
    //{
    //    Debug.Log("Seek function --Show it is working!");
    //    //  throw new NotImplementedException();
    //}


    //public void Seeker()
    //{

    //    Debug.Log("Seek function --Show it is working!");

    //    ////  desiredVelocity = seekTarget - transform.position;
    //    //desiredVelocity.Normalize();
    //    //desiredVelocity *= maxSpeed;
    //    //if (rb.velocity.magnitude > maxSpeed)
    //    //{
    //    //    rb.velocity = rb.velocity.normalized * maxSpeed;
    //    //}

    //    //transform.up = desiredVelocity;

    //    //return;

    //}





    private void Init()
    {
        Debug.Log("Init function It is Working now!");
        _state = State.Seeker;
    }
    private void Seek()
    {
        Debug.Log("Seek function It is Working now!");
        _state = State.Seeker;
    }


    private void Persue()
    {
        Debug.Log("Persue It is Working now!");
        _state = State.Persue;
    }


    private void Flee()
    {
        Debug.Log("Start Fleeing from target");
        _state = State.Flee;
    }

    // Update is called once per frame
    void Update()
    {
        //if (rb.isKinematic)
        //{
        //    switch (aiState)
        //    {
        //        case State.Seeker:
        //            transform.position += Seeker(_target) * Time.deltaTime;
        // rb.AddForce(Seek(_target));
        //break;

        //case AgentState.Flee:
        //    transform.position += Flee(_target) * Time.deltaTime;
        //    //rb.AddForce(Flee(_target));
        //    break;

        //case AgentState.Arrive:
        //    transform.position += Arrive(_target) * Time.deltaTime;
        //    //rb.AddForce(Flee(_target));
        //    break;

        //    // case AgentState.DynamicArrive:
        //    //   transform.position += DynamicArrive(_target) * Time.deltaTime;
        //rb.AddForce(Flee(_target));
        // break;
    }
}
// transform.position += Flee();
//_target = targetObject.transform.position;
// float step = maxSpeed * Time.deltaTime;
//  transform.position = Vector3.MoveTowards(transform.position, Seek(),step);





//void FixedUpdate()
//{
//    if (!rb.isKinematic)
//    {
//        if (!flocking)
//        {
//            //    _target = targetObject.transform.position;
//            switch (aiState)
//            {
//                case State.Seeker:
//                    rb.AddForce(Seeker(_target));
//                    break;

//                    //case State.Flee:
//                    //    rb.AddForce(Flee(_target));
//                    //    break;

//                    //case AgentState.Arrive:
//                    //    rb.AddForce(DynamicArrive(_target));
//                    //    break;

//                    ////  case AgentState.DynamicArrive:
//                    ////   rb.AddForce(DynamicArrive(_target));
//                    ////  break;

//                    //case AgentState.Persue:
//                    //    rb.AddForce(Persue(targetObject));
//                    //    break;

//                    //case AgentState.Align:
//                    //    Align(targetObject.transform.rotation);
//                    //    break;

//                    //case AgentState.Face:
//                    //    Face(targetObject.transform.position);
//                    //    break;

//                    //case AgentState.Wander:
//                    //    rb.AddForce(AltWander());
//                    //    break;


//            }
//            //rb.AddForce(WallAvoid());
//        }
//        else
//        {
//            // rb.AddForce(CalculateWeightedPrioritized());

//        }
//        //    rb.AddForce(Seeker());
//        _target = targetObject.transform.position;

//    }
//}
//}
