using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour {

    public Transform playerCamera;
    public Transform portal;
    public Transform otherPortal;

	// Update is called once per frame
	void Update () {

        //0 - 90 is good 90 - 270 not good 
        //dot between right vector of portala and of portalb

        float dot = Vector3.Dot(otherPortal.up, portal.right); //-1 if when other portal is 90 away from portal

        Vector3 playerOffsetFromPortal = playerCamera.position - otherPortal.position;

        float angularDifferenceBetweenPortalRotations = Quaternion.Angle(portal.rotation, otherPortal.rotation);
        angularDifferenceBetweenPortalRotations += 180f; //to flip rotations so camera doesn't mirror player, but inverse mirrors them

        if (dot < 0)
        {
            angularDifferenceBetweenPortalRotations = 360 - angularDifferenceBetweenPortalRotations;
        }

        Quaternion angleAxis = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);

        Vector3 newCameraPosition = portal.position + (angleAxis * playerOffsetFromPortal); //rotate camera needed based on difference between 2 portals rotations

        transform.position = new Vector3(newCameraPosition.x, newCameraPosition.y, newCameraPosition.z);

        Quaternion portalRotationDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        Vector3 newCameraDirection = portalRotationDifference * playerCamera.forward;
        transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
	}
}
