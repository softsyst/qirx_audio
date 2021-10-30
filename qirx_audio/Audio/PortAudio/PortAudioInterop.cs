#define LINUX

using System;
using System.Runtime.InteropServices;

#region Typedefs
using PaError			= Commons.Media.PortAudio.PaErrorCode;
// typedef int 	PaError
using PaDeviceIndex =  System.Int32;
// typedef int 	PaDeviceIndex
using PaHostApiIndex		= System.Int32;
// typedef int 	PaHostApiIndex
using PaTime			= System.Double;
// typedef double 	PaTime
//using PaSampleFormat		= System.UInt64;	// typedef unsigned long 	PaSampleFormat
//using PaStream			= System.Void;		// typedef void 	PaStream
using PaStreamFlags		= System.UInt32;
// typedef unsigned long 	PaStreamFlags
#endregion

namespace Commons.Media.PortAudio
{
	#region Typedefs -> delegates
	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	public delegate PaStreamCallbackResult/*int*/ PaStreamCallback (/*const*/ IntPtr/*void **/input,IntPtr/**void **/output, [MarshalAs (UnmanagedType.SysUInt)] /*unsigned long*/IntPtr frameCount,/*const*/ IntPtr/*PaStreamCallbackTimeInfo **/timeInfo, [MarshalAs (UnmanagedType.SysUInt)] UIntPtr/*PaStreamCallbackFlags*/ statusFlags,IntPtr/*void **/userData);

	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	public delegate void PaStreamFinishedCallback (IntPtr/*void **/userData);
	#endregion

#if LINUX
	unsafe public static class PortAudioInterop
    {
        static PortAudioInterop()
        {
            Pa_Initialize();
            AppDomain.CurrentDomain.DomainUnload += (o, args) => Pa_Terminate();
        }

	#region Functions
        [DllImport("libportaudio.so.2")]
        public static extern
        int Pa_GetVersion(/*void*/)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        /*const*/ IntPtr/*char * */ 	Pa_GetVersionText(/*void*/)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        /*const*/ IntPtr/*char * */ 	Pa_GetErrorText(PaError errorCode)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        PaError Pa_Initialize(/*void*/)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        PaError Pa_Terminate(/*void*/)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        PaHostApiIndex Pa_GetHostApiCount(/*void*/)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        PaHostApiIndex Pa_GetDefaultHostApi(/*void*/)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        /*const*/ IntPtr/*PaHostApiInfo * */ 	Pa_GetHostApiInfo(PaHostApiIndex hostApi)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        PaHostApiIndex Pa_HostApiTypeIdToHostApiIndex(PaHostApiTypeId type)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        PaDeviceIndex Pa_HostApiDeviceIndexToDeviceIndex(PaHostApiIndex hostApi, int hostApiDeviceIndex)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        /*const*/ IntPtr/*PaHostErrorInfo * */ 	Pa_GetLastHostErrorInfo(/*void*/)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        PaDeviceIndex Pa_GetDeviceCount(/*void*/)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        PaDeviceIndex Pa_GetDefaultInputDevice(/*void*/)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        PaDeviceIndex Pa_GetDefaultOutputDevice(/*void*/)
            ;
        [DllImport("libportaudio.so.2")]
        public static extern
        /*const*/ IntPtr/*PaDeviceInfo * */ 	Pa_GetDeviceInfo(PaDeviceIndex device)
            ;

        [DllImport("libportaudio.so.2")]
        public static extern
        PaError Pa_IsFormatSupported(/*const*/IntPtr/*PaStreamParameters * */inputParameters, /*const*/
        int*/*PaStreamParameters * */outputParameters, double sampleRate);

		[DllImport ("libportaudio.so.2")]
		public static extern
		PaError 	Pa_OpenStream (out IntPtr/*PaStream ** */stream,
                                   /*const*/IntPtr/*PaStreamParameters * */inputParameters, 
                                   /*const*/IntPtr/*PaStreamParameters * */outputParameters, 
                                   double sampleRate, 
                                   [MarshalAs (UnmanagedType.SysUInt)] /*unsigned long*/uint framesPerBuffer, 
                                   [MarshalAs (UnmanagedType.SysUInt)] PaStreamFlags streamFlags, 
                                   PaStreamCallback streamCallback, IntPtr/*void **/userData)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		PaError 	Pa_OpenDefaultStream (out IntPtr/*PaStream ** */stream, 
                                          int numInputChannels, 
                                          int numOutputChannels, 
                                          [MarshalAs (UnmanagedType.SysUInt)] IntPtr/*PaSampleFormat*/ sampleFormat, 
                                          double sampleRate, 
                                          [MarshalAs (UnmanagedType.SysUInt)] /*unsigned long*/IntPtr framesPerBuffer, PaStreamCallback streamCallback, IntPtr/*void **/userData)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		PaError 	Pa_CloseStream (IntPtr/*PaStream * */stream)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		PaError 	Pa_SetStreamFinishedCallback (IntPtr/*PaStream * */stream, 
                                                  PaStreamFinishedCallback streamFinishedCallback)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		PaError 	Pa_StartStream (IntPtr/*PaStream * */stream)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		PaError 	Pa_StopStream (IntPtr/*PaStream * */stream)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		PaError 	Pa_AbortStream (IntPtr/*PaStream * */stream)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		PaError 	Pa_IsStreamStopped (IntPtr/*PaStream * */stream)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		PaError 	Pa_IsStreamActive (IntPtr/*PaStream * */stream)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		/*const*/ IntPtr/*PaStreamInfo * */	Pa_GetStreamInfo (IntPtr/*PaStream * */stream)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		PaTime 	Pa_GetStreamTime (IntPtr/*PaStream * */stream)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		double 	Pa_GetStreamCpuLoad (IntPtr/*PaStream * */stream)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		PaError 	Pa_ReadStream (IntPtr/*PaStream * */stream, IntPtr/*void **/buffer, 
                                   [MarshalAs (UnmanagedType.SysUInt)] /*unsigned long*/uint frames)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		PaError Pa_WriteStream(IntPtr stream, byte* buffer, uint frames)
			;
		//PaError Pa_WriteStream(IntPtr/*PaStream * */stream, /*const*/IntPtr/*void **/buffer,
		//							[MarshalAs(UnmanagedType.SysUInt)] /*unsigned long*/uint frames)
		//	;

		[DllImport ("libportaudio.so.2")]
		[return: MarshalAs (UnmanagedType.SysInt)]
		public static extern
		/*signed long*/int 	Pa_GetStreamReadAvailable (IntPtr/*PaStream * */stream)
			;

		[DllImport ("libportaudio.so.2")]
		[return: MarshalAs (UnmanagedType.SysUInt)]
		public static extern
		/*signed long*/int 	Pa_GetStreamWriteAvailable (IntPtr/*PaStream * */stream)
			;

		// Return value is originally defined as PaError but this should rather make sense.
		[DllImport ("libportaudio.so.2")]
		public static extern
		int 	Pa_GetSampleSize ([MarshalAs (UnmanagedType.SysUInt)] PaSampleFormat format)
			;

		[DllImport ("libportaudio.so.2")]
		public static extern
		void 	Pa_Sleep ([MarshalAs (UnmanagedType.SysInt)] /*long*/int msec)
			;
 
	#endregion
    }
#else
	unsafe public static class PortAudioInterop
	{
		static PortAudioInterop()
		{
			Pa_Initialize();
			AppDomain.CurrentDomain.DomainUnload += (o, args) => Pa_Terminate();
		}

		#region Functions
		[DllImport("libportaudio64bit.dll")]
		public static extern
		int Pa_GetVersion(/*void*/)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		/*const*/ IntPtr/*char * */ 	Pa_GetVersionText(/*void*/)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		/*const*/ IntPtr/*char * */ 	Pa_GetErrorText(PaError errorCode)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaError Pa_Initialize(/*void*/)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaError Pa_Terminate(/*void*/)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaHostApiIndex Pa_GetHostApiCount(/*void*/)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaHostApiIndex Pa_GetDefaultHostApi(/*void*/)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		/*const*/ IntPtr/*PaHostApiInfo * */ 	Pa_GetHostApiInfo(PaHostApiIndex hostApi)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaHostApiIndex Pa_HostApiTypeIdToHostApiIndex(PaHostApiTypeId type)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaDeviceIndex Pa_HostApiDeviceIndexToDeviceIndex(PaHostApiIndex hostApi, int hostApiDeviceIndex)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		/*const*/ IntPtr/*PaHostErrorInfo * */ 	Pa_GetLastHostErrorInfo(/*void*/)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaDeviceIndex Pa_GetDeviceCount(/*void*/)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaDeviceIndex Pa_GetDefaultInputDevice(/*void*/)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaDeviceIndex Pa_GetDefaultOutputDevice(/*void*/)
			;
		[DllImport("libportaudio64bit.dll")]
		public static extern
		/*const*/ IntPtr/*PaDeviceInfo * */ 	Pa_GetDeviceInfo(PaDeviceIndex device)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaError Pa_IsFormatSupported(/*const*/IntPtr/*PaStreamParameters * */inputParameters, /*const*/
		int*/*PaStreamParameters * */outputParameters, double sampleRate);
		//IntPtr/*PaStreamParameters * */outputParameters, double sampleRate);

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaError Pa_OpenStream(out IntPtr/*PaStream ** */stream,
								   /*const*/IntPtr/*PaStreamParameters * */inputParameters,
								   /*const*/IntPtr/*PaStreamParameters * */outputParameters,
								   double sampleRate,
								   [MarshalAs(UnmanagedType.SysUInt)] /*unsigned long*/uint framesPerBuffer,
								   [MarshalAs(UnmanagedType.SysUInt)] PaStreamFlags streamFlags,
								   PaStreamCallback streamCallback, IntPtr/*void **/userData)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaError Pa_OpenDefaultStream(out IntPtr/*PaStream ** */stream,
										  int numInputChannels,
										  int numOutputChannels,
										  [MarshalAs(UnmanagedType.SysUInt)] IntPtr/*PaSampleFormat*/ sampleFormat,
										  double sampleRate,
										  [MarshalAs(UnmanagedType.SysUInt)] /*unsigned long*/IntPtr framesPerBuffer, PaStreamCallback streamCallback, IntPtr/*void **/userData)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaError Pa_CloseStream(IntPtr/*PaStream * */stream)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaError Pa_SetStreamFinishedCallback(IntPtr/*PaStream * */stream,
												  PaStreamFinishedCallback streamFinishedCallback)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaError Pa_StartStream(IntPtr/*PaStream * */stream)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaError Pa_StopStream(IntPtr/*PaStream * */stream)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaError Pa_AbortStream(IntPtr/*PaStream * */stream)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaError Pa_IsStreamStopped(IntPtr/*PaStream * */stream)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaError Pa_IsStreamActive(IntPtr/*PaStream * */stream)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		/*const*/ IntPtr/*PaStreamInfo * */	Pa_GetStreamInfo(IntPtr/*PaStream * */stream)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaTime Pa_GetStreamTime(IntPtr/*PaStream * */stream)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		double Pa_GetStreamCpuLoad(IntPtr/*PaStream * */stream)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		PaError Pa_ReadStream(IntPtr/*PaStream * */stream, IntPtr/*void **/buffer,
								   [MarshalAs(UnmanagedType.SysUInt)] /*unsigned long*/uint frames)
			;

		[DllImport("libportaudio64bit.dll")]
		unsafe public static extern
		PaError Pa_WriteStream(IntPtr stream, byte* buffer, uint frames)
			;
		//PaError Pa_WriteStream(IntPtr/*PaStream * */stream, /*const*/IntPtr/*void **/buffer,
		//							[MarshalAs(UnmanagedType.SysUInt)] /*unsigned long*/uint frames)
		//	;

		[DllImport("libportaudio64bit.dll")]
		[return: MarshalAs(UnmanagedType.SysInt)]
		public static extern
		/*signed long*/int Pa_GetStreamReadAvailable(IntPtr/*PaStream * */stream)
			;

		[DllImport("libportaudio64bit.dll")]
		[return: MarshalAs(UnmanagedType.SysUInt)]
		public static extern
		/*signed long*/int Pa_GetStreamWriteAvailable(IntPtr/*PaStream * */stream)
			;

		// Return value is originally defined as PaError but this should rather make sense.
		[DllImport("libportaudio64bit.dll")]
		public static extern
		int Pa_GetSampleSize([MarshalAs(UnmanagedType.SysUInt)] PaSampleFormat format)
			;

		[DllImport("libportaudio64bit.dll")]
		public static extern
		void Pa_Sleep([MarshalAs(UnmanagedType.SysInt)] /*long*/int msec)
			;

		#endregion
	}
#endif
#if !USE_CXXI

	public static class Factory
	{
		// NOT WORKING Compiler complaint //
		unsafe public static CppInstancePtr ToNative<T> (T value)
		{
			IntPtr ret = Marshal.AllocHGlobal (Marshal.SizeOf (value));
			//IntPtr ret = Marshal.AllocHGlobal (Marshal.SizeOf<T>());
			Marshal.StructureToPtr (value, ret, false);
			return CppInstancePtr.Create<T> (ret);
		}
		
		public static T Create<T> (CppInstancePtr handle)
		{
			return (T)Marshal.PtrToStructure (handle.Native, typeof(T));
		}
		
	}

	[StructLayout (LayoutKind.Sequential)]
	public struct PaHostApiInfo
	{
		public int 	structVersion;
		public PaHostApiTypeId 	type;
		[MarshalAs (UnmanagedType.LPStr)]
		public string
			name;
		public int 	deviceCount;
		public PaDeviceIndex 	defaultInputDevice;
		public PaDeviceIndex 	defaultOutputDevice;
	}
	
	[StructLayout (LayoutKind.Sequential)]
	public struct PaHostErrorInfo
	{
		public PaHostApiTypeId 	hostApiType;
		[MarshalAs (UnmanagedType.SysInt)]
		public /*long*/int 	errorCode;
		[MarshalAs (UnmanagedType.LPStr)]
		public string
			errorText;
	}

	[StructLayout (LayoutKind.Sequential)]
	public struct PaDeviceInfo
	{
		public int 	structVersion;
		[MarshalAs (UnmanagedType.LPStr)]
		public string
			name;
		public PaHostApiIndex 	hostApi;
		public int 	maxInputChannels;
		public int 	maxOutputChannels;
		public PaTime 	defaultLowInputLatency;
		public PaTime 	defaultLowOutputLatency;
		public PaTime 	defaultHighInputLatency;
		public PaTime 	defaultHighOutputLatency;
		public double 	defaultSampleRate;
	}

    //[StructLayout (LayoutKind.Sequential)]
    //public struct PaStreamParameters
    //{
    //	public UInt32 	device;
    //	public int 	channelCount;
    //	//[MarshalAs (UnmanagedType.SysUInt)]
    //	public UInt32 	sampleFormat;
    //	public double 	suggestedLatency;
    //	public IntPtr 	hostApiSpecificStreamInfo;
    //}

    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct PaStreamParameters
    {
        [MarshalAs(UnmanagedType.SysUInt)]
        public PaDeviceIndex device;
        [MarshalAs(UnmanagedType.SysUInt)]
        public int channelCount;
        [MarshalAs(UnmanagedType.SysUInt)]
        public PaSampleFormat sampleFormat;
        [MarshalAs(UnmanagedType.R8)]
        public PaTime suggestedLatency;
        public IntPtr hostApiSpecificStreamInfo;
    }

    [StructLayout (LayoutKind.Sequential)]
	public struct PaStreamCallbackTimeInfo
	{
		public PaTime 	inputBufferAdcTime;
		public PaTime 	currentTime;
		public PaTime 	outputBufferDacTime;
	}

	[StructLayout (LayoutKind.Sequential)]
	public struct PaStreamInfo
	{
		public int 	structVersion;
		public PaTime 	inputLatency;
		public PaTime 	outputLatency;
		public double 	sampleRate;
	}
	
	public struct CppInstancePtr : IDisposable
	{
		IntPtr ptr;
		bool delete;
		Type type;
		
		public CppInstancePtr (IntPtr ptr)
		{
			this.ptr = ptr;
			delete = false;
			type = null;
		}
		
		public static CppInstancePtr Create<T> (IntPtr ptr)
		{
			return new CppInstancePtr (ptr, typeof(T));
		}
		
		CppInstancePtr (IntPtr ptr, Type type)
		{
			this.ptr = ptr;
			this.delete = true;
			this.type = type;
		}

		public void Dispose ()
		{
			if (delete)
				Marshal.DestroyStructure (ptr, type);
		}
		
		public IntPtr Native {
			get { return ptr; }
		}
	}
#endif

}



