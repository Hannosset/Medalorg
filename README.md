# Medalorg

## Proof Of Concept

The medalorg-POC is the proof of concept to acquire the technology, the experience and information implementing such a solution.

The code is *'dirty'* and the POC hos no other purpose than to experience how it works.

The information is:
1. The best way to extract the audio and video would be using *WebResponse* then access and reading the stream.
2. The maximum buffer transmitted in a *read()* is of maximum 16,384 bytes.
3. When a video is not in high demand, it is stored on the back up servers. It is possible to access the data but as the access is not prioritized by the server we will receive now and then either a *timeout* or an *unexpected EOF*. 
   The solution is to wait a few seconds and to restart the process. I am still puzzled about the *protocol error* message. 
  My take is that the audio or video access request is cancelled by the server as the media is maybe located on a remote backup server and need to be brought back to *life* - all I know is that after a few hours, I can then start to download the media.
4. For each media, we can download in parallel the audio, the video and the subtitles.
5. Some alternative to download using *Google* or some examples from *libvideo* expect you to create a cookie of the form as shown below. I personally do not like and mistrust any cookies
	...
    new Cookie( "CONSENT", "YES+cb", "/", "youtube.com" )
	...

## Engine
### Phase I
**Goal**: Extracting the extrenal logic and make it run.
The engine is generating 2 important files: an audio mp3 file of the best quality available and a mpeg file of the best quality capped to 1080 (config default).
facts:
1. When downloading a *mp3* or a *mpeg* file asynchronous events like closing the application may prevent the file for being totally downloaded. Similarly, after 6 attempts or a protocol error, the download attempts will stop to be resumed later on.
2. Both these files must exist and be correctly downloaded for the *ffmpeg.exe* to process and merge these two creating an *.mp4* file.
3. The existence of a *mp3* or an *mpeg* files does not mean the download is completed and successful.
### Phase II
**Goal**: Improve the core engine and confirming all files are downloaded
1. Once the sequence is processed, some file(s) may not be downloaded, an external loop will reparse each item in the list until all the processed items are completely downloaded.
2. The similar logic will be implemented for the playlist, to the exception that a playlist may not be fully downloaded because some videos have been removed. To prevent infinite loop, the list download engine must receive from the download process confirmation the video is present or not. If the video is not present, the video will simply be return true (in the POC code it is the *static bool Download( string url )*) to indicate that the process is completed. In our GUI application, we will require to morp the *bool* into a more meaningful value allowing *DownloadCompleted*, *PartialDownload* and *NotAvailable*.
### Phase III
**Goal**: Investigate the merging tool and how to use it.
1. In case of error, it appears *ffmpeg.exe* does not return an error code even with the argument *-max_error_rate 0.0*, therefore before launching the merge, we will need to not only verify if the file exists but also if its length corresponds to the content length. In that case if *ffmpeg.exe* returns an error, there is nothing we can do.
2. If a *mpeg* file and an *mp4* exist, then something wrong happened, and we will restart it all deleting all instance of the media.
3. **Possible solution**: once the mpeg file is completely downloaded, a file with the name of the video and the extension *.nfo* will contain the resolution and file length of the *mpeg*, the bitrate audio and file length of the *mp3*, the hash of the audio. Before downloading, the logic will first check if the *nfo* file exist, the engine will confirm the length of the mpeg and the length of the audio with both the file and the data retrieved.
4. **Parallel processing constraint**: In order to parallelize efficiently, we will add in a list all video and audio we want to download, then create threads and download in each threads the audio and or video. the thread of the video will wait for the audio to be fully downloaded before initiating the merge calling *ffmpeg.exe*
### Phase IV
**Goal**: Integrate the logic of possible multiple async requests and prepare for a threaded object oriented solution.
1. The best alternative is to store the data in a *MemoryStream* object and flush the object only if fully downloaded. Therefore upon a fully donwloaded media, we will have an *.aac/*.ogg/*.opus* and an *.mpeg* file completed (the preferred audio and video format is confiogurable - default *.ogg* and *.mp4* - Note that the video file will always have the extension *.mpeg*) - the merge will generate an *.mp4 or .webm* file and the *.mpeg* file will be deleted. 
2. It appears that due to the activity on the you tube server we are connecting, we may receive a protocol error - camcal the operation thet will be resumed later on.
3. When receiving an EOF, timeout or pending, we simply increase the web timeout and *sleep()*

## GUI application
### Configuration

