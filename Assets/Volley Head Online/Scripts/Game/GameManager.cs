using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

namespace VollyHead.Online
{
    public class GameManager : NetworkBehaviour
    {
        [Serializable]
        public struct Team
        {
            [HideInInspector] public int score;
            public List<Player> teamPlayer;
            public List<Transform> startingPos;
            public Transform serveArea;
            public Transform ballPosOnServe;
        }

        [Header("Game")]
        private Guid matchGuid;
        public UIManager gameUI;
        public int targetScore = 5;
        public float timeToNewRound;
        private bool isPlaying;

        [Header("Team Info")]
        public Team[] teams;
        private Player currentPlayerToServe;

        [Header("Environment")]
        public Ball ball;
        public Collider2D midBoundary;

        [Header("Audio")]
        public AudioSource scoredAudio;
        public AudioSource whistleEndAudio;


        [Server]
        public void InitializeGameData(Guid matchGuid, List<Player> playerTeam1, List<Player> playerTeam2, Ball _ball)
        {
            this.matchGuid = matchGuid;

            foreach (Player player in playerTeam1)
            {
                player.InitializeDataServerPlayer(this, 0);
                teams[0].teamPlayer.Add(player);
            }

            foreach (Player player in playerTeam2)
            {
                player.InitializeDataServerPlayer(this, 1);
                teams[1].teamPlayer.Add(player);
            }


            ball = _ball;
            ball.InitializeBallData(this);
        }

        [Server]
        public void StartGame()
        {
            isPlaying = true;
            RandomFirstTeamToServe();
            SetStartingPosition();
        }

        [Server]
        private void RandomFirstTeamToServe()
        {
            // random
            int teamService = UnityEngine.Random.Range(0, 2);
            RandomPlayerToServe(teamService);
        }

        [Server]
        private void RandomPlayerToServe(int _team)
        {
            int rand = UnityEngine.Random.Range(0, teams[_team].teamPlayer.Count);
            currentPlayerToServe = teams[_team].teamPlayer[rand];
        }

        [Server]
        private void SetStartingPosition()
        {
            // set player starting pos
            currentPlayerToServe.StartServeRpc();
            currentPlayerToServe.StartServe();
            currentPlayerToServe.transform.position = teams[currentPlayerToServe.GetTeam()].serveArea.position;
            ball.transform.position = teams[currentPlayerToServe.GetTeam()].ballPosOnServe.position;

            // set another player position
            foreach (Team team in teams)
            {
                int index = 0;
                foreach (Player player in team.teamPlayer)
                {
                    if (player != currentPlayerToServe)
                    {
                        player.transform.position = team.startingPos[index].position;
                        player.StartMove();
                        index++;
                    }
                }
            }

            ball.GetComponent<Ball>().ServeMode();
            ball.GetComponent<Ball>().StartNewRound();
        }

        [Server]
        public IEnumerator StartNewRound(int serviceTeam)
        {
            yield return new WaitForSeconds(timeToNewRound);

            if (isPlaying)
            {
                RandomPlayerToServe(serviceTeam);
                SetStartingPosition();
            }
        }

        [Server]
        public void Scored(int scoredTeam)
        {
            // add score
            teams[scoredTeam].score += 1;

            // scored ui
            RpcScored(teams[0].score, teams[1].score);

            if (CheckWin(scoredTeam)) return;

            // start a new round
            StartCoroutine(StartNewRound(scoredTeam));
        }

        [ClientRpc]
        private void RpcScored(int team1Score, int team2Score)
        {
            Debug.Log($"{team1Score} || {team2Score}");

            // Score UI updated
            gameUI.SetScore(team1Score, team2Score);

            scoredAudio.Play();
        }

        [Server]
        private bool CheckWin(int scoredTeam)
        {
            if (teams[scoredTeam].score == targetScore)
            {
                GameEnd(scoredTeam);
                return true;
            }

            return false;
        }

        [Server]
        private void GameEnd(int winnerTeam)
        {
            int loserTeam = winnerTeam == 0 ? 1 : 0;

            foreach (Player player in teams[winnerTeam].teamPlayer)
            {
                RpcGameWin(player.connectionToClient);
            }

            foreach (Player player in teams[loserTeam].teamPlayer)
            {
                RpcGameLose(player.connectionToClient);
            }

            PlayEndAudioRpc();

            isPlaying = false;
            MatchMaker.instance.OnPlayerDisconnected -= OnPlayerDisconnected;
            StartCoroutine(GameManagerTimeoutEndMatch(60f, matchGuid));
        }

        [TargetRpc]
        private void RpcGameLose(NetworkConnection target)
        {
            gameUI.SetGameEndUI(false);

            Debug.Log($"Lose...");
        }

        [TargetRpc]
        private void RpcGameWin(NetworkConnection target)
        {
            gameUI.SetGameEndUI(true);

            Debug.Log($"Win...");
        }

        #region Audio

        [ClientRpc]
        private void PlayEndAudioRpc()
        {
            whistleEndAudio.Play();
        }

        #endregion

        public void OnPlayerDisconnected(NetworkConnection conn)
        {
            string matchId = MatchMaker.instance.playerInfos[conn].matchId;
            isPlaying = false;
            StartCoroutine(ServerPlayerDisconnected(conn, matchId));
        }

        // match disconnected
        public IEnumerator ServerPlayerDisconnected(NetworkConnection conn, string matchId)
        {
            MatchMaker.instance.OnPlayerDisconnected -= OnPlayerDisconnected;

            PlayerDisconnectUIRpc();

            NetworkServer.Destroy(ball.gameObject);

            // Skip a frame so the message goes out ahead of object destruction
            yield return null;

            StartCoroutine(GameManagerTimeoutEndMatch(10f, matchId.ToGuid()));
        }

        [ClientRpc]
        private void PlayerDisconnectUIRpc()
        {
            gameUI.ShowPlayerDisconnectedUI();
        }

        private IEnumerator GameManagerTimeoutEndMatch(float delayTime, Guid matchGuid)
        {
            yield return new WaitForSeconds(delayTime);


            if (MatchMaker.instance.matchConnections.ContainsKey(matchGuid))
            {
                if (MatchMaker.instance.matchConnections[matchGuid].Count > 0)
                {
                    foreach (NetworkConnection playerConn in MatchMaker.instance.matchConnections[matchGuid])
                    {
                        RpcExitGame(playerConn);
                        NetworkServer.RemovePlayerForConnection(playerConn, true);
                    }
                }
            }

            MatchMaker.instance.OnServerMatchEnded(matchGuid);

            NetworkServer.Destroy(gameObject);
        }

        [TargetRpc]
        private void RpcExitGame(NetworkConnection conn)
        {
            SceneManager.UnloadSceneAsync(MatchMaker.instance.gameScene);
            LobbyUIManager.instance.ResetLobby();
        }

        [Client]
        public void BackToMenu()
        {
            SceneManager.UnloadSceneAsync(MatchMaker.instance.gameScene);
            LobbyUIManager.instance.ResetLobby();
            CmdBackMenu();

            // Destroy(gameObject);
        }

        [Command(requiresAuthority = false)]
        private void CmdBackMenu(NetworkConnectionToClient conn = null)
        {
            MatchMaker.instance.ResetPlayerInfo(conn);
            MatchMaker.instance.RemovePlayerFromMatch(conn, matchGuid);
        }
    }
}