using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace YoutubePlayer
{
    public class DownloadYoutubeVideo : MonoBehaviour, IProgress<double>
    {
        public YoutubePlayer youtubePlayer;
        public Environment.SpecialFolder destination;
        private Image downloadProgress;
        private float progress;
        public bool useDataPath = true;
        public bool extractSRT;

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

        public async void Download()
        {
            if (string.IsNullOrEmpty(youtubePlayer.youtubeUrl))
            {
                return;
            }

            YoutubePlayer.OnDownloading.Invoke();

            try
            {
                Debug.Log("Downloading, please wait...");

                var v = (useDataPath ? System.IO.Directory.GetCurrentDirectory() : Environment.GetFolderPath(destination)) + @"/Recordings/ExtractedVideo";

                if (!Directory.Exists(v))
                    Directory.CreateDirectory(v);

                var videoDownloadTask = youtubePlayer.DownloadVideoAsync(v, null, this);
                var captionsDownloadTask = youtubePlayer.DownloadClosedCaptions();

                var filePath = await videoDownloadTask;
                var captionTrack = await captionsDownloadTask;

                if (extractSRT)
                {
                    var srtPath = Path.ChangeExtension(filePath, ".srt");
                    File.WriteAllText(srtPath, captionTrack.ToSRT());
                }

                YoutubePlayer.OnEndDownload.Invoke();
                downloadProgress.fillAmount = 0f;
                Debug.Log($"Video saved to {Path.GetFullPath(filePath)}");
            }
            catch (Exception e)
            {
                YoutubePlayer.OnErrorShown.Invoke(e.Message);
                YoutubePlayer.OnEndDownload.Invoke();
            }
            
        }

        public void Report(double value)
        {
            progress = (float) value;
        }

        private void Update()
        {
            if (downloadProgress)
                downloadProgress.fillAmount = progress;

            if (Input.GetKeyDown(KeyCode.F1))
            {
                var v = (useDataPath ? System.IO.Directory.GetCurrentDirectory() : Environment.GetFolderPath(destination)) + @"/Recordings/ExtractedVideo/";
                if (!Directory.Exists(v))
                    Directory.CreateDirectory(v);

                System.Diagnostics.Process.Start(Path.GetFullPath(v));
            }
        }
    }
}