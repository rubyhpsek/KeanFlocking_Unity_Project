using UnityEngine;

public class Seeker : Agent
{

    override protected Vector3 combineB()
    {

        return config.KfW * wanderB();
    }
}
