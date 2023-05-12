using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OutOfFieldEvent_
{
    ThrowIn,
    GoalKick,
    Corner
}

public class Ball : MonoBehaviour
{
    private float ballOutOfFieldTimeOut;
    private float delayCheckOutOfField;
    private OutOfFieldEvent_ outOfFieldEvent;
    private Vector3 speed;
    private Vector3 ballOutOfFieldposition;
    private Vector3 previousPosition;
    private new Rigidbody rigidbody;
    private AudioSource soundWhistle;
    private const float BALL_GROUND_POSITION_Y = 0.72f;
    private const float PASSING_SPEED = 25f;
    private bool isOutOfField;

    public float BallOutOfFieldTimeOut { get => ballOutOfFieldTimeOut; set => ballOutOfFieldTimeOut = value; }
    public Rigidbody Rigidbody { get => rigidbody; set => rigidbody = value; }
    public Vector3 Speed { get => speed; set => speed = value; }
    public float DelayCheckOutOfField { get => delayCheckOutOfField; set => delayCheckOutOfField = value; }
    public bool IsOutOfField { get => isOutOfField; set => isOutOfField = value; }

    private float timePassedBall;
    private CinemachineVirtualCamera playerFollowCamera;

    private void Awake()
    {
        playerFollowCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();

        soundWhistle = GameObject.Find("Sound/whistle").GetComponent<AudioSource>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public void PutOnGround()
    {
        transform.position = new Vector3(transform.position.x, BALL_GROUND_POSITION_Y, transform.position.z);
    }
    public void PutOnCenterSpot()
    {
        transform.position = new Vector3(-0.1f, BALL_GROUND_POSITION_Y, -0.049f);
    }

    private void TakeThrowInOrGoalKick()
    {
        if (Game.Instance.PlayerWithBall != null)
        {
            Game.Instance.PlayerWithBall.HasBall = false;
            Debug.Log("loose ball :" + Game.Instance.PlayerWithBall.name);
            Game.Instance.SetPlayerWithBall(null);
        }
        transform.position = ballOutOfFieldposition;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        Player player;

        if (outOfFieldEvent == OutOfFieldEvent_.ThrowIn)
        {
            player = Game.Instance.GetPlayerToThrowIn();
            player.Animator.Play("ThrowIn", PlayerAnimation.LAYER_THROW_IN, 0.4f);
            player.Animator.SetLayerWeight(PlayerAnimation.LAYER_THROW_IN, 1);
            player.DoingThrow = true;
            rigidbody.isKinematic = true;
            transform.position = player.BallHandPosition.position;
        }
        else if (outOfFieldEvent == OutOfFieldEvent_.GoalKick)
        {
            player = Game.Instance.GetPlayerToGoalKick();
            player.DoingKick = true;
            transform.position = player.PlayerBallPosition.position;
        }
        else
        {
            // corner
            player = Game.Instance.GetPlayerToThrowIn();
            player.Team.Stats.Corners++;
            player.DoingKick = true;
            transform.position = player.PlayerBallPosition.position;
        }
        transform.parent = player.transform;
        player.SetPosition(new Vector3(ballOutOfFieldposition.x, player.transform.position.y, ballOutOfFieldposition.z));
        // Look in the directon of the field.
        player.transform.LookAt(Game.Instance.KickOffPosition);
        player.TimePlayerActionRequested = Time.time;
        Game.Instance.SetPlayerWithBall(player);
        Game.Instance.NextGameState = GameState_.BringingBallIn;
        Game.Instance.SetGameState(GameState_.WaitingForWhistle);

        // move players that are too close
        Game.Instance.SetMinimumDistanceOtherPlayers(player);
    }

    private void CheckBallOutOfField()
    {
        // ball out of field
        if (transform.position.z < Game.FIELD_BOUNDARY_LOWER_Z)
        {
            isOutOfField = true;
            outOfFieldEvent = OutOfFieldEvent_.ThrowIn;
            ballOutOfFieldTimeOut = 1.0f;
            ballOutOfFieldposition = new Vector3(transform.position.x, BALL_GROUND_POSITION_Y, -25);
        }
        if (transform.position.z > Game.FIELD_BOUNDARY_UPPER_Z)
        {
            isOutOfField = true;
            outOfFieldEvent = OutOfFieldEvent_.ThrowIn;
            ballOutOfFieldTimeOut = 1.0f;
            ballOutOfFieldposition = new Vector3(transform.position.x, BALL_GROUND_POSITION_Y, 25);
        }

        if (transform.position.x < Game.FIELD_BOUNDARY_LOWER_X)
        {
            isOutOfField = true;
            ballOutOfFieldTimeOut = 1.0f;
            if (Game.Instance.TeamLastTouched.Number == 0)
            {
                outOfFieldEvent = OutOfFieldEvent_.GoalKick;
                ballOutOfFieldposition = new Vector3(-46.8f, BALL_GROUND_POSITION_Y, -4.89f);
            }
            else
            {
                outOfFieldEvent = OutOfFieldEvent_.Corner;
                if (transform.position.z > 0)
                {
                    ballOutOfFieldposition = new Vector3(-52.6f, BALL_GROUND_POSITION_Y, 25);
                }
                else
                {
                    ballOutOfFieldposition = new Vector3(-52.6f, BALL_GROUND_POSITION_Y, -25);
                }
            }
        }

        if (transform.position.x > Game.FIELD_BOUNDARY_UPPER_X)
        {
            isOutOfField = true;
            ballOutOfFieldTimeOut = 1.0f;
            if (Game.Instance.TeamLastTouched.Number == 1)
            {
                outOfFieldEvent = OutOfFieldEvent_.GoalKick;
                ballOutOfFieldposition = new Vector3(46.58f, BALL_GROUND_POSITION_Y, 5.29f);
            }
            else
            {
                outOfFieldEvent = OutOfFieldEvent_.Corner;
                if (transform.position.z > 0)
                {
                    ballOutOfFieldposition = new Vector3(52.3f, BALL_GROUND_POSITION_Y, 25);
                }
                else
                {
                    ballOutOfFieldposition = new Vector3(52.3f, BALL_GROUND_POSITION_Y, -25);
                }
            }
        }

        if (isOutOfField)
        {
            Debug.Log("out of field: " + transform.position);
            soundWhistle.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.Instance.GameState != GameState_.Playing)
        {
            return;
        }

        if (ballOutOfFieldTimeOut > 0)
        {
            ballOutOfFieldTimeOut -= Time.deltaTime;
            if (ballOutOfFieldTimeOut <= 0)
            {
                TakeThrowInOrGoalKick();
                return;
            }
        }
        else if (delayCheckOutOfField > 0)
        {
            delayCheckOutOfField -= Time.deltaTime;
        }
        else if (!isOutOfField)
        {
            CheckBallOutOfField();
        }

        if (Game.Instance.PassDestinationPlayer != null)
        {
            UpdatePass();
        }
        else if (Game.Instance.PlayerWithBall != null)
        {
            transform.position = Game.Instance.PlayerWithBall.PlayerBallPosition.position;
        }

        //Debug.Log(speed.x);

        if (transform.position.x > 15 && speed.x > 10 && Game.Instance.TeamLastTouched.Number == 1)
        {
            Game.Instance.GoalKeeperCameraTeam0.enabled = true;
            ((HumanPlayer)Game.Instance.Teams[0].GoalKeeper).Activate();
        }
    }

    void FixedUpdate()
    {
        UpdateBallSpeedAndRotation();
    }

    private void UpdateBallSpeedAndRotation()
    {
        if (Time.deltaTime > 0)
        {
            speed = new Vector3((transform.position.x - previousPosition.x) / Time.deltaTime, 0, (transform.position.z - previousPosition.z) / Time.deltaTime);
        }
        previousPosition.x = transform.position.x;
        previousPosition.z = transform.position.z;
        Vector3 rotationAxis = Vector3.Cross(speed.normalized, Vector3.up);
        transform.Rotate(rotationAxis, -speed.magnitude * 1.8f, Space.World);
    }

    public void ExecutePass(Player player)
    {
        timePassedBall = Time.time;
    }
    
    private void UpdatePass()
    {
        if (Game.Instance.PassDestinationPlayer is HumanPlayer)
        {
            if (Time.time - timePassedBall > 0.2 && playerFollowCamera.Follow != Game.Instance.PassDestinationPlayer.PlayerCameraRoot)
            {
                // switch player
                playerFollowCamera.Follow = Game.Instance.PassDestinationPlayer.PlayerCameraRoot;
            }
        }

        Vector3 movedirection = Game.Instance.PassDestinationPlayer.PlayerBallPosition.position - transform.position;
        if (movedirection.magnitude < 1f)
        {
            // pass arrived
            transform.position = Game.Instance.PassDestinationPlayer.PlayerBallPosition.position;
            Game.Instance.SetPlayerWithBall(Game.Instance.PassDestinationPlayer);
        }
        movedirection.Normalize();
        transform.position += movedirection * PASSING_SPEED * Time.deltaTime;
    }

}
