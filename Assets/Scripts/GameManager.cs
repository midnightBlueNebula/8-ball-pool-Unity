using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private GameObject whiteBall;
    private GameObject ghostBall;

    private Rigidbody whiteBallRb;

    public bool whiteBallSwappedWithGhostBall = false;

    public float tableHeight = 5.5f;

    public Vector3 defaultPosition;
    public Vector3 hidePosition;

    public bool movingWhiteBall = false;

    public bool foulCommitted = false;
    public bool ballInpocket = false;

    public bool whiteBallHittedBall = false;

    public string currentTurn = "Player 1";
    public string currentTarget = "TBD";

    public bool isEightBallTarget = false;

    public bool turnEnded = false;

    public bool eightBallInPocket = false;

    private TMP_Text turnInfo;


    // Start is called before the first frame update
    void Start()
    {
        defaultPosition = new Vector3(0.17f, tableHeight, -13.4f);
        hidePosition = new Vector3(500, 0, 500);

        whiteBall = GameObject.FindGameObjectWithTag("White Ball");
        ghostBall = GameObject.FindGameObjectWithTag("Ghost Ball");

        whiteBallRb = whiteBall.GetComponent<Rigidbody>();

        turnInfo = GameObject.FindGameObjectWithTag("Turn Info").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInfo();

        if(eightBallInPocket && HaveAllBallsStopped())
        // If 8-Ball in pocket, wait until all balls to stop for deciding winner.
        {
            WeHavaAWinner(); // Shows who is the winner.
            return; // Game Over. Don't switch turn.
        }

        if (turnEnded && HaveAllBallsStopped())
        // turnEnded placed before HaveAllBallsStopped(). so if turn did not ended,
        // program will not loop through objects unnecessarily.
        {
            SwitchTurnIfNecessary();
        }
    }


    void UpdateInfo()
    {
        // Display info about current player and their target.
        string info = currentTurn + "'s turn - Target: ";

        if (isEightBallTarget) // If all targets cleared, 8-Ball is the target.
        {
            turnInfo.text = info + "8-Ball";
            return;
        }

        turnInfo.text = info + currentTarget; // Display current target for current player.
    }


    public void AssignTargets(int number)
    {
        // Assign targets based on first ball to pocket.
        if(number < 8) // Solid ball.
        {
            currentTarget = "Solid";
        } 
        else if (number > 8) // Stripped ball.
        {
            currentTarget = "Stripped";
        }
    }

    public void SwitchTurnIfNecessary()
    {
        turnEnded = false;

        if(ballInpocket && !foulCommitted)
        {
            return; // No switching turn, current player continues playing.
        }

        if (!whiteBallHittedBall) // If white ball did not hit any ball, foul committed.
        {
            Foul();
        }

        // Switch current player.
        if(currentTurn == "Player 1")
        {
            currentTurn = "Player 2";
        } 
        else
        {
            currentTurn = "Player 1";
        }

        // Switch current target.
        if(currentTarget == "Solid")
        {
            currentTarget = "Stripped";
        }
        else if(currentTarget == "Stripped")
        {
            currentTarget = "Solid";
        }

        SetEightBallAsTargetIfTargetsCleared();
    }


    public void ClearTurnTraction() // Clear turn traction before start of new turn.
    {
        ballInpocket = false;
        whiteBallHittedBall = false;
    }

    public void SetEightBallAsTargetIfTargetsCleared()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

        bool noSolidBall = true;
        bool noStrippedBall = true;

        foreach(GameObject ball in balls)
        {
            BallScripts ballProperties = ball.GetComponent<BallScripts>();
            
            if(ballProperties.number > 8) {
                noStrippedBall = false; // Stripped balls are not cleared.
            }

            if(ballProperties.number < 8)
            {
                noSolidBall = false; // Solid balls are not cleared.
            }
        }

        if((currentTarget == "Solid" && noSolidBall) || (currentTarget == "Stripped" && noStrippedBall))
        {
            isEightBallTarget = true; // 8-Ball is the target in this turn.
        } 
        else
        {
            isEightBallTarget = false;
        }
    }

    public void WeHavaAWinner()
    {
        // If 8-Ball is the target, current player Wins.
        if (isEightBallTarget && !foulCommitted) // If foul committed, other player wins.
        {
            Debug.Log(currentTurn + " Wins");
            return;
        }

        // Else other player wins.
        if(currentTurn == "Player 1") // Other player is Player 2.
        {
            Debug.Log("Player 2 Wins");
            // Player 2 wins.
        }
        else
        {
            Debug.Log("Player 1 Wins"); // Other player is Player 1.
            // Player 1 wins.
        }
    }

    public void Foul()
    {
        foulCommitted = true;
    }

    // Place white ball on anywhere on table if foul committed.
    // Before turn start.
    public void MoveWhiteBallIfFoulCommited()
    {
        if (foulCommitted && !whiteBallSwappedWithGhostBall && HaveAllBallsStopped())
        {
            whiteBall.transform.position = hidePosition;
            ghostBall.transform.position = defaultPosition;

            whiteBallSwappedWithGhostBall = true;
            movingWhiteBall = true;
        }
    }

    public void BallHit()
    {
        whiteBallHittedBall = true;
    }

    
    public bool HaveAllBallsStopped()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

        foreach (GameObject ball in balls)
        {
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();

            if (ballRb.velocity.magnitude != 0)
            {
                return false;
            }
        }

        return whiteBallRb.velocity.magnitude == 0;
    }


    public void EightBallInPocket()
    {
        eightBallInPocket = true;
    }
}
