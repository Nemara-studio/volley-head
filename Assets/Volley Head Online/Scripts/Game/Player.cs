using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

namespace VollyHead.Online
{
    public class Player : NetworkBehaviour
    {
        public enum PlayerState
        {
            MOVE,
            SERVE
        }

        private GameManager gameManager;
        public SpriteRenderer graphic;
        public TMP_Text nameText;
        [SyncVar] public string playerName;
        [SyncVar] private int team;

        [Header("Move Attribute")]
        public float speed;
        public float jumpForce;
        public Transform groundChecker;
        public LayerMask groundLayer;
        private Rigidbody2D playerRb;
        [SerializeField] private NetworkAnimator anim;
        private int inputHorizontal;

        [Header("Serve Attribute")]
        public float servePowerMultiplier = 100f;
        private float servePower;

        [Header("Audio")]
        [SerializeField] private AudioSource walkSound;
        private bool walkSoundPlayed = false;
        [SerializeField] private AudioSource jumpSound;
        private AudioListener audioListener;

        private PlayerState state = PlayerState.MOVE;
        private PhysicsScene2D physicsScene;

        private void Start()
        {
            playerRb = GetComponent<Rigidbody2D>();

            audioListener = GetComponent<AudioListener>();
            audioListener.enabled = false;
            if (isLocalPlayer)
            {
                audioListener.enabled = true;
            }

            SetGraphic((team == 1));
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                InputPlayer();
                CmdInputHorizontal(inputHorizontal);
            }

            if (isServer)
            {
                UpdateAnimation();
                UpdateWalkAudio();
            }
        }

        private void FixedUpdate()
        {
            if (isServer)
            {
                Move(inputHorizontal);
            }
        }

        [Server]
        public void InitializeDataServerPlayer(GameManager gameManager, int team)
        {
            this.team = team;

            this.gameManager = gameManager;
            if (isServer)
            {
                physicsScene = gameObject.scene.GetPhysicsScene2D();
            }
            CmdInitializeDataClientPlayer(this.gameManager, this.team);
        }

        private void SetGraphic(bool isFlip)
        {
            nameText.text = $"{playerName}";
            graphic.flipX = isFlip;
        }

        [TargetRpc]
        private void CmdInitializeDataClientPlayer(GameManager gameManager, int team)
        {
            this.gameManager = gameManager;

            if (isLocalPlayer)
            {
                gameManager.gameUI.serveButton.onReleased.AddListener(() => CmdServe(servePower));
                gameManager.gameUI.jumpButton.onPressed.AddListener(() => CmdJump());
            }
        }

        private void InputPlayer()
        {
            if (state == PlayerState.MOVE)
            {
                if (gameManager.gameUI.leftButton.IsPressed)
                {
                    inputHorizontal = -1;
                }
                else if (gameManager.gameUI.rightButton.IsPressed)
                {
                    inputHorizontal = 1;
                }
                else
                {
                    inputHorizontal = 0;
                }
            }
            else if (state == PlayerState.SERVE)
            {
                if (gameManager.gameUI.serveButton.IsPressed)
                {
                    IncreasingServePower();
                }
            }
        }

        [Command]
        private void CmdInputHorizontal(int currentInput)
        {
            if (state != PlayerState.MOVE) return;

            inputHorizontal = currentInput;
        }

        #region Player Move

        public void StartMove()
        {
            state = PlayerState.MOVE;
            inputHorizontal = 0;
            servePower = 0;
        }

        [TargetRpc]
        public void StartMoveRpc()
        {
            StartMove();
            gameManager.gameUI.SetMoveUI();
        }

        private void Move(int direction)
        {
            if (state != PlayerState.MOVE)
            {
                playerRb.velocity = Vector2.zero;
                return;
            }

            playerRb.velocity = new Vector2(speed * direction * Time.fixedDeltaTime * 10, playerRb.velocity.y);
        }

        #endregion

        #region Jump

        [Command]
        private void CmdJump()
        {
            Jump();
        }

        private void Jump()
        {
            if (GroundCheck() && state == PlayerState.MOVE)
            {
                playerRb.AddForce(new Vector2(0, jumpForce * 10));
                anim.animator.SetTrigger("Jump");
                TriggerJumpSoundRpc();
            }
        }

        #endregion

        #region Serve
        public void StartServe()
        {
            state = PlayerState.SERVE;
            inputHorizontal = 0;
            servePower = 0;
        }

        [TargetRpc]
        public void StartServeRpc()
        {
            StartServe();
            gameManager.gameUI.SetServeUI();
        }

        private void IncreasingServePower()
        {
            if (servePower >= 1)
            {
                servePower = 1;
            }
            else
            {
                servePower += Time.deltaTime;
            }

            gameManager.gameUI.SetServePowerUI(servePower);
        }

        [Command]
        private void CmdServe(float servePower)
        {
            Serve(servePower);
            StartMove();
            StartMoveRpc();
        }

        private void Serve(float power)
        {
            float finalPower = team == 0 ? power : -power;
            gameManager.ball.GetComponent<Ball>().ServeBall(finalPower * servePowerMultiplier);
        }
        #endregion

        #region Animation

        private void UpdateAnimation()
        {
            anim.animator.SetBool("IsWalk", inputHorizontal != 0);
            anim.animator.SetBool("IsGround", GroundCheck());
        }

        #endregion

        #region Audio

        private void UpdateWalkAudio()
        {
            if (playerRb.velocity.x != 0 && !walkSoundPlayed)
            {
                UpdateWalkAudioRpc(true);
                walkSoundPlayed = true;
            }
            else if (playerRb.velocity.x == 0 && walkSoundPlayed)
            {
                UpdateWalkAudioRpc(false);
                walkSoundPlayed = false;
            }
        }

        [ClientRpc]
        private void UpdateWalkAudioRpc(bool play)
        {
            if (play) walkSound.Play();
            else walkSound.Stop();
        }

        [ClientRpc]
        private void TriggerJumpSoundRpc()
        {
            if (jumpSound == null) return;

            jumpSound.Play();
        }

        #endregion

        private bool GroundCheck()
        {
            return (physicsScene.OverlapPoint(groundChecker.position, groundLayer));
        }

        public int GetTeam() { return team; }
    }
}