using GPU_Jpeg_Test.GPUJPEG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GPU_Jpeg_Test.GPUJPEG.Decode;
using static GPU_Jpeg_Test.GPUJPEG.Common;
using static GPU_Jpeg_Test.GPUJPEG.Encode;
using static GPU_Jpeg_Test.GPUJPEG.Type;
using System.Diagnostics;
using OpenCvSharp;

namespace GPU_Jpeg_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            int deviceId = 0;
            Encode.Encoder encoder = EncoderInit(deviceId);
            //string imagePath = "D:/yuxx/output.pnm";
            string imagePath = "D:/yuxx/output.rgb";
            EncodeImageTest(imagePath, encoder);


            //Decode.Decoder decoder = DecoderInit(deviceId);
            //UIntPtr imageSize = UIntPtr.Zero;
            //IntPtr image = IntPtr.Zero;

            //// load image
            //string imagePath = "D:/yuxx/test.jpg";
            //var loadImageResult = ImageLoadFromFile(imagePath, out image, ref imageSize);
            //if (loadImageResult != 0)
            //{
            //    Console.WriteLine("Load image failed");
            //}

            //DecoderOutput decoderOutput = new DecoderOutput();
            //DecoderOutputSetDefault(ref decoderOutput);

            //Stopwatch sw = Stopwatch.StartNew();
            //var decodeResult = DecoderDecode(decoder, image, imageSize, ref decoderOutput);
            //sw.Stop();
            //Trace.WriteLine($"Decode Time: {sw.ElapsedMilliseconds}");

            //if (decodeResult != 0)
            //{
            //    Console.WriteLine("Decode Failed");
            //}

            //// save raw image to file
            //var saveResult = ImageSaveToFile("output.pnm", decoderOutput.data, imageSize, ref decoderOutput.param_image);
            //if (saveResult != 0)
            //{
            //    Console.WriteLine("Save Raw Image Failed");
            //}

            //ImageDestroy(image);
            //DecoderDestroy(decoder);
        }

        public static Decode.Decoder DecoderInit(int deviceId)
        {
            CudaStream cudaStream = new CudaStream();
            CudaError error = CudaApi.CUDAStreamCreate(ref cudaStream);
            var initResult = InitDevice(deviceId, 1);
            if (initResult != 0)
            {
                Console.WriteLine("gpu library init failed");
            }

            Decode.Decoder decoder = DecoderCreate(cudaStream);
            return decoder;
        }

        public static Encode.Encoder EncoderInit(int deviceId)
        {
            CudaStream cudaStream = new CudaStream();
            CudaError error = CudaApi.CUDAStreamCreate(ref cudaStream);
            var gpuInitResult = InitDevice(deviceId, 1); // verbose laevel 1:show message
            if (gpuInitResult != 0)
            {
                Console.WriteLine("Gpu stream init failed");
            }
            Encode.Encoder encoder = EncoderCreate(cudaStream);
            return encoder;
        }

        public static void EncodeImageTest(string imagePath, Encode.Encoder encoder)
        {
            Parameters encodeParams = new Parameters();
            SetDefaultParameters(ref encodeParams);
            encodeParams.Quality = 75;
            encodeParams.Verbose = 3;
            encodeParams.Interleaved = 1;
            encodeParams.SegmentInfo = 12800;

            ImageParameters imageParams = new ImageParameters();
            ImageSetDefaultParameters(ref imageParams);
            imageParams.Width = 3840;
            imageParams.Height = 2160;
            imageParams.ColorSpace = ColorSpace.GPUJPEG_RGB;
            imageParams.PixelFormat = PixelFormat.GPUJPEG_444_U8_P012;

            // use 4:2:0 YCbCr subsampling
            ParametersChromaSubsampling(ref encodeParams, GPUJPEG_SUBSAMPLING_444);
            
            // Load image from file
            //Mat imageMat = Cv2.ImRead(imagePath);
            UIntPtr imageSize = UIntPtr.Zero;
            IntPtr image = IntPtr.Zero;
            var readImageResult = ImageLoadFromFile(imagePath, out image, ref imageSize);
            if (readImageResult != 0)
            {
                Console.WriteLine($"Load Image Error");
            }
            
            EncoderInput encoderInput = new EncoderInput();
            EncoderInputSetImage(ref encoderInput, image);
            IntPtr imageCompressed = IntPtr.Zero;
            UIntPtr imageCompressedSize = UIntPtr.Zero;
            var encodeResult = EncoderEncode(encoder, ref encodeParams, ref imageParams, ref encoderInput, out imageCompressed, out imageCompressedSize);

            if (encodeResult != 0)
            {
                Console.WriteLine("Encode Process Error");
            }

            ImageParameters outImgParams = new ImageParameters();
            ImageSetDefaultParameters(ref outImgParams);
            outImgParams.Width = imageParams.Width;
            outImgParams.Height = imageParams.Height;
            outImgParams.ColorSpace = ColorSpace.GPUJPEG_RGB;

            //outImgParams.PixelFormat = PixelFormat.GPUJPEG_444_U8_P0P1P2;
            var saveToFileResult = ImageSaveToFile("D:/yuxx/test_encode_rgb1.jpg", imageCompressed, imageCompressedSize, ref outImgParams);
            if (saveToFileResult != 0)
            {
                Console.WriteLine("Write To file failed");
            }
            ImageDestroy(image);
            EncoderDestroy(encoder);
        }


    }
}
