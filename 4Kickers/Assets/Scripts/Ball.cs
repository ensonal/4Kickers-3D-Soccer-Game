using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Transform player; 
    private Transform ballLocationWithPlayer;
    private bool inThePlayer; 
    float speed;
    Vector3 previousLocation;
    Player scriptPlayer;

    public bool inThePlayer2 { get => inThePlayer; set => inThePlayer = value; } 
    
    void Start()
    {
        ballLocationWithPlayer = player.Find("Geometry").Find("BallLocation");
        scriptPlayer = player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(inThePlayer2 == false)
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            if(distanceToPlayer < 0.5)
            {
                inThePlayer2 = true;
                scriptPlayer.BallAttachedToPlayer = this;
            }
        }
        else
        {
            Vector2 currentLocation = new Vector2(transform.position.x, transform.position.y);
            speed = Vector2.Distance(currentLocation, previousLocation) / Time.deltaTime;
            transform.position = ballLocationWithPlayer.position;
            transform.Rotate(new Vector3(player.right.x, 0, player.right.z), speed, Space.World);
            previousLocation = currentLocation;
        }
    }
}
