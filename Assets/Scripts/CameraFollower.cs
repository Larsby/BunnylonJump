using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{

	public GameObject player;       //Public variable to store a reference to the player game object

    private Vector3 offset;         //Private variable to store the offset distance between the player and camera
    private Vector3 StartOffset;         //Private variable to store the offset distance between the player and camera

    private float drift = 0;
    private float driftModfier = 0.005f;

	[HideInInspector]
	public bool rolling = false;

    // Use this for initialization
    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
        StartOffset = offset;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
		if (!rolling) return;

        offset.y = StartOffset.y + drift;

        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        transform.position = player.transform.position + offset;
        drift += driftModfier;
    }

    public void resetDrift()
    {
        offset = StartOffset;

        drift = 0;
    }



    public void changeTheDriftVariable(float inDrift)
    {
        driftModfier = inDrift;
        StopCoroutine(changeDirectionBack(0));

    }

    public void changeDirection()
    {
        driftModfier = -0.007f;
        StartCoroutine(changeDirectionBack(0.15f));

    }

    private IEnumerator changeDirectionBack(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (driftModfier == -0.007f)
            driftModfier = 0.005f;

    }


}