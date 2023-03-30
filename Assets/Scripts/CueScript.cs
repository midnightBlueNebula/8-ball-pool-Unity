using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueScript : MonoBehaviour
{
    private GameManager gm;

    private float aimSpeed = 15.0f;

    public GameObject whiteBall;
    private Rigidbody whiteBallRb;

    private bool isAiming = false;
    private bool isPositioningCue = false;

    private Vector3 offset = new Vector3(0, 0, -7);

    private float horizontalInput;
    private float mouseYInput;

    private float maxDistanceToBall = 10.0f;

    private Vector3 lastPositionOfCue;

    private float cueSpeed;


    // Start is called before the first frame update
    void Start()
    {
        IgnoreObjectsForCollision();

        HideCue();

        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();

        lastPositionOfCue = transform.position;

        whiteBallRb = whiteBall.GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        if (gm.eightBallInPocket) // If 8-Ball in pocket
        {
            return; // Game Over. No more moving cue around.
        }

        if (gm.movingWhiteBall)
        {
            isAiming = false; // If moving white ball, place it before start aiming with cue.
        }


        if (!isAiming && gm.HaveAllBallsStopped())
        // !isAiming placed before to prevent unnecessary loop in gm.HaveAllBallsStopped().
        {
            isPositioningCue = true;
        }
        

        gm.MoveWhiteBallIfFoulCommited();

        CueManager(); // Move cue with user inputs.
    }


    private void IgnoreObjectsForCollision()
    {
        GameObject[] tableParts = GameObject.FindGameObjectsWithTag("Table");

        GameObject[] otherTableParts = GameObject.FindGameObjectsWithTag("Bottom");

        GameObject[] sides = GameObject.FindGameObjectsWithTag("Sides");

        GameObject[] otherBalls = GameObject.FindGameObjectsWithTag("Ball");


        foreach (GameObject part in tableParts)
        {
            Physics.IgnoreCollision(part.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        }

        foreach (GameObject part in otherTableParts)
        {
            Physics.IgnoreCollision(part.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        }

        foreach (GameObject ball in otherBalls)
        {
            Physics.IgnoreCollision(ball.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        }

        foreach (GameObject side in sides)
        {
            Physics.IgnoreCollision(side.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("White Ball"))
        {
            gm.turnEnded = true;

            whiteBallRb.AddForce(GetHitDirection() * 10 * cueSpeed);

            isAiming = false;

            HideCue();
        }
    }


    private void CueManager()
    {
        if (gm.foulCommitted)
        {
            return; // Place white ball before control the cue.
        }

        if (isPositioningCue)
        {
            ResetWhiteBallRotation();

            PositionCue(); // Position cue behind white ball.

            isPositioningCue = false; // Cue positioned behind the white ball.

            isAiming = true; // Now controlling/aiming the cue is available.

            gm.ClearTurnTraction(); // Reset foul tracker for new turn.
        }

        if (isAiming)
        {
            HandleThrust(); // Move cue forward/backward with Mouse Y axis movement to hit white ball.

            HandleRotation(); // Rotate cue around white ball to determine hit direction.
        }
    }

    private void PositionCue()
    {
        //gm.SwitchTurnIfNecessary(); // Passes cue to other player if current turn ended.

        transform.position = whiteBall.transform.position + offset; // Position cue behind white ball.
    }

    
    private void HideCue()
    {
        transform.position = new Vector3(0, -1000, 0);
        transform.eulerAngles = new Vector3(0, 0, 0);
    }


    private Vector3 GetHitDirection()
    {
        return whiteBall.transform.position - gameObject.transform.position;
    }


    private void HandleThrust()
    {
        mouseYInput = Input.GetAxis("Mouse Y");

        Vector3 positionBeforeThrust = transform.position;

        transform.Translate(Vector3.forward * mouseYInput * Time.deltaTime * aimSpeed);

        cueSpeed = (positionBeforeThrust - transform.position).magnitude / Time.deltaTime;

        KeepCueInTheLimits();
    }


    private void HandleRotation()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        transform.RotateAround(whiteBall.transform.position, 
                               Vector3.up, 
                               Time.deltaTime * horizontalInput * aimSpeed);

        lastPositionOfCue = transform.position;
    }


    private float GetDistanceToBall()
    {
        return (whiteBall.transform.position - transform.position).magnitude;
    }


    private void KeepCueInTheLimits()
    {
        if (GetDistanceToBall() > maxDistanceToBall)
        {
            transform.position = lastPositionOfCue;
        }
        else
        {
            lastPositionOfCue = transform.position;
        }
    }


    private void ResetWhiteBallRotation()
    {
        whiteBall.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 
                                                      transform.eulerAngles.y, 
                                                      transform.eulerAngles.z);
    }
}
