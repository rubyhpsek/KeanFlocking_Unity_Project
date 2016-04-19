using UnityEngine;

public class AgentConfig : MonoBehaviour
{

    public float RdC;  //  Radii for Cohesion
    public float RdA;  //  Radii for Alignment
    public float RdS;  //  Radii for Separation
    public float RdAvoid;   // Radii for Avoid Agent Enemies

    public float KfC;  //  Coefficient for Cohesion
    public float KfA;  //  Coefficient for Alignment
    public float KfS;  //  Coefficient for Separation
    public float KfW;  //  Coefficient for Wander
    public float KfAvoid;  // Coefficient for Avoid Agent Behaviour

    public float maxAccl; // Maximum Acceleration
    public float maxVel;  // Maximum Velocity


    public float MaximumFieldOfViewAngle = 180;

    // All parameter of Wander target
    public float WanderShake; // the point target would oscillates on circle
    public float WanderRadius; // the radius of the circle
    public float WanderDistance;   //Distance of center to the circle
}
