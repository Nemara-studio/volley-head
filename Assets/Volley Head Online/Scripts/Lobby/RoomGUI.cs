using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VollyHead.Online
{
    public class RoomGUI : MonoBehaviour
    {
        public GameObject readyBtn;
        public GameObject startBtn;
        public TMP_Text roomCodeText;

        public List<PlayerGUI> teams1GUI;
        public List<PlayerGUI> teams2GUI;
        public GameObject changeTeamBtn;

        public void ResetRoom()
        {
            foreach (PlayerGUI player in teams1GUI)
            {
                player.ResetData();
            }

            foreach (PlayerGUI player in teams2GUI)
            {
                player.ResetData();
            }

            changeTeamBtn.SetActive(false);
        }

        public void SetRoom(string matchId, bool isRoomMaster)
        {
            roomCodeText.text = $"{matchId}";

            if (isRoomMaster)
            {
                startBtn.SetActive(true);
            }
            else
            {
                startBtn.SetActive(false);
            }
        }


    }
}