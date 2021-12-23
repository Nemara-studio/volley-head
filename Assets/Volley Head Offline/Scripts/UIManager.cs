using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VollyHead.Offline
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        public Text[] teamScoreText;
        public GameObject serveUI;
        public GameObject movementUI;

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

        public void SetServeUI()
        {
            serveUI.SetActive(true);
            movementUI.SetActive(false);
        }

        public void SetMoveUI()
        {
            serveUI.SetActive(false);
            movementUI.SetActive(true);
        }
    }

}
