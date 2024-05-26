using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyInitialization : MonoBehaviour
{

    // The script is expected to be attached to the enemy object. It requires some values which will discribe enemy's behavior.

    public int enemyFieldOfView = 30;       // the field of view of the enemy's camera, or the field of view of the enemy itself
    public int enemyViewDistance = 20;      // the view distance of the enemy's camera, or the view distance of the enemy itself
    public bool isRoamingBahvior = false;   // if false, the enemy would have the Guarding Behavior. If true, the enemy would have the Roaming Behavior

    // Initialization of the enemy's objects and their values.
    void Start()
    {
        // Getting the camera object form the Enemy Constructor.
        GameObject enemyConstructor = GameObject.Find("Enemy Constructor");
        GameObject enemyCameraObject = enemyConstructor.transform.Find("Enemy Camera").gameObject;
        Camera enemyCamera = enemyCameraObject.GetComponent<Camera>();

        var instantiatedEnemyCamera = GameObject.Instantiate(enemyCamera, transform.position, transform.rotation); // Copying enemyCamera object
        instantiatedEnemyCamera.transform.parent = gameObject.transform; // Attaching the copied camera to the enemy on which this script
        instantiatedEnemyCamera.fieldOfView = enemyFieldOfView;          // Changing enemy's field of view
        instantiatedEnemyCamera.farClipPlane = enemyViewDistance;        // Changing enemy's view distance
        

        // Adding all the scripts on the enemy
        gameObject.AddComponent<EnemyBehaviorManager>();
        gameObject.AddComponent<RoamingBehavior>();
        gameObject.AddComponent<GuardingBehavior>();
        gameObject.AddComponent<SuspicionsBehavior>();
        gameObject.AddComponent<ChaseBehavior>();
        gameObject.AddComponent<LostTraceBehavior>();
        gameObject.AddComponent<HealthManager>();
        gameObject.AddComponent<AttackManager>();
        gameObject.AddComponent<VisionState>();
    }

    // Add the check for enemy being too far away from the player.
    void Update()
    {
        
    }
}
