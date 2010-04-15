using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;

namespace Definitif.VisualStudio.Generator
{
    [Guid("CAE380BA-42D4-11DF-B540-4B2F56D89593")]
    public class Generator : IVsSingleFileGenerator, IDisposable
    {
        IntPtr resPtr;

        int IVsSingleFileGenerator.DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".cs";
            return VSConstants.S_OK;
        }

        int IVsSingleFileGenerator.Generate(
            string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace,
            IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            byte[] result = Encoding.UTF8.GetBytes("// Output of Definitif.VisualStudio.Generator");
            pcbOutput = (uint)result.Length;

            resPtr = Marshal.AllocCoTaskMem(result.Length);
            Marshal.Copy(result, 0, resPtr, result.Length);
            rgbOutputFileContents[0] = resPtr;
            return VSConstants.S_OK;
        }

        void IDisposable.Dispose()
        {
            Marshal.ZeroFreeCoTaskMemUnicode(resPtr);
        }
    }
}
