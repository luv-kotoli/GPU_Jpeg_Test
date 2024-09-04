using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static GPU_Jpeg_Test.GPUJPEG.Common;
using static GPU_Jpeg_Test.GPUJPEG.Type;

namespace GPU_Jpeg_Test.GPUJPEG
{
    public static class Decode
    {
        public struct Decoder 
        {
            private IntPtr handle;
        }

        //Decoder output type
        public enum DecoderOutputType
        {
            /// Decoder will use it's internal output buffer
            GPUJPEG_DECODER_OUTPUT_INTERNAL_BUFFER,
            /// Decoder will use custom output buffer
            GPUJPEG_DECODER_OUTPUT_CUSTOM_BUFFER,
            /// Decoder will use OpenGL Texture PBO Resource as output buffer
            GPUJPEG_DECODER_OUTPUT_OPENGL_TEXTURE,
            /// Decoder will use internal CUDA buffer as output buffer
            GPUJPEG_DECODER_OUTPUT_CUDA_BUFFER,
            /// Decoder will use custom CUDA buffer as output buffer
            GPUJPEG_DECODER_OUTPUT_CUSTOM_CUDA_BUFFER,
        };


        [StructLayout(LayoutKind.Sequential)]
        public struct DecoderOutput
        {
            public DecoderOutputType type;
            public IntPtr data;
            public UIntPtr data_size;
            public ImageParameters param_image;
            public IntPtr texture;  // IntPtr to GpuJpegOpenglTexture struct
        }

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_decoder_output_set_default", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DecoderOutputSetDefault(ref DecoderOutput output);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_decoder_output_set_custom", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DecoderOutputSetCustom(ref DecoderOutput output, IntPtr customBuffer);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_decoder_output_set_texture", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DecoderOutputSetTexture(ref DecoderOutput output, IntPtr texture);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_decoder_output_set_cuda_buffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DecoderOutputSetCudaBuffer(ref DecoderOutput output);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_decoder_output_set_custom_cuda", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DecoderOutputSetCustomCuda(ref DecoderOutput output, IntPtr d_customBuffer);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_decoder_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern Decoder DecoderCreate(CudaStream stream);

        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_decoder_init", CallingConvention = CallingConvention.Cdecl)]
        public static extern int DecoderInit(IntPtr decoder, IntPtr param, IntPtr paramImage);

        // Decompress image by decoder
        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_decoder_decode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int DecoderDecode(Decoder decoder, IntPtr image, UIntPtr imageSize, ref DecoderOutput output);

        // Returns duration statistics for the last decoded image
        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_decoder_get_stats", CallingConvention = CallingConvention.Cdecl)]
        public static extern int DecoderGetStats(IntPtr decoder, ref DurationStats stats);

        // Destroy JPEG decoder
        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_decoder_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern int DecoderDestroy(Decoder decoder);

        // Constants for special pixel formats
        public const PixelFormat GPUJPEG_PIXFMT_AUTODETECT = PixelFormat.GPUJPEG_PIXFMT_NONE - 1;
        public const PixelFormat GPUJPEG_PIXFMT_NOALPHA = GPUJPEG_PIXFMT_AUTODETECT - 1;
        public const PixelFormat GPUJPEG_PIXFMT_STD = GPUJPEG_PIXFMT_NOALPHA - 1; 

        // Constant for default color space
        public const ColorSpace GpuJpegCsDefault = ColorSpace.GPUJPEG_NONE - 1;

        // Set output format
        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_decoder_set_output_format", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DecoderSetOutputFormat(IntPtr decoder, ColorSpace colorSpace, PixelFormat pixelFormat);

        // Get image info
        [DllImport("gpujpeg.dll", EntryPoint = "gpujpeg_decoder_get_image_info", CallingConvention = CallingConvention.Cdecl)]
        public static extern int DecoderGetImageInfo(IntPtr image, UIntPtr imageSize, ref ImageParameters paramImage, ref Parameters param, ref int segmentCount);
    }
}
