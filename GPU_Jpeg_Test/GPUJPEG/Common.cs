using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static GPU_Jpeg_Test.GPUJPEG.Type;
using SamplingFactor = System.UInt32;

namespace GPU_Jpeg_Test.GPUJPEG
{
    public static class Common
    {
        // Constant definition
        public const int MaxDeviceCount = 10;

        /// <summary>
        /// CUDA stream instance type. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CudaStream
        {
            private IntPtr handle;
        }

        /// <summary>
        /// handle to cuda stream instance type
        /// </summary>
        public class CudaStreamHandle : SafeHandle
        {
            public CudaStreamHandle() : base(IntPtr.Zero, true) { }

            public CudaStreamHandle(IntPtr handle) : base(handle, true) { }

            public override bool IsInvalid => handle == IntPtr.Zero;

            protected override bool ReleaseHandle()
            {
                // Implement the release logic if necessary
                return true;
            }
        }

        // Function declarations
        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl,EntryPoint = "gpujpeg_version")]
        public static extern int GetVersion();

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_version_to_string")]
        public static extern IntPtr GetVersionString(int version);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_get_time")]
        public static extern double GetTime();

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DeviceInfo
        {
            /// Device id
            public int Id;

            /// Device name
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Name;

            /// Compute capability major version
            public int ComputeCapabilityMajor;

            /// Compute capability minor version
            public int ComputeCapabilityMinor;

            /// Amount of global memory
            public UIntPtr GlobalMemory;

            /// Amount of constant memory
            public UIntPtr ConstantMemory;

            /// Amount of shared memory
            public UIntPtr SharedMemory;

            /// Number of registers per block
            public int RegisterCount;

            /// Number of multiprocessors
            public int MultiprocessorCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DevicesInfo
        {
            /// Number of devices
            public int DeviceCount;

            /// Device info for each
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxDeviceCount)]
            public DeviceInfo[] Devices;
        }

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_get_devices_info")]
        public static extern DevicesInfo GetDevicesInfo();

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_print_devices_info")]
        public static extern int PrintDevicesInfo();

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_init_device")]
        public static extern int InitDevice(int deviceId, int flags);

        public enum RestartInterval
        {
            Auto = -1,  ///< auto-select the best restart interval
            None = 0,   ///< disabled; CPU Huffman encoder will be used
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Parameters
        {
            /// Verbosity level - show more information, collects duration of each phase, etc.
            /// 0 - normal, 1 - verbose, 2 - debug, 3 - debug2 (if not defined NDEBUG)
            public int Verbose;

            /// Record performance stats, set to 1 to allow gpujpeg_encoder_get_stats()
            public int PerfStats;

            /// Encoder quality level (0-100)
            public int Quality;

            /// Restart interval (0 means that restart interval is disabled and CPU Huffman coder is used)
            public int RestartInterval;

            /// Flag which determines if interleaved format of JPEG stream should be used
            public int Interleaved;

            /// Use segment info in stream for fast decoding
            public int SegmentInfo;

            /// JPEG image component count; count of valid sampling_factor elements
            public int ComponentCount;

            /// Sampling factors for each color component inside JPEG stream
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = GPUJPEG_MAX_COMPONENT_COUNT)]
            public ComponentSamplingFactor[] SamplingFactor;

            /// Color space used inside JPEG stream
            public ColorSpace ColorSpaceInternal;
        }

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_set_default_parameters")]
        public static extern void SetDefaultParameters(ref Parameters param);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_default_parameters")]
        public static extern Parameters DefaultParameters();

        // Subsampling definitions
        public const uint GPUJPEG_SUBSAMPLING_UNKNOWN = 0U;
        public static readonly uint GPUJPEG_SUBSAMPLING_4444 = MK_SUBSAMPLING(1, 1, 1, 1, 1, 1, 1, 1);
        public static readonly uint GPUJPEG_SUBSAMPLING_444 = MK_SUBSAMPLING(1, 1, 1, 1, 1, 1, 0, 0);
        public static readonly uint GPUJPEG_SUBSAMPLING_440 = MK_SUBSAMPLING(1, 2, 1, 1, 1, 1, 0, 0);
        public static readonly uint GPUJPEG_SUBSAMPLING_422 = MK_SUBSAMPLING(2, 1, 1, 1, 1, 1, 0, 0);
        public static readonly uint GPUJPEG_SUBSAMPLING_420 = MK_SUBSAMPLING(2, 2, 1, 1, 1, 1, 0, 0);
        public static readonly uint GPUJPEG_SUBSAMPLING_411 = MK_SUBSAMPLING(4, 1, 1, 1, 1, 1, 0, 0);
        public static readonly uint GPUJPEG_SUBSAMPLING_410 = MK_SUBSAMPLING(4, 2, 1, 1, 1, 1, 0, 0);
        public static readonly uint GPUJPEG_SUBSAMPLING_400 = MK_SUBSAMPLING(1, 1, 0, 0, 0, 0, 0, 0);
        public static readonly uint GPUJPEG_SUBSAMPLING_442 = MK_SUBSAMPLING(1, 2, 1, 2, 1, 1, 0, 0);
        public static readonly uint GPUJPEG_SUBSAMPLING_421 = MK_SUBSAMPLING(2, 2, 2, 1, 1, 1, 0, 0);

        // MK_SUBSAMPLING function equivalent
        public static uint MK_SUBSAMPLING(
            uint comp1_factor_h, uint comp1_factor_v,
            uint comp2_factor_h, uint comp2_factor_v,
            uint comp3_factor_h, uint comp3_factor_v,
            uint comp4_factor_h, uint comp4_factor_v)
        {
            return (comp1_factor_h << 28) | (comp1_factor_v << 24) |
                   (comp2_factor_h << 20) | (comp2_factor_v << 16) |
                   (comp3_factor_h << 12) | (comp3_factor_v << 8) |
                   (comp4_factor_h << 4) | (comp4_factor_v << 0);
        }

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_parameters_chroma_subsampling")]
        public static extern void ParametersChromaSubsampling(ref Parameters param, SamplingFactor subsampling);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_subsampling_get_name")]
        public static extern string SubsamplingGetName(int compCount, [In] ComponentSamplingFactor[] samplingFactor);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_subsampling_from_name")]
        public static extern SamplingFactor SubsamplingFromName(string subsampling);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_image_set_default_parameters")]
        public static extern void ImageSetDefaultParameters(ref ImageParameters param);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_default_image_parameters")]
        public static extern ImageParameters DefaultImageParameters();


        // Struct for gpujpeg_image_parameters
        [StructLayout(LayoutKind.Sequential)]
        public struct ImageParameters
        {
            /// Image data width
            public int Width;

            /// Image data height
            public int Height;

            /// Image data color space
            public ColorSpace ColorSpace;

            /// Image data sampling factor
            public PixelFormat PixelFormat;
        }


        public enum ImageFileFormat
        {
            /// Unknown image file format
            GPUJPEG_IMAGE_FILE_UNKNOWN = 0,
            /// JPEG file format
            GPUJPEG_IMAGE_FILE_JPEG = 1,
            /// Raw file format
            /// @note all following formats must be raw
            GPUJPEG_IMAGE_FILE_RAW = 2,
            /// Gray file format
            GPUJPEG_IMAGE_FILE_GRAY,
            /// RGB file format, simple data format without header [R G B] [R G B] ...
            GPUJPEG_IMAGE_FILE_RGB,
            /// RGBA file format, simple data format without header [R G B A] [R G B A] ...
            GPUJPEG_IMAGE_FILE_RGBA,
            /// PNM file format
            GPUJPEG_IMAGE_FILE_PGM,
            GPUJPEG_IMAGE_FILE_PPM,
            GPUJPEG_IMAGE_FILE_PNM,
            /// PAM file format
            GPUJPEG_IMAGE_FILE_PAM,
            GPUJPEG_IMAGE_FILE_Y4M,
            /// YUV file format, simple data format without header [Y U V] [Y U V] ...
            /// @note all following formats must be YUV
            GPUJPEG_IMAGE_FILE_YUV,
            /// YUV file format with alpha channel [Y U V A] [Y U V A] ...
            GPUJPEG_IMAGE_FILE_YUVA,
            /// i420 file format
            GPUJPEG_IMAGE_FILE_I420,
            /// testing (empty) image, that is SW generated
            GPUJPEG_IMAGE_FILE_TST,
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct DurationStats
        {
            public double DurationMemoryTo;
            public double DurationMemoryFrom;
            public double DurationMemoryMap;
            public double DurationMemoryUnmap;
            public double DurationPreprocessor;
            public double DurationDctQuantization;
            public double DurationHuffmanCoder;
            public double DurationStream;
            public double DurationInGpu;
        }

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_image_file_format")]
        public static extern ImageFileFormat ImageGetFileFormat(string filename);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_set_device")]
        public static extern void SetDevice(int index);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_image_calculate_size")]
        public static extern UIntPtr ImageCalculateSize(ref ImageParameters param);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_image_load_from_file")]
        public static extern int ImageLoadFromFile(string filename, out IntPtr image, ref UIntPtr imageSize);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_image_save_to_file")]
        public static extern int ImageSaveToFile(string filename, IntPtr image, UIntPtr imageSize, ref ImageParameters paramImage);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_image_get_properties")]
        public static extern int ImageGetProperties(string filename, ref ImageParameters paramImage, int fileExists);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_image_destroy")]
        public static extern int ImageDestroy(IntPtr image);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_image_range_info")]
        public static extern void ImageRangeInfo(string filename, int width, int height, PixelFormat samplingFactor);

        [DllImport("gpujpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "gpujpeg_image_convert")]
        public static extern int ImageConvert(string input, string output, ImageParameters paramImageFrom, ImageParameters paramImageTo);

        //For opengl part
        struct OpenglContext { }

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_opengl_init", CallingConvention = CallingConvention.Cdecl)]
        public static extern int OpenglInit(out IntPtr ctx);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_opengl_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void OpenglDestroy(IntPtr ctx);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_opengl_texture_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern int OpenglTextureCreate(int width, int height, IntPtr data);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_opengl_texture_set_data", CallingConvention = CallingConvention.Cdecl)]
        public static extern int OpenglTextureSetData(int textureId, IntPtr data);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_opengl_texture_get_data", CallingConvention = CallingConvention.Cdecl)]
        public static extern int OpenglTextureGetData(int textureId, IntPtr data, ref UIntPtr dataSize);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_opengl_texture_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void OpenglTextureDestroy(int textureId);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_opengl_texture_register", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr OpenglTextureRegister(int textureId, OpenglTextureType textureType);

        public enum OpenglTextureType
        {
            GPUJPEG_OPENGL_TEXTURE_READ = 1,
            GPUJPEG_OPENGL_TEXTURE_WRITE = 2
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OpenglTexture
        {
            public int TextureId;
            public OpenglTextureType TextureType;
            public int TextureWidth;
            public int TextureHeight;
            public int TexturePboType;
            public int TexturePboId;
            public IntPtr TexturePboResource; // cudaGraphicsResource* is represented by IntPtr
            public IntPtr TextureCallbackParam; // void* is represented by IntPtr

            // Function pointers for the callbacks
            public IntPtr TextureCallbackAttachOpengl;
            public IntPtr TextureCallbackDetachOpengl;
        }

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_opengl_texture_unregister", CallingConvention = CallingConvention.Cdecl)]
        public static extern void OpenglTextureUnregister(IntPtr texture);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_opengl_texture_map", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr OpenglTextureMap(IntPtr texture, ref UIntPtr dataSize);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_opengl_texture_unmap", CallingConvention = CallingConvention.Cdecl)]
        public static extern void OpenglTextureUnmap(IntPtr texture);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_color_space_get_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ColorSpaceGetName(ColorSpace colorSpace);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_pixel_format_by_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern PixelFormat PixelFormatByName(string name);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_print_pixel_formats", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PrintPixelFormats();

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_color_space_by_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern ColorSpace ColorSpaceByName(string name);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_pixel_format_get_comp_count", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PixelFormatGetCompCount(PixelFormat pixelFormat);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_pixel_format_get_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PixelFormatGetName(PixelFormat pixelFormat);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_pixel_format_is_planar", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PixelFormatIsPlanar(PixelFormat pixelFormat);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_device_reset", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeviceReset();

    }



}
