using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static GPU_Jpeg_Test.GPUJPEG.Common;


namespace GPU_Jpeg_Test.GPUJPEG
{
    public static class Encode
    {
        // Forward declaration for gpujpeg_encoder
        [StructLayout(LayoutKind.Sequential)]
        public struct Encoder
        {
            private IntPtr handle;
            // Placeholder for any fields if needed
        }

        // Encoder input type
        public enum InputType
        {
            // Encoder will use custom input buffer
            GPUJPEG_ENCODER_INPUT_IMAGE,

            // Encoder will use OpenGL Texture PBO Resource as input buffer
            GPUJPEG_ENCODER_INPUT_OPENGL_TEXTURE,

            // Encoder will use custom GPU input buffer
            GPUJPEG_ENCODER_INPUT_GPU_IMAGE,
        }

        // Encoder input structure
        [StructLayout(LayoutKind.Sequential)]
        public struct EncoderInput
        {
            // Output type
            public InputType type;

            // Image data
            public IntPtr image;

            // Registered OpenGL Texture
            public IntPtr texture;
        }

        // Set encoder input to image data
        [DllImport("gpujpeg", EntryPoint = "gpujpeg_encoder_input_set_image", CallingConvention = CallingConvention.Cdecl)]
        public static extern void EncoderInputSetImage(ref EncoderInput input, IntPtr image);

        // Set encoder input to GPU image data
        [DllImport("gpujpeg", EntryPoint = "gpujpeg_encoder_input_set_gpu_image", CallingConvention = CallingConvention.Cdecl)]
        public static extern void EncoderInputSetGpuImage(ref EncoderInput input, IntPtr image);

        // Set encoder input to OpenGL texture
        [DllImport("gpujpeg", EntryPoint = "gpujpeg_encoder_input_set_texture", CallingConvention = CallingConvention.Cdecl)]
        public static extern void EncoderInputSetTexture(ref EncoderInput input, IntPtr texture);

        // Create JPEG encoder
        [DllImport("gpujpeg", EntryPoint = "gpujpeg_encoder_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern Encoder EncoderCreate(CudaStream stream);

        // Compute maximum number of image pixels
        [DllImport("gpujpeg", EntryPoint = "gpujpeg_encoder_max_pixels", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr EncoderMaxPixels(ref Parameters param, ref ImageParameters paramImage, InputType imageInputType, UIntPtr memorySize, out int maxPixels);

        // Compute maximum size of device memory
        [DllImport("gpujpeg", EntryPoint = "gpujpeg_encoder_max_memory", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr EncoderMaxMemory(ref Parameters param, ref ImageParameters paramImage, InputType imageInputType, int maxPixels);

        // Pre-allocate encoding buffers
        [DllImport("gpujpeg", EntryPoint = "gpujpeg_encoder_allocate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EncoderAllocate(Encoder encoder, ref Parameters param, ref ImageParameters paramImage, InputType imageInputType);

        // Compress image by encoder
        [DllImport("gpujpeg", EntryPoint = "gpujpeg_encoder_encode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EncoderEncode(Encoder encoder, ref Parameters param, ref ImageParameters paramImage, ref EncoderInput input, out IntPtr imageCompressed, out UIntPtr imageCompressedSize);

        // Get duration statistics for last encoded image
        [DllImport("gpujpeg", EntryPoint = "gpujpeg_encoder_get_stats", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EncoderGetStats(Encoder encoder, ref DurationStats stats);

        // JPEG header type enum
        [Flags]
        public enum HeaderType
        {
            GPUJPEG_HEADER_DEFAULT = 0,
            GPUJPEG_HEADER_JFIF = 1 << 0,
            GPUJPEG_HEADER_SPIFF = 1 << 1,
            GPUJPEG_HEADER_ADOBE = 1 << 2
        }

        // Set JPEG header
        [DllImport("gpujpeg", EntryPoint = "gpujpeg_encoder_set_jpeg_header", CallingConvention = CallingConvention.Cdecl)]
        public static extern void EncoderSetJpegHeader(Encoder encoder, HeaderType headerType);

        // Suggest optimal restart interval
        [DllImport("gpujpeg", EntryPoint = "gpujpeg_encoder_suggest_restart_interval", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EncoderSuggestRestartInterval(ref ImageParameters paramImage, int subsampling, bool interleaved, int verbose);

        // Destroy JPEG encoder
        [DllImport("gpujpeg", EntryPoint = "gpujpeg_encoder_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EncoderDestroy(Encoder encoder);
    }
    
}
