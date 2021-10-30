# qirx_audio
#### AAC Audio output for the DAB+ Decoder of the [QIRX](https://softsyst.com/qirx) SDR (V2+)  
- Independent 64-bit Windows exe, although intended to cooperate with QIRX.
- Written in C# (.NET framework 4.5.2), cooperating with libfaad via P/Invoke.
- Receives raw AAC frames (with ADTS headers) and/or commands from QIRX and creates **PCM16** samples for **NAudio**, as well as status responses returned via UDP.  
- Uses the **UDP** ports: 8766, 8767, 8768, 8769.
- Uses the **libfaad2** library as AAC decoder, which is built in the VisualStudio (VS2015) solution as well (see _External_ subdirectory).  
- **Important Remark:
The cooperation with QIRX works ONLY with the _libfaad.dll_ built with the sources provided here.** The original libfaad2.dll is NOT able to cope with ADTS headers together with 960samples frames, which are produced by DAB+. 
- To work together with QIRX, all binaries must be contained in the **_qirx_audio_** subdirectory of the QIRX runtime. This is already prepared when downloading QIRX.
#### V2.0 (November 2021) 
- net5 Version
- Now features PortAudio as its audio engine.
- With the #define LINUX, it works also on Linux (tested on Debian 11.1). Identical codebase.
#### Linux Build for Debian 11 in VS2019 on Windows
- In VS, search for //#define LINUX and un-comment it.
- Do a "Rebuild All",
- On a Windows Command Line, run the script "build_qirxAudioLinux.bat" (without the quotation marks),
- This will produce the executable file **qirx_audio** (sic, without extension)
- In Debian, copy it to your directory of choice.
- Make sure that in the Linux search path for shared libraries the file **libportaudio.so.2** is present.
- Run ./qirx_audio

Please note that the **installation of net5 on Debian is NOT necessary**, as the qirx_audio is self-contained (the reason for its size).
#### Credit 
PortAudio P/Invoke interfacing based on work by Atsushi Eno, https://github.com/atsushieno/portaudio-sharp
