# qirx_audio
#### AAC Audio output for the DAB+ Decoder of the [QIRX](https://softsyst.com/qirx) SDR (V2+)  
- Independent 64-bit Windows exe, although intended to cooperate with QIRX.   
- Receives raw AAC frames (with ADTS headers) and/or commands from QIRX and creates **PCM16** samples for **NAudio**, as well as status responses returned via UDP.  
- Uses the **UDP** ports: 8766, 8767, 8768, 8769.
- Uses the **libfaad2** library as AAC decoder, which is built in the VisualStudio (VS2015) solution as well (see _External_ subdirectory).  
- **Important Remark:
The cooperation with QIRX works ONLY with the _libfaad.dll_ built with the sources provided here.** The original libfaad2.dll is NOT able to cope with ADTS headers together with 960samples frames, which are produced by DAB+. 
- To work together with QIRX, all binaries must be contained in the **_qirx_audio_** subdirectory of the QIRX runtime. This is already prepared when downloading QIRX.
