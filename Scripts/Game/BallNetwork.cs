using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallNetwork : MonoBehaviour
{
    private Transform ballTransform;
    private CenterField centerField;
    private Coroutine ballCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        ballTransform = GetComponent<Transform>();
        centerField = FindAnyObjectByType<CenterField>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        GoalKeeperZone goalKeeperZone = other.GetComponent<GoalKeeperZone>();
        if(goalKeeperZone != null)
        {
            // Move the ball to the center of the level
            if (ballCoroutine != null)
            {
                ballCoroutine = null;
                ballCoroutine = StartCoroutine(Respawn(5.0f));
            }else
            {
                ballCoroutine = StartCoroutine(Respawn(5.0f));
            }

        }
    }

    private IEnumerator Respawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        // move the ball
        Debug.Log("Goal!!!!!!!!!!!");
        Debug.Log("Ball Network: Respawn");

        if(centerField != null)
        {
            ballTransform.position = centerField.GetComponent<Transform>().position;
        }
    }
}
