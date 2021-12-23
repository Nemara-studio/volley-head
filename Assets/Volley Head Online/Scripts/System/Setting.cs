using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace VollyHead.Online
{
    public class Setting : MonoBehaviour
    {
        public static Setting instance;

        // sound
        public AudioMixer mixer;
        public string volumeName;

        public bool isMuted = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            LoadSetting();
        }

        public void ToogleSound()
        {
            isMuted = !isMuted;

            UpdateVolume();

            SaveSetting();
        }

        private void UpdateVolume()
        {
            if (isMuted)
            {
                mixer.SetFloat(volumeName, -80);
            }
            else
            {
                mixer.SetFloat(volumeName, 0);
            }
        }


        private void SaveSetting()
        {
            // save sound on or off
            PlayerPrefs.SetInt("isMuted", isMuted ? 1 : 0);
        }

        private void LoadSetting()
        {
            if (!PlayerPrefs.HasKey("isMuted")) return;

            isMuted = PlayerPrefs.GetInt("isMuted") == 1 ? true : false;
            UpdateVolume();
        }
    }
}