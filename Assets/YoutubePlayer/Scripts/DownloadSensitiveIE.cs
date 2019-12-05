using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YoutubePlayer
{
    public class DownloadSensitiveIE : MonoBehaviour
    {
        TMPro.TMP_InputField m_inputField;

        // Start is called before the first frame update
        void Start()
        {
            if (!m_inputField)
                m_inputField = GetComponent<TMPro.TMP_InputField>();


            YoutubePlayer.OnDownloading += () => { m_inputField.interactable = false; };
            YoutubePlayer.OnEndDownload += () => { m_inputField.interactable = true; };
        }
    }

}
