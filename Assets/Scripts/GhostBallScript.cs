using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Move ghost ball on table to select position for white ball.
public class GhostBallScript : MonoBehaviour
{
    private float speed = 20.0f;

    private GameManager gm;

    private GameObject whiteBall;

    private float mouseXInput;
    private float mouseYInput;


    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();

        whiteBall = GameObject.FindGameObjectWithTag("White Ball");
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.movingWhiteBall)
        {
            MoveBall();
        }
    }


    private void MoveBall()
    {
        mouseXInput = Input.GetAxis("Mouse X");
        mouseYInput = Input.GetAxis("Mouse Y");

        transform.Translate(Vector3.right * mouseXInput * Time.deltaTime * speed);
        transform.Translate(Vector3.forward * mouseYInput * Time.deltaTime * speed);

        if (Input.GetKeyDown(KeyCode.E))
        {
            gm.movingWhiteBall = false;
            gm.foulCommitted = false;
            gm.whiteBallHittedBall = true;
            gm.whiteBallSwappedWithGhostBall = false;

            whiteBall.transform.position = new Vector3(transform.position.x, 
                                                       gm.tableHeight, 
                                                       transform.position.z);

            transform.position = gm.hidePosition;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        transform.Translate((transform.position - other.gameObject.transform.position).normalized);
        // 

        transform.position = new Vector3(transform.position.x,
                                         gm.tableHeight, transform.position.z);
                                      // Keep ghost ball on table level.
    }
}
