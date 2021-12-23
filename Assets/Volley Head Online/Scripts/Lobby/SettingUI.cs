using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VollyHead.Online
{
    public class SettingUI : MonoBehaviour
    {
        public TMP_Text playerNameText;

        [Header("SOUND")]
        public GameObject soundOnImage;
        public GameObject soundOffImage;

        private void Start()
        {
            UpdateSoundUI(Setting.instance.isMuted);
        }

        private void Update()
        {
            SetPlayerNameUI();
        }

        private void SetPlayerNameUI()
        {
            playerNameText.text = $"{PlayerData.instance.playerName}";
        }

        #region Sound
        public void ToogleSound()
        {
            Setting.instance.ToogleSound();
            UpdateSoundUI(Setting.instance.isMuted);
        }

        private void UpdateSoundUI(bool isMuted)
        {
            if (isMuted)
            {
                soundOffImage.SetActive(true);
                soundOnImage.SetActive(false);
            }
            else
            {
                soundOffImage.SetActive(false);
                soundOnImage.SetActive(true);
            }
        }
        #endregion
    }
}