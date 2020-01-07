using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YoutubePlayer
{
    public class YoutubeLogText : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text m_text;

        // Start is called before the first frame update
        void Start()
        {
            if (!m_text)
                m_text = transform.GetChild(0).GetComponent<TMPro.TMP_Text>();

            //m_text.text = "Welcome. Introduce an URL in the Input Field and click on Play. \n <color=blue>Play</color> button restarts the video with" + "the url introduced. Pause will pause/resume the current video";
            m_text.text = "";

            StartListeners();
        }

        private void OnDestroy()
        {
            StopListeners();
        }

        void StartListeners()
        {
            if (YoutubePlayer.OnVideoPlay == null)
            {
                YoutubePlayer.OnVideoPlay = OnVideoStart;
            }
            else
            {
                YoutubePlayer.OnVideoPlay += OnVideoStart;
            }

            if (YoutubePlayer.OnVideoLoad == null)
            {
                YoutubePlayer.OnVideoLoad = OnVideoLoad;
            }
            else
            {
                YoutubePlayer.OnVideoLoad += OnVideoLoad;
            }

            if (YoutubePlayer.OnDownloading == null)
            {
                YoutubePlayer.OnDownloading = OnStartDownload;
            }
            else
            {
                YoutubePlayer.OnDownloading += OnStartDownload;
            }

            if (YoutubePlayer.OnEndDownload == null)
            {
                YoutubePlayer.OnEndDownload = OnEndDownload;
            }
            else
            {
                YoutubePlayer.OnEndDownload += OnEndDownload;
            }
            if (YoutubePlayer.OnTextClear == null)
            {
                YoutubePlayer.OnTextClear = OnTextClear;
            }
            else
            {
                YoutubePlayer.OnTextClear += OnTextClear;
            }  
            
            if (YoutubePlayer.OnErrorShown == null)
            {
                YoutubePlayer.OnErrorShown = OnErrorShown;
            }
            else
            {
                YoutubePlayer.OnErrorShown += OnErrorShown;
            }
        }

        void StopListeners()
        {
            if (YoutubePlayer.OnTextClear != null)
            {
                YoutubePlayer.OnTextClear -= OnTextClear;
            }

            if (YoutubePlayer.OnVideoPlay != null)
            {
                YoutubePlayer.OnVideoPlay -= OnVideoStart;
            }

            if (YoutubePlayer.OnEndDownload != null)
            {
                YoutubePlayer.OnEndDownload -= OnEndDownload;
            }

            if (YoutubePlayer.OnDownloading != null)
            {
                YoutubePlayer.OnDownloading -= OnStartDownload;
            }

            if (YoutubePlayer.OnVideoLoad != null)
            {
                YoutubePlayer.OnVideoLoad -= OnVideoLoad;
            }

            if (YoutubePlayer.OnAudioExtracting != null)
            {
                YoutubePlayer.OnAudioExtracting -= OnAudioExtracting;
            }

            if (YoutubePlayer.OnEndAudioExtracting != null)
            {
                YoutubePlayer.OnEndAudioExtracting -= OnEndAudioExtracting;
            } 
            
            if (YoutubePlayer.OnErrorShown != null)
            {
                YoutubePlayer.OnErrorShown -= OnErrorShown;
            }
        }

        void OnErrorShown(string _error)
        {
            if (m_text.text == "Extracting Audio..." || m_text.text == "Downloading..." || m_text.text == "Loading...")
                m_text.text = "";

            m_text.gameObject.SetActive(true);
            m_text.text += "\nError: " + _error;
        }

        void OnTextClear()
        {
            m_text.gameObject.SetActive(false);
            m_text.text = "";
        }

        void OnEndAudioExtracting()
        {
            m_text.gameObject.SetActive(false);
        }

        void OnAudioExtracting()
        {
            m_text.gameObject.SetActive(true);
            m_text.text = "Extracting Audio...";
        }

        void OnStartDownload()
        {
            m_text.gameObject.SetActive(true);
            m_text.text = "Downloading...";
        }

        void OnEndDownload()
        {
            m_text.gameObject.SetActive(false);
        }

        void OnVideoStart()
        {
            m_text.gameObject.SetActive(false);
        }

        void OnVideoLoad()
        {
            m_text.gameObject.SetActive(true);
            m_text.text = "Loading...";
        }
    }

}
