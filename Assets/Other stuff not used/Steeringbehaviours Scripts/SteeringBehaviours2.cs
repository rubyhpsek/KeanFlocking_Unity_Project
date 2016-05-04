using UnityEngine;
using System.Collections;

public class SteeringBehaviours2 : MonoBehaviour {

	Rigidbody2D rb;
	Vector3 desiredVelocity, _target;
	public float maxSpeed, arriveRadius=0.5f, slowradius = 3f, maxPrediction = 5f;
	public const float TIMETOARRIVE = 0.25f, TIMETOARRIVEDYN = 0.1F;
	public GameObject targetObject, showTarget;
	public enum AgentState {Seek, Flee, Arrive, Persue}
	public AgentState aiState;

	void Start(){
		rb = GetComponent<Rigidbody2D> ();

	}

	public Vector3 Seek(Vector3 seekTarget){
		desiredVelocity = seekTarget - transform.position;
		desiredVelocity.Normalize ();
		desiredVelocity *= maxSpeed ;
		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}
		transform.up = desiredVelocity;
		return desiredVelocity - (Vector3)rb.velocity; 
	}

	public Vector3 Flee(Vector3 fleeTarget){
		desiredVelocity = transform.position - fleeTarget;
		desiredVelocity.Normalize ();
		desiredVelocity *= maxSpeed ;
		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}
		transform.up = desiredVelocity;
		return desiredVelocity - (Vector3)rb.velocity;
	}

	Vector3 Arrive(Vector3 arriveTarget){
		Vector3 direction = arriveTarget - transform.position;
		float dist= direction.magnitude;
		if (dist < arriveRadius) {
			return Vector3.zero;
		}
		direction /= TIMETOARRIVE;
		if (direction.magnitude > maxSpeed) {
			direction.Normalize ();
			direction *= maxSpeed;
		}
		transform.up = direction;
		return direction;
	}

	Vector3 DynamicArrive(Vector3 dynArriveTarget){
		float targetSpeed;
		Vector3 direction = dynArriveTarget - transform.position;
		float dist = direction.magnitude;
		if (dist < arriveRadius) {
			return -rb.velocity;
		}
		if (dist > slowradius) {
			targetSpeed = maxSpeed;
		} else {
			targetSpeed = maxSpeed * dist / slowradius;
		}
		direction.Normalize ();
		direction *= targetSpeed;

		direction = direction - (Vector3)rb.velocity;
		direction /= TIMETOARRIVEDYN;

		if (direction.magnitude > maxSpeed) {
			direction.Normalize ();
			direction *= maxSpeed;
		}
		return direction;
	}

	public Vector3 Persue(GameObject targetObject){
		float prediction;
		Vector3 direction = targetObject.transform.position - transform.position;
		float dist = direction.magnitude;
		float speed = rb.velocity.magnitude;
		if (speed <= dist / maxPrediction) {
			prediction = maxPrediction;
		} else {
			prediction = dist / speed;
		}
		Vector3 newTarget = targetObject.transform.position;
		Vector3 targetVel = new Vector3();
		Rigidbody2D targetRB = targetObject.GetComponent<Rigidbody2D> ();
		if (targetRB != null) {
			targetVel = targetRB.velocity;
		}
		newTarget += targetVel * prediction;
		showTarget.transform.position = newTarget;
		return Seek (newTarget);
	}

	void Update(){
		if (rb.isKinematic) {
			switch (aiState) {
			case AgentState.Seek:
				transform.position += Seek (_target) * Time.deltaTime;
				break;
			case AgentState.Flee:
				transform.position += Flee (_target) * Time.deltaTime;
				break;
			case AgentState.Arrive:
				transform.position += Arrive (_target) * Time.deltaTime;
				break;
			}
		}
		_target = targetObject.transform.position;
	}
		

	void FixedUpdate(){
		if (!rb.isKinematic) {
			switch (aiState) {
			case AgentState.Seek:
				rb.AddForce (Seek (_target));
				break;
			case AgentState.Flee:
				rb.AddForce (Flee (_target));
				break;
			case AgentState.Arrive:
				rb.AddForce (DynamicArrive (_target));
				break;
			case AgentState.Persue:
				rb.AddForce (Persue (targetObject));
				break;
			}
		}
	}
}
