using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VollyHead.Online
{
    public class CrowdSound : MonoBehaviour
    {
        public List<AudioClip> crowdSounds;
        public int minDelayTime = 3;
        public int maxDelayTime = 5;

        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();

            StartCoroutine(PlayCrowdAudio());
        }

        private IEnumerator PlayCrowdAudio()
        {
            int indexSound = Random.Range(0, crowdSounds.Count);

            audioSource.PlayOneShot(crowdSounds[indexSound]);

            yield return new WaitUntil(() => !audioSource.isPlaying);

            PlayCrowdAudio();
        }
    }
}