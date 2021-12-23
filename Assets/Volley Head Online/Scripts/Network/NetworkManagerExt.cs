using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace VollyHead.Online
{
    public class NetworkManagerExt : NetworkManager
    {
        private List<NetworkConnection> playerConns1 = new List<NetworkConnection>();
        private List<NetworkConnection> playerConns2 = new List<NetworkConnection>();
        private int currentPlayer = 0;
        public int playerToPlay = 2;

        [Header("Object in Game")]
        public GameObject ballPrefabs;

        public override void OnServerConnect(NetworkConnection conn)
        {
            // base.OnServerConnect(conn);

            // add connection player
            if (currentPlayer % 2 == 0)
            {
                playerConns1.Add(conn);
                currentPlayer++;
            }
            else
            {
                playerConns2.Add(conn);
                currentPlayer++;
            }

        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);

            // check if player ready to start
            if (currentPlayer == playerToPlay)
            {
                OnStartedGame();
            }
        }

        private void OnStartedGame()
        {
            // spawn ball
            GameObject ball = Instantiate(ballPrefabs);
            NetworkServer.Spawn(ball);

            // spawn player
            List<Player> playerTeam1 = new List<Player>();
            List<Player> playerTeam2 = new List<Player>();
            foreach (NetworkConnection playerConn in playerConns1)
            {
                GameObject player = Instantiate(NetworkManager.singleton.playerPrefab);
                NetworkServer.AddPlayerForConnection(playerConn, player);
                playerTeam1.Add(player.GetComponent<Player>());
            }

            foreach (NetworkConnection playerConn in playerConns2)
            {
                GameObject player = Instantiate(NetworkManager.singleton.playerPrefab);
                NetworkServer.AddPlayerForConnection(playerConn, player);
                playerTeam2.Add(player.GetComponent<Player>());
            }

            // GameManager.instance.SetGameData(playerTeam1, playerTeam2, ball);
            // GameManager.instance.StartGame();
        }
    }
}