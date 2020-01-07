using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YoutubePlayer
{
    public class DownloadSensitiveButton : MonoBehaviour
    {
        [SerializeField] private Button m_button;

        // Start is called before the first frame update
        void Start()
        {
            if (!m_button)
                m_button = GetComponent<Button>();


            YoutubePlayer.OnDownloading += () => { m_button.interactable = false; };
            YoutubePlayer.OnEndDownload += () => { m_button.interactable = true; };


            YoutubePlayer.OnAudioExtracting += () => { m_button.interactable = false; };
            YoutubePlayer.OnEndAudioExtracting += () => { m_button.interactable = true; };
        }

        private void OnDestroy()
        {
            YoutubePlayer.OnDownloading -= () => { m_button.interactable = false; };
            YoutubePlayer.OnEndDownload -= () => { m_button.interactable = true; };


            YoutubePlayer.OnAudioExtracting -= () => { m_button.interactable = false; };
            YoutubePlayer.OnEndAudioExtracting -= () => { m_button.interactable = true; };
        }

    }

}
