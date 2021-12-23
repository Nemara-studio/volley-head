using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VollyHead.Online
{
    public class PlayerData : MonoBehaviour
    {
        public static PlayerData instance;

        public string playerName;

        [Header("UI")]
        public GameObject setNameObject;
        public TMP_InputField inputName;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this.gameObject);
            }

            playerName = PlayerPrefs.GetString("Name");
        }

        private void Start()
        {
            if (playerName == string.Empty)
            {
                // TODO: Lobby UI Create Name
                setNameObject.SetActive(true);
            }
            else
            {
                setNameObject.SetActive(false);
            }
        }

        public void SetName(string newName)
        {
            playerName = newName;

            MatchMaker.instance.RequestChangeName(newName);
        }

        #region UI Function
        public void ConfirmSetName()
        {
            if (inputName.text == string.Empty) return;

            SetName(inputName.text);
        }

        public void ChangeSuccess()
        {
            inputName.text = string.Empty;
            setNameObject.SetActive(false);
            PlayerPrefs.SetString("Name", playerName);
        }
        #endregion
    }
}