using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDetect : MonoBehaviour
{
    [SerializeField] private Player scriptPlayer;
    [SerializeField] private Ball ball;
    
    Vector3 goalLocation1 = new Vector3(0, 0.1143377f, 18.455f);
    Vector3 goalLocation2 = new Vector3(0, 0.1143377f, -18.455f);



    void Start()
    {
        
    }

  
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Ball"))
        {
            if (name.Equals("GoalDetector"))
            {
                scriptPlayer.increaseMyScore();
                Debug.Log("GoalDetect1");
                ball.gameObject.transform.position = goalLocation1;
                ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            }
            else
            {
                scriptPlayer.increaseOtherScore();
                Debug.Log("GoalDetect2");
                ball.gameObject.transform.position = goalLocation2;
                ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }

        }
        
    }
}
