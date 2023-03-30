using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Solid balls -> ball with number < 8
// Stripped balls -> ball with number > 8

public class BallScripts : MonoBehaviour
{
    private GameManager gm;

    public int number;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bottom"))
        {
            PocketEvaluation(); // Check if coorect ball is in the pocket
        }
        
        if(gameObject.CompareTag("White Ball") && collision.gameObject.CompareTag("Ball"))
        {
            int number = collision.gameObject.GetComponent<BallScripts>().number;

            FirstBallHitEvaluation(number); // Check if white ball hitted correct ball first.

            gm.BallHit(); // Confirm white ball hitted another ball.
                          // Otherwise, it is automatic foul.
        }
    }

    private void PocketEvaluation()
    {
        if (number == 0)
        {
            gm.Foul();

            return;
        }

        if (number == 8)
        {
            gm.EightBallInPocket();

            return;
        }

        if (gm.currentTarget == "TBD")
        {
            gm.AssignTargets(number);
        } 
        else if (gm.currentTarget == "Solid" && number > 8)
        {
            gm.Foul();
        }
        else if (gm.currentTarget == "Stripped" && number < 8)
        {
            gm.Foul();
        }
       
        // Confirm a ball is in the pocket.
        // If correct ball is in the pocket & no foul committed, current player continues to play.
        gm.ballInpocket = true;

        Destroy(gameObject); // Remove the ball in the pocket.
    }

    private void FirstBallHitEvaluation(int number) // Checks if white ball hitted correct ball first.
    {
        if (gm.whiteBallHittedBall) // Return if already checked.
        {
            return;
        }


        if(number == 8 && !gm.isEightBallTarget)
        {
            gm.Foul();
        }
        else if(number < 8 && gm.currentTarget == "Stripped")
        {
            gm.Foul();
        }
        else if (number > 8 && gm.currentTarget == "Solid")
        {
            gm.Foul();
        }
    }
}
