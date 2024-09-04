using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace GPU_Jpeg_Test.GPUJPEG
{
    public static class Type
    {
        // Constants
        public const int GPUJPEG_MAX_COMPONENT_COUNT = 4;

        // Flags
        public const int GPUJPEG_VERBOSE = 1;
        public const int GPUJPEG_OPENGL_INTEROPERABILITY = 2;

        // Maximum number of segment info header in stream
        public const int GPUJPEG_MAX_SEGMENT_INFO_HEADER_COUNT = 100;

        // Errors
        public const int GPUJPEG_NOERR = 0;
        public const int GPUJPEG_ERROR = -1;
        public const int GPUJPEG_ERR_RESTART_CHANGE = -2;

        // Color spaces for JPEG codec
        public enum ColorSpace
        {
            GPUJPEG_NONE = 0,
            GPUJPEG_RGB = 1,
            GPUJPEG_YCBCR_BT601 = 2,          // limited-range YCbCr BT.601
            GPUJPEG_YCBCR_BT601_256LVLS = 3,  // full-range YCbCr BT.601
            GPUJPEG_YCBCR_JPEG = GPUJPEG_YCBCR_BT601_256LVLS, // GPUJPEG_YCBCR_BT601_256LVLS
            GPUJPEG_YCBCR_BT709 = 4,          // limited-range YCbCr BT.709
            GPUJPEG_YCBCR = GPUJPEG_YCBCR_BT709,  // GPUJPEG_YCBCR_BT709
            GPUJPEG_YUV = 5                   // Deprecated: will be removed soon
        }

        // Pixel format for input/output image data
        public enum PixelFormat
        {
            GPUJPEG_PIXFMT_NONE = -1,

            // 8bit unsigned samples, 1 component
            GPUJPEG_U8 = 0,

            // 8bit unsigned samples, 3 components, 4:4:4 sampling,
            // sample order: comp#0 comp#1 comp#2, interleaved
            GPUJPEG_444_U8_P012 = 1,

            // 8bit unsigned samples, 3 components, 4:4:4, planar
            GPUJPEG_444_U8_P0P1P2 = 2,

            // 8bit unsigned samples, 3 components, 4:2:2,
            // order of samples: comp#1 comp#0 comp#2 comp#0, interleaved
            GPUJPEG_422_U8_P1020 = 3,

            // 8bit unsigned samples, planar, 3 components, 4:2:2, planar
            GPUJPEG_422_U8_P0P1P2 = 4,

            // 8bit unsigned samples, planar, 3 components, 4:2:0, planar
            GPUJPEG_420_U8_P0P1P2 = 5,

            // 8bit unsigned samples, 3 or 4 components, each pixel padded to 32bits
            // with optional alpha or unused, 4:4:4(:4) sampling, interleaved
            GPUJPEG_4444_U8_P0123 = 6,
        }

        // Sampling factor for color component in JPEG format
        [StructLayout(LayoutKind.Sequential)]
        public struct ComponentSamplingFactor
        {
            public byte horizontal;
            public byte vertical;
        }
    }
    
        
    

    

}
