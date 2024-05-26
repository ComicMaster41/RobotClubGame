using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBehaviorManager : MonoBehaviour
{

    T GetScript<T>(string scriptName)
    {
        // The function returns script which can be used for its methods and other functions.

        switch (scriptName)
        {
            case "enemyInitializationScript":
                EnemyInitialization enemyInitializationScript = gameObject.GetComponent<EnemyInitialization>();
                return (T)Convert.ChangeType(enemyInitializationScript, typeof(T));

            case "roamingBehaviorScript":
                RoamingBehavior roamingBehaviorScript = gameObject.GetComponent<RoamingBehavior>();
                return (T)Convert.ChangeType(roamingBehaviorScript, typeof(T));

            case "guardingBehaviorScript":
                GuardingBehavior guardingBehaviorScript = gameObject.GetComponent<GuardingBehavior>();
                return (T)Convert.ChangeType(guardingBehaviorScript, typeof(T));

            case "suspicionsBehaviorScript":
                SuspicionsBehavior suspicionsBehaviorScript = gameObject.GetComponent<SuspicionsBehavior>();
                return (T)Convert.ChangeType(suspicionsBehaviorScript, typeof(T));

            case "chaseBehaviorScript":
                ChaseBehavior chaseBehaviorScript = gameObject.GetComponent<ChaseBehavior>();
                return (T)Convert.ChangeType(chaseBehaviorScript, typeof(T));

            case "lostTraceBehaviorScript":
                LostTraceBehavior lostTraceBehaviorScript = gameObject.GetComponent<LostTraceBehavior>();
                return (T)Convert.ChangeType(lostTraceBehaviorScript, typeof(T));

            default:
                return (T)Convert.ChangeType(0, typeof(T));
        }

    }

    bool IsTargetVisible(Camera camera, GameObject target)
    {
        // This function determines if the target is visiable inside the camera. In affermative case, it returns true. Othervise,
        // it returns false. The function requres the camera object which is used to determine if the object is
        // visiable and the target GameObject which we check.


        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

        if (GeometryUtility.TestPlanesAABB(planes, target.GetComponent<Collider>().bounds))
        {
            // If the target is within the camera's field of view

            Vector3 targetPosition = target.transform.position;
            RaycastHit objectHitInfo;

            if (Physics.Raycast(camera.transform.position, new Vector3(targetPosition.x, targetPosition.y + (float)Convert.ChangeType(1.5, typeof(float)), targetPosition.z) - camera.transform.position, out objectHitInfo))
            {
                // If the ray was able to hit the target

                if (objectHitInfo.transform.position - targetPosition == new Vector3(0, 0, 0))
                {
                    // If the target is not behind another object
                    return true;
                }
            }

        }

        return false;
    }

    void Update()
    {
        Debug.Log(IsTargetVisible(gameObject.transform.Find("Enemy Camera(Clone)").gameObject.GetComponent<Camera>(), GameObject.Find("Player")));
    }

}
