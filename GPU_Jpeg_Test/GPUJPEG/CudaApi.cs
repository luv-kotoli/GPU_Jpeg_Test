using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static GPU_Jpeg_Test.GPUJPEG.Common;

namespace GPU_Jpeg_Test.GPUJPEG
{
    public static class CudaApi
    {
        private const string CUDADllName = "cudart64_12.dll";

        [DllImport(CUDADllName, EntryPoint = "cudaStreamCreate")]
        public static extern CudaError CUDAStreamCreate(ref CudaStream stream);

        [DllImport(CUDADllName, EntryPoint = "cudaDeviceSynchronize")]
        public static extern CudaError CUDADeviceSynchronize();
    }
}
