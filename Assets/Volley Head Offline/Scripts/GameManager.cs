using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VollyHead.Offline
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public float timeToNewRound = 2f;

        [Serializable]
        public struct Team
        {
            [HideInInspector] public int score;
            public List<Player> teamPlayer;
            public List<Transform> startingPos;
        }

        public Team[] teams = new Team[2];
        public GameObject ball;

        private Player currentPlayerService;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeStartingData();
        }

        public void InitializeStartingData()
        {
            // reset score
            teams[0].score = 0;
            teams[1].score = 0;

            RandomFirstTeamToServe();
            SetStartingPosition();
        }

        /* 
         This is to set the position of servicing player
         */
        private void SetStartingPosition()
        {
            // set player starting pos
            currentPlayerService.transform.position = currentPlayerService.serviceArea.position;
            ball.transform.position = currentPlayerService.serviceBallPos.transform.position;

            // set another player starting pos
            foreach (Team team in teams)
            {
                int pos = 0;
                foreach (Player player in team.teamPlayer)
                {
                    if (player != currentPlayerService)
                    {
                        player.transform.position = team.startingPos[pos].position;
                        pos++;
                    }
                }
            }

            // set serve mode
            currentPlayerService.ServeMode();
            ball.GetComponent<Ball>().ServeMode();

            // set serve ui
            UIManager.instance.SetServeUI();
        }

        /*
         * This is to random the team and player to service first time.
         */
        private void RandomFirstTeamToServe()
        {
            // random
            int teamService = UnityEngine.Random.Range(0, 2);
            RandomPlayerToServe(teamService);
        }

        /*
         * To random player on the team to serve
         */
        private void RandomPlayerToServe(int _team)
        {
            int rand = UnityEngine.Random.Range(0, 2);
            currentPlayerService = teams[_team].teamPlayer[rand];
        }
        

        public IEnumerator StartNewRound(int serviceTeam)
        {
            yield return new WaitForSeconds(timeToNewRound);

            RandomPlayerToServe(serviceTeam);
            SetStartingPosition();
        }

        public void Scored(int scoredTeam)
        {
            // add score
            teams[scoredTeam].score += 1;

            // Score UI updated
            UIManager.instance.teamScoreText[scoredTeam].text = teams[scoredTeam].score.ToString();

            // start a new round
            StartCoroutine(StartNewRound(scoredTeam));
        }
    }

}
