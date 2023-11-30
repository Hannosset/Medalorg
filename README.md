# Medalorg
To rebuild the Medalog-POC, you must initalize the context.

- Install the Video Library dll
    Sources available on GitHub https://github.com/omansak/libvideo
	nuget console, run the following commands
	PM> Install-Package VideoLibrary

- The mpeg merge executable is available from the following website
	https://ffmpeg.org/download.html#build-windows
	and from the Github project
	https://github.com/BtbN/FFmpeg-Builds/releases

- The subtitles or caption functionality implementation are originated from
	the code was originally written in python
	https://github.com/jdepoix/youtube-transcript-api/blob/master/youtube_transcript_api/_api.py
	the python code was rewritten in c#
	https://github.com/BobLd/youtube-transcript-api-sharp
	As no dll are provided, we need then to access the source code and integrate it to our project.
	nuget console, run the following commands
	PM> Install-Package Google.Apis.YouTube.v3
- 