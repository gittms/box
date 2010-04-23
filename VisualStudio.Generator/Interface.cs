using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using Definitif.VisualStudio.Generator;

namespace Definitif.VisualStudio
{
    [Guid("CAE380BA-42D4-11DF-B540-4B2F56D89593")]
    public class BoxFileGenerator : IVsSingleFileGenerator, IDisposable
    {
        IntPtr resPtr;

        /// <summary>
        /// Provides default output file extension.
        /// </summary>
        int IVsSingleFileGenerator.DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".cs";
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Performs code generation based on input.
        /// </summary>
        int IVsSingleFileGenerator.Generate(
            string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace,
            IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            string code = "// Output of Definitif.VisualStudio.Generator";

            // Passing code output to Visual Studio.
            byte[] result = Encoding.UTF8.GetBytes(code);
            pcbOutput = (uint)result.Length;

            resPtr = Marshal.AllocCoTaskMem(result.Length);
            Marshal.Copy(result, 0, resPtr, result.Length);
            rgbOutputFileContents[0] = resPtr;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Disposes generator object and pointers.
        /// </summary>
        void IDisposable.Dispose()
        {
            Marshal.ZeroFreeCoTaskMemUnicode(resPtr);
        }
    }
}
