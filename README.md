# qirx_audio
#### AAC Audio output for the DAB+ Decoder of the [QIRX](https://softsyst.com/qirx) SDR (V2+)  
- Independent 64-bit Windows exe, although intended to cooperate with QIRX.
- Written in C# (.NET framework 4.5.2, from V2 in .net5 ), cooperating with libfaad via P/Invoke.
- Receives raw AAC frames (with ASC headers) and/or commands from QIRX and creates **PCM16** samples for **PortAudio**, as well as status responses returned via UDP.  
- Uses the **UDP** ports: 8766, 8767, 8768, 8769.
- Uses the **libfaad2** library as AAC decoder.  
- To work together with QIRX, all binaries must be contained in the **_qirx_audio_** subdirectory of the QIRX runtime. When using the QIRX isntaller for Windows, this is already prepared.
#### V2.0 (November 2021) 
- net5 Version
- Change from ADTS to ASC headers.
- Now features PortAudio as its audio engine.
- With the #define LINUX, it works also on Linux (tested on Debian 11.1). Identical codebase.
#### V2.1 (November 2021) 
- Program structure reorganized, due to problems in Linux on slower machines.
#### Linux Build for Debian 11 in VS2019 on Windows
- In VisualStudio, search for //#define LINUX and un-comment it.
- Do a "Rebuild All",
- On a Windows Command Line, run the script "build_qirxAudioLinux.bat" (without the quotation marks),
- This will produce the executable file **qirx_audio** (sic, without extension)
- In Debian, copy it to your directory of choice.
- Make sure that in the Linux search path for shared libraries the following libraries are present:
  - libportaudio.so.2
  - libfaad.so
- Run ./qirx_audio

Please note that the **installation of net5 on Debian is NOT necessary**, as the qirx_audio is self-contained (the reason for its size).
#### Credit 
PortAudio P/Invoke interfacing based on work by Atsushi Eno, https://github.com/atsushieno/portaudio-sharp
