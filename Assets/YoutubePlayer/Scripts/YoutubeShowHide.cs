using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YoutubePlayer
{
    public class YoutubeShowHide : MonoBehaviour
    {

        public bool showing = true;
        public GameObject[] gos;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void ShowHide()
        {
            showing = !showing;
            foreach (GameObject go in gos)
                go.SetActive(showing);
        }

        public void SetFullScreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        public void Exit()
        {
            Application.Quit();
        }
    }

}
