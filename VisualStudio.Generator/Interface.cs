using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security;
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
            pbstrDefaultExtension = ".box.cs";
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Performs code generation based on input.
        /// </summary>
        int IVsSingleFileGenerator.Generate(
            string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace,
            IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            // Finding project root path.
            string dirName = new FileInfo(wszInputFilePath).DirectoryName;
            string projectRoot = dirName;
            while (true)
            {
                DirectoryInfo dir = new DirectoryInfo(dirName);
                if (dir.GetFiles("*.*proj", SearchOption.TopDirectoryOnly).Length > 0)
                {
                    projectRoot = dirName;
                    break;
                }

                try
                {
                    if (dir.Parent == null) break;
                    dirName = dir.Parent.FullName;
                }
                catch (SecurityException)
                {
                    break;
                }
            }

            // Parsing file and generating code.
            CodeDom codeDom = CodeDom.ParseFile(wszInputFilePath, projectRoot);
            string code = CodeGenerator.Generate(codeDom, wszDefaultNamespace);

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
