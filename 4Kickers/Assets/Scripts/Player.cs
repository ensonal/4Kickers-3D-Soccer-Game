using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI goalText;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    private Ball ballAttachedToPlayer;
    private float timeShot = -1f;
    public const int ANIMATION_LAYER_SHOOT = 1;
    private int myScore, otherScore = 0;
    private float goalTextAlpha;

    public Ball BallAttachedToPlayer { get => ballAttachedToPlayer; set => ballAttachedToPlayer = value; }

    void Start()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();

    }

    void Update()   
    {
        if(starterAssetsInputs.shoot)
        {
            starterAssetsInputs.shoot = false;
            timeShot = Time.time;
            animator.Play("Shoot", ANIMATION_LAYER_SHOOT, 0f);
            animator.SetLayerWeight(ANIMATION_LAYER_SHOOT, 1f);
        }
        if(timeShot > 0)
        {
            // shoot ball
            if(ballAttachedToPlayer != null && Time.time - timeShot > 0.2)
            {
                ballAttachedToPlayer.inThePlayer2 = false;

                Rigidbody rigidbody = ballAttachedToPlayer.transform.gameObject.GetComponent<Rigidbody>();
                Vector3 shootDirection = transform.forward;
                shootDirection.y += 0.3f;
                rigidbody.AddForce(shootDirection * 20f, ForceMode.Impulse);

                ballAttachedToPlayer = null;
            }

            // end of the shoot animation
            if(Time.time - timeShot > 0.5)
            {
                timeShot = -1f;
            }
        }
        else
        {
            animator.SetLayerWeight(ANIMATION_LAYER_SHOOT, Mathf.Lerp(animator.GetLayerWeight(ANIMATION_LAYER_SHOOT), 0f, Time.deltaTime * 10f));
        }

        if(goalTextAlpha > 0)
        {
            goalTextAlpha -= Time.deltaTime;
            goalText.alpha = goalTextAlpha;
            goalText.fontSize = 200 - (goalTextAlpha * 1 - 0);
        }
    }

    public void increaseMyScore()
    {
        myScore++;
        scoreText.text = "SCORE: " + myScore.ToString() + " - " + otherScore.ToString();
        goalTextAlpha = 1f;
    }

    public void increaseOtherScore()
    {
        otherScore++;
        scoreText.text = "SCORE: " + myScore.ToString() + " - " + otherScore.ToString();
        goalTextAlpha = 1f;

    }
}
