using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingStone : MonoBehaviour
{

    public float delayTime = 1.0f;
    public string stoneOneName = "Green";
    public Color stoneOneColor = Color.green;

    public string stoneTwoName = "Red";
    public Color stoneTwoColor = Color.red;

	public int nofFlips = -1;

    void Start()
    {
        StartCoroutine(Flip(delayTime));
    }

    private IEnumerator Flip(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);


        if (transform.parent.name.Contains(stoneOneName))
        {
            transform.parent.name = stoneTwoName;
            GetComponent<SpriteRenderer>().color = stoneTwoColor;
        }

        else if (transform.parent.name.Contains(stoneTwoName))
        {
            transform.parent.name = stoneOneName;
            GetComponent<SpriteRenderer>().color = stoneOneColor;
        }

		nofFlips--;
		if (nofFlips != 0)
	        StartCoroutine(Flip(waitTime));
    }


    void Update()
    {
    }
}
