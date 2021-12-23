using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VollyHead.Online
{
    public class AutoConnect : MonoBehaviour
    {
        public float reconnectTimeout = 10f;

        [Header("CONNECTING UI")]
        public GameObject connectingUI;


        // Start is called before the first frame update
        void Start()
        {
            if (!Application.isBatchMode)
            { //Headless build
                if (!NetworkClient.ready)
                {
                    StartCoroutine(TryToConnect());
                }
            }
            else
            {
                Debug.Log($"=== Server Build ===");
            }
        }

        private IEnumerator TryToConnect()
        {
            Coroutine connectToServerCoroutine = null;

            if (!NetworkClient.ready)
            {
                connectingUI.SetActive(true);

                connectToServerCoroutine = StartCoroutine(ConnectToServer());
            }

            yield return new WaitUntil(() => NetworkClient.ready);

            connectingUI.SetActive(false);

            StopCoroutine(connectToServerCoroutine);
        }

        private IEnumerator ConnectToServer()
        {
            while (!NetworkClient.ready)
            {
                NetworkManager.singleton.StartClient();

                yield return new WaitForSeconds(reconnectTimeout);
            }
        }
    }
}