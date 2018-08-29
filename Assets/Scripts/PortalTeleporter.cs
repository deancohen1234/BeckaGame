using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PortalTeleporter : MonoBehaviour {

    public Transform player;
    public Transform reciever;


    private bool playerIsOverlapping = false;
	
	// Update is called once per frame
	void Update ()
    {
        /*if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            Debug.Log("Spinning");
            player.rotation = Quaternion.identity;

            player.GetComponent<FirstPersonController>().OverrideMouseLook(player, player.GetChild(0));
        }*/

        Vector3 portalToPlayer = player.position - transform.position;
        float dotProduct = Vector3.Dot(-transform.forward, portalToPlayer);

        float portalDot = Vector3.Dot(transform.forward, reciever.right); //-1 if when other portal is 90 away from portal


        //if this is true then player has moved across the portal

        if (playerIsOverlapping)
        {
            if (dotProduct < 0f)
            {
                float rotationDiff = -Quaternion.Angle(transform.rotation, reciever.rotation);
                Debug.Log("Rotation Dif" + rotationDiff);
                rotationDiff += 180f;

                if (portalDot < 0)
                {
                    rotationDiff = 360 - rotationDiff;
                }

                player.Rotate(Vector3.up, rotationDiff);

                player.GetComponent<FirstPersonController>().OverrideMouseLook(player, player.GetChild(0));


                Vector3 positionOffset = Quaternion.Euler(0, rotationDiff, 0) * portalToPlayer;
                player.position = reciever.position + positionOffset;

                playerIsOverlapping = false;
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        playerIsOverlapping = true;
    }

    private void OnTriggerExit(Collider other)
    {
        playerIsOverlapping = false;
    }
}
