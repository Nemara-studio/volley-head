using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VollyHead.Offline
{
    public class Player : MonoBehaviour
    {
        public enum PlayerState
        {
            Serve,
            Move
        }

        public bool testingPlayer = false;

        public int teamID;
        public float speed;
        public float jumpForce;
        public Transform serviceArea;
        public GameObject serviceBallPos;
        public Ball ball;
        [SerializeField] private LayerMask layerMask;

        public Button serveButton;
        public Slider serveSlider;
        public float servePowerMultiplier = 100f;
        public float servePower;
        public float inputServe;

        private PlayerState playerState = PlayerState.Move;
        public Rigidbody2D playerRb;
        public Vector2 inputHorizontal;


        private void Start()
        {
            playerRb = GetComponent<Rigidbody2D>();
            
            InitializeInputPlayer();
        }

        private void Update()
        {
            if (!testingPlayer) return;

            InputPlayer();
            if (playerState == PlayerState.Serve)
                IncreasingServePower();
        }

        private void FixedUpdate()
        {
            if (playerState == PlayerState.Move)
                Move();

        }

        private void InitializeInputPlayer()
        {
            
        }

        private void InputPlayer()
        {
            if (playerState == PlayerState.Move)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    Jump();
                }
            }
            else if (playerState == PlayerState.Serve)
            {
                
            }
        }

        public void Move()
        {
            playerRb.velocity = new Vector2(speed * inputHorizontal.x * Time.fixedDeltaTime * 10, playerRb.velocity.y);
        }

        public void Jump()
        {


            playerRb.AddForce(new Vector2(0, jumpForce * 10));
        }

        public void ServeMode()
        {
            playerState = PlayerState.Serve;
            inputHorizontal = Vector2.zero;
            servePower = 0;
        }

        public void IncreasingServePower()
        {
            if (servePower >= 1)
            {
                servePower = 1;
            } 
            else
            {
                servePower += Time.deltaTime * inputServe;
            }

            if (serveSlider != null)
                serveSlider.value = servePower;
        }

        public void Serve()
        {
            ball.ShootServe(servePower * servePowerMultiplier);
            playerState = PlayerState.Move;

            // set ui
            servePower = 0;
            serveSlider.value = 0;
            UIManager.instance.SetMoveUI();
        }

        private bool GroundCheck()
        {
            float offsetHeight = 1f;
            Collider2D collider = GetComponent<Collider2D>();
            RaycastHit2D raycastHit = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.down, offsetHeight, layerMask);

            return raycastHit.collider != null;
        }

        public int GetTeam()
        {
            return teamID;
        }
    }

}
