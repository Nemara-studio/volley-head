using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsController : NetworkBehaviour
{
    private PhysicsScene2D physicsScene;

    private void Start()
    {
        if (isServer)
        {
            physicsScene = gameObject.scene.GetPhysicsScene2D();
        }
    }

    private void FixedUpdate()
    {
        if (isServer)
        {
            if (physicsScene != null)
            {
                physicsScene.Simulate(Time.fixedDeltaTime * 1);
            }
        }
    }
}
