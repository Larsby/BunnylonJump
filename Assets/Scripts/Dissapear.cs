using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissapear : MonoBehaviour
{

    int myPlace = 0;
    Animator playerAnimator = null;
    // Use this for initialization
    void Start()
    {
        playerAnimator = gameObject.GetComponent<Animator>();

        if (playerAnimator)
        {
            playerAnimator.Play("Appear");
        }
        else
        {
            gameObject.transform.localScale = new Vector3(0, 0, 0);

            iTween.ScaleTo(gameObject, new Vector3(1.0f, 1.0f, 1.0f), 0.5f);

        }



    }

    public void setMyPlace(int inPlace)
    {
        myPlace = inPlace;


    }
    public void startDissapear(float dissapeartime)
    {
        StartCoroutine(theEnd(dissapeartime + 0.3f));
        StartCoroutine(scaleaway(dissapeartime));
        //gameObject.transform.localScale = new Vector3(1f, 1f, 1f);




    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator scaleaway(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (playerAnimator)
        {
            playerAnimator.Play("DissAppear");

        }
        else
        {
            iTween.ScaleTo(gameObject, new Vector3(0, 0, 0), 0.3f);

        }


    }

    private IEnumerator theEnd(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GameObject go = GameObject.Find("Brains");
        go.SendMessage("dissapear", myPlace, SendMessageOptions.RequireReceiver);

    }
}