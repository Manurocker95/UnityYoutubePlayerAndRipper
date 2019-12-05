using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Audio;
using UnityEngine.UI;
using UnityEngine.Video;
using NAudio.Wave;
using VideoLibrary;

namespace YoutubePlayer
{
    public class YoutubeAudioDownload : MonoBehaviour, IProgress<double>
    {
        public YoutubePlayer youtubePlayer;
        public Environment.SpecialFolder destination;
        private Image downloadProgress;
        private float progress;
        
        private void Start()
        {
            downloadProgress = GetComponentsInChildren<Image>().First(image => image.gameObject != gameObject);
            if (downloadProgress.sprite == null)
            {
                var texture = Texture2D.whiteTexture;
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 100);
                downloadProgress.sprite = sprite;
            }
        }

        public async void DownloadAudio()
        {
            YoutubePlayer.OnDownloading.Invoke();
            var youtube = YouTube.Default;
            var video = await youtube.GetVideoAsync(youtubePlayer.youtubeUrl);
            try
            {
                var bytes = await video.GetBytesAsync();
                var v = Environment.GetFolderPath(destination);
                var filePath = v + @"\" + video.FullName + ".mp3";
                File.WriteAllBytes(filePath, bytes);

                YoutubePlayer.OnEndDownload.Invoke();

                Debug.Log($"Audio saved to {Path.GetFullPath(filePath)}");
            }
            catch (Exception e)
            {
                Debug.Log($"Couldn't save Audio save audio.");
                YoutubePlayer.OnEndDownload.Invoke();
            }


        }

        public void Report(double value)
        {
            progress = (float)value;
        }

        private void Update()
        {
            if (downloadProgress)
                downloadProgress.fillAmount = progress;
        }
    }
}