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
using YoutubeDownloader;
using System.Threading;
using MediaToolkit.Model;
using MediaToolkit;

namespace YoutubePlayer
{
    public class YoutubeAudioDownload : MonoBehaviour, IProgress<double>
    {
        public YoutubePlayer youtubePlayer;
        public Environment.SpecialFolder destination;
        private Image downloadProgress;
        private float progress;
        public bool useDataPath = true;
        public bool tryMultipleMethods = false;
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

        public void DownloadAudioAsync()
        {
            if (string.IsNullOrEmpty(youtubePlayer.youtubeUrl))
            {
                Debug.LogError("YOTUUBE URL NULL");
            }

            var v = (useDataPath ? System.IO.Directory.GetCurrentDirectory() : Environment.GetFolderPath(destination)) + @"/Recordings/";

            if (Directory.Exists(v))
                Directory.CreateDirectory(v);

            var link1 = new LinkInfo(youtubePlayer.youtubeUrl);
  

            var downloader = new YTDownloaderBuilder()
                            .SetExportAudioPath(v) // mandatory
                            .SetExportVideoPath(v) // mandatory
                            .SetExportOptions(ExportOptions.ExportAudio) 
                            .SetSkipDownloadIfFilesExists(false) // default setting
                            .SetLinks(link1) // check other overloads
                            .Build();

            Task<DownloadResult[]> results = downloader.DownloadLinksAsync(CancellationToken.None); // process download

            downloader.AddDownloadProgressChangedAction(link1.GUID, (evArgs) =>
            {
                Report(evArgs.ProgressPercentage * 0.01);
            });
            downloader.AddDownloadStartedAction(link1.GUID, (evArgs) => 
            {
               Debug.Log ("DOWNLOAD AUDIO STARTED");
                YoutubePlayer.OnDownloading.Invoke();
            });
            downloader.AddDownloadFinishedAction(link1.GUID, (evArgs) => 
            {
                Debug.Log("DOWNLOAD FINISHED");
    
            });

            downloader.AddAudioConvertingEndedAction(link1.GUID, (convertArgs) =>
            {
                YoutubePlayer.OnEndDownload.Invoke();
                //Console.WriteLine("Converting audio done !");
            });

        }

        public void DownloadAudioSync()
        {
            if (string.IsNullOrEmpty(youtubePlayer.youtubeUrl))
            {
                Debug.LogError("YOTUUBE URL NULL");
            }

            var v = (useDataPath ? System.IO.Directory.GetCurrentDirectory() : Environment.GetFolderPath(destination)) + @"/Recordings/";

            if (Directory.Exists(v))
                Directory.CreateDirectory(v);

            var link1 = new LinkInfo(youtubePlayer.youtubeUrl);

            var downloader = new YTDownloaderBuilder()
                            .SetExportAudioPath(v) // mandatory
                            .SetExportVideoPath(v) // mandatory
                            .SetExportOptions(ExportOptions.ExportAudio) // default setting
                            .SetSkipDownloadIfFilesExists(false) // default setting
                            .SetLinks(link1) // check other overloads
                            .Build();

            YoutubePlayer.OnDownloading.Invoke();

            downloader.AddDownloadProgressChangedAction(link1.GUID, (evArgs) =>
            {
                Report(evArgs.ProgressPercentage * 0.01);
            });
            downloader.AddDownloadStartedAction(link1.GUID, (evArgs) =>
            {
                Debug.Log("DOWNLOAD AUDIO STARTED");

            });
            downloader.AddDownloadFinishedAction(link1.GUID, (evArgs) =>
            {
                Debug.Log("DOWNLOAD FINISHED");

            });

            downloader.AddAudioConvertingEndedAction(link1.GUID, (convertArgs) =>
            {
                YoutubePlayer.OnEndDownload.Invoke();
                //Console.WriteLine("Converting audio done !");
            });

            DownloadResult[] results = downloader.DownloadLinks(); // process download

        }

        public async void DownloadAudio()
        {
            if (string.IsNullOrEmpty(youtubePlayer.youtubeUrl))
            {
                Debug.LogError("YOTUUBE URL NULL");
                return;
            }

            if (!tryMultipleMethods)
            {
                DownloadVideoAndExtract();
                return;
            }

            var v = (useDataPath ? System.IO.Directory.GetCurrentDirectory() : Environment.GetFolderPath(destination)) + @"\Recordings\";

            if (Directory.Exists(v))
                Directory.CreateDirectory(v);

            var link1 = new LinkInfo(youtubePlayer.youtubeUrl);

            YoutubePlayer.OnDownloading.Invoke();

            var downloader = new YTDownloaderBuilder()
                            .SetExportAudioPath(v) // mandatory
                            .SetExportVideoPath(v) // mandatory
                            .SetExportOptions(ExportOptions.ExportAudio | ExportOptions.ExportVideo)
                            .SetSkipDownloadIfFilesExists(false) // default setting
                            .SetLinks(link1) // check other overloads
                            .Build();

            downloader.AddDownloadProgressChangedAction(link1.GUID, (evArgs) =>
            {
                Report(evArgs.ProgressPercentage * 0.01);
            });
            downloader.AddDownloadStartedAction(link1.GUID, (evArgs) =>
            {
                Debug.Log("DOWNLOAD AUDIO STARTED");
        
            });
            downloader.AddDownloadFinishedAction(link1.GUID, (evArgs) =>
            {
                Debug.Log("DOWNLOAD FINISHED");
                YoutubePlayer.OnEndDownload.Invoke();
            });
            try
            {
                DownloadResult[] results = await downloader.DownloadLinksAsync(CancellationToken.None); // process download
                foreach (var res in results)
                {
                    Console.WriteLine(res.AudioSavedFilePath);
                    Console.WriteLine(res.VideoSavedFilePath);
                    Console.WriteLine(res.FileBaseName);
                    Console.WriteLine(res.GUID);
                    Console.WriteLine(res.DownloadSkipped);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Couldn't extract audio because "+e);
                
                YoutubePlayer.OnEndDownload.Invoke();

                Debug.LogError("Trying other method... ");
                NewTest();
                //OldDownload();
            }
        }

        void NewTest()
        {
            if (string.IsNullOrEmpty(youtubePlayer.youtubeUrl))
            {
                Debug.LogError("YOTUUBE URL NULL");
                return;
            }

            var v = (useDataPath ? System.IO.Directory.GetCurrentDirectory() : Environment.GetFolderPath(destination)) + @"\Recordings\";

            if (Directory.Exists(v))
                Directory.CreateDirectory(v);

            var source = v;
            var youtube = YouTube.Default;
            try
            {
                var vid = youtube.GetVideo(youtubePlayer.youtubeUrl);
                var filename =  "_temp_" + vid.FullName;
                File.WriteAllBytes(source + "_temp_" + vid.FullName, vid.GetBytes());

                var inputFile = new MediaFile { Filename = filename };
                var outputFile = new MediaFile { Filename = $"{filename}.mp3" };

                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);
                    engine.Convert(inputFile, outputFile);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Trying old method... ");
               
                OldDownload();
            }
        }
        async void OldDownload()
        {
            YoutubePlayer.OnDownloading.Invoke();
            var youtube = YouTube.Default;
       
            try
            {
                var video = await youtube.GetVideoAsync(youtubePlayer.youtubeUrl);
                var bytes = await video.GetBytesAsync();
                var v = (useDataPath ? System.IO.Directory.GetCurrentDirectory() : Environment.GetFolderPath(destination)) + @"/Recordings/";

                if (Directory.Exists(v))
                    Directory.CreateDirectory(v);

                var filePath = v + @"\" + video.FullName + ".mp3";
                File.WriteAllBytes(filePath, bytes);

                YoutubePlayer.OnEndDownload.Invoke();

                Debug.Log($"Audio saved to {Path.GetFullPath(filePath)}");
            }
            catch (Exception e)
            {
                Debug.Log($"Trying last attempt....");
                YoutubePlayer.OnEndDownload.Invoke();
                DownloadLastAttempt();
            }
        }


        public async void DownloadLastAttempt()
        {
            YoutubePlayer.OnDownloading.Invoke();

            try
            {
                var source = (useDataPath ? System.IO.Directory.GetCurrentDirectory() : Environment.GetFolderPath(destination)) + @"/Recordings/";
                var youtube = YouTube.Default;
                var vid = await youtube.GetVideoAsync(youtubePlayer.youtubeUrl);
                File.WriteAllBytes(source + vid.FullName, vid.GetBytes());

                var inputFile = new MediaFile { Filename = source + vid.FullName };
                var outputFile = new MediaFile { Filename = source + $"{vid.FullName.Replace(".mp4", "")}.mp3" };

                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);

                    engine.Convert(inputFile, outputFile);
                }
                Debug.Log($" Downloaded audio at {source}/{vid.FullName.Replace(".mp4", "")}.mp3");
                YoutubePlayer.OnEndDownload.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"Trying to download the video "+e);
                YoutubePlayer.OnEndDownload.Invoke();
                DownloadVideoAndExtract();
            }
        }

        public async void DownloadVideoAndExtract()
        {
            YoutubePlayer.OnDownloading.Invoke();

            Debug.Log("Downloading, please wait...");

            var v = (useDataPath ? System.IO.Directory.GetCurrentDirectory() : Environment.GetFolderPath(destination)) + @"/Recordings/DownloadAndExtract/";

            if (Directory.Exists(v))
                Directory.CreateDirectory(v);

            try
            {
                var filePath = await youtubePlayer.DownloadVideoAsync(v, null, this);

                Debug.Log("Extracting audio...");

                string filename = Path.GetFileNameWithoutExtension(filePath);
                //System.Diagnostics.Process.Start(Application.dataPath + "/convert.bat", $@"""{Application.dataPath}/MediaToolkit"" ""{v}{filename}.mp4"" ""{v}{filename}.mp3""");
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = Application.dataPath + "/convert.bat";
                startInfo.Arguments = $@"""{Application.dataPath}/MediaToolkit"" ""{v}{filename}.mp4"" ""{v}{filename}.mp3""";
                p.StartInfo = startInfo;
                p.Start();

                p.WaitForExit();
                if (File.Exists(filePath))
                {
                    Debug.Log("Deleting video file...");
                    File.Delete(filePath);
                }
               
                YoutubePlayer.OnEndDownload.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"Couldn't save Audio save audio because " + e);
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