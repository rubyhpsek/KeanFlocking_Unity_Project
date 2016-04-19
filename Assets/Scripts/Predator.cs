using UnityEngine;
using System.Collections;

public class Predator: Agent {

    override protected Vector3 combineB()
    {

        return config.KfW * wanderB();
    }
}
