using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VollyHead.Offline
{
    public class Ball : MonoBehaviour
    {
        public BoxCollider2D midBoundary;
        public float maxSpeed;
        private string onAreaTeam;

        private bool isPlayed = true;
        private Rigidbody2D ballRb;

        private float defaultGravityScale;

        private int lastTeamTouchBall;
        private int latestTeamTouchCount;

        private Player latestPlayerTouchBall = null;

        // Start is called before the first frame update
        void Start()
        {
            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            ballRb = GetComponent<Rigidbody2D>();
            defaultGravityScale = ballRb.gravityScale;

            Physics2D.IgnoreCollision(collider, midBoundary);
        }

        private void FixedUpdate()
        {
            CheckMaxSpeed();
        }

        // limit the velocity of ball
        private void CheckMaxSpeed()
        {
            if (ballRb.velocity.magnitude > maxSpeed)
            {
                ballRb.velocity = Vector3.ClampMagnitude(ballRb.velocity, maxSpeed);
            }
        }

        public void ServeMode()
        {
            isPlayed = true;
            ballRb.velocity = Vector2.zero;
            ballRb.gravityScale = 0;
        }

        public void ShootServe(float power)
        {
            ballRb.gravityScale = 0.8f;
            ballRb.AddForce(new Vector2(20f * power, 5f * power));
        }

        public void ResetBallData()
        {
            lastTeamTouchBall = -1;
            latestTeamTouchCount = 0;
            latestPlayerTouchBall = null;
        }

        private void Scored(int scoredTeam)
        {
            isPlayed = false;
            ResetBallData();
            GameManager.instance.Scored(scoredTeam);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<Player>() != null)
            {
                if (isPlayed)
                {
                    Player playerCollided = collision.gameObject.GetComponent<Player>();
                    int collideTeamID = playerCollided.GetTeam();

                    if (lastTeamTouchBall == collideTeamID)
                    {
                        latestTeamTouchCount++;
                        if (latestTeamTouchCount > 3)
                        {
                            // add score to enemy
                            int scoredTeam = collideTeamID == 0? 1 : 0;

                            Scored(scoredTeam);
                            return;
                        }
                    }
                    else
                    {
                        lastTeamTouchBall = collideTeamID;
                        latestTeamTouchCount = 1;
                    }

                    if (latestPlayerTouchBall == playerCollided)
                    {
                        int scoredTeam = collideTeamID == 0? 1 : 0;

                        // add score to enemy
                        Scored(scoredTeam);
                        return;
                    }
                    else
                    {
                        latestPlayerTouchBall = playerCollided;
                    }
                }
            }

            if (collision.gameObject.tag == "Ground")
            {
                if (isPlayed)
                {
                    if (onAreaTeam == "Area1")
                    {
                        Debug.Log($"Team 2 has scored..");
                        Scored(1);
                    }
                    else if (onAreaTeam == "Area2")
                    {
                        Debug.Log($"Team 1 has scored..");
                        Scored(0);
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag != onAreaTeam)
            {
                ResetBallData();
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Area1")
            {
                onAreaTeam = collision.gameObject.tag;
            }
            else if (collision.gameObject.tag == "Area2")
            {
                onAreaTeam = collision.gameObject.tag;
            }
        }

    }
}

