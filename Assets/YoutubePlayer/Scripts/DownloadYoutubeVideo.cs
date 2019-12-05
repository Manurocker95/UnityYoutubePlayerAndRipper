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
            YoutubePlayer.OnDownloading.Invoke();

            Debug.Log("Downloading, please wait...");

            var v = Environment.GetFolderPath(destination);

#if UNITY_EDITOR_WIN
            
            //v = OnOpen(StandaloneFileBrowser.OpenFilePanel(Title, Directory, Extension, Multiselect));
#endif

            var videoDownloadTask = youtubePlayer.DownloadVideoAsync(v, null, this);
            var captionsDownloadTask = youtubePlayer.DownloadClosedCaptions();

            var filePath = await videoDownloadTask;
            var captionTrack = await captionsDownloadTask;
            
            var srtPath = Path.ChangeExtension(filePath, ".srt");
            File.WriteAllText(srtPath, captionTrack.ToSRT());

            YoutubePlayer.OnEndDownload.Invoke();

            Debug.Log($"Video saved to {Path.GetFullPath(filePath)}");
        }

        public void Report(double value)
        {
            progress = (float) value;
        }

        private void Update()
        {
            if (downloadProgress)
                downloadProgress.fillAmount = progress;
        }
    }
}