cd %1
ffmpeg -i  %2 -vn -ar 44100 -ac 1 -b:a 32k -f mp3 %3