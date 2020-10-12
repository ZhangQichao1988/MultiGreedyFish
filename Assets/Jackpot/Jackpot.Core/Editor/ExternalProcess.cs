/**
 * Jackpot
 * Copyright(c) 2014 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 * 
 * Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
 * conditions, Licensor grants to you, and you hereby accept nontransferable,
 * nonexclusive limited right to access, obtain, use, copy and/or download
 * a copy of this product only for requirement purposes. You may not rent,
 * lease, loan, time share, sublicense, transfer, make generally available,
 * license, disclose, disseminate, distribute or otherwise make accessible or
 * available this product to any third party without the prior written approval
 * of Licensor. Unauthorized copying of this product, including modifications
 * of this product or programs in which this product has been merged or included
 * with other software products is expressly forbidden.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Jackpot
{
    public static class ExternalProcess
    {
        public static ProcessStartInfo CreateInfo(string file, string arguments, string workingDirectory)
        {
            return new ProcessStartInfo(file, arguments) {
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        public static Tuple<string, string, bool> ProcessWithTimeout(ProcessStartInfo psi, TimeSpan timeout)
        {
            using (var process = Process.Start(psi))
            {
                var stdout = new StringBuilder();
                var stderr = new StringBuilder();
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        stdout.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        stderr.AppendLine(e.Data);
                    }
                };
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                var isTimedOut = false;
                if (!process.WaitForExit((int) timeout.TotalMilliseconds))
                {
                    isTimedOut = true;
                    process.Kill();
                }
                process.CancelOutputRead();
                process.CancelErrorRead();

                return Tuple.Create(
                    stdout.Replace("\r\r\n", "\n").ToString(),
                    stderr.Replace("\r\r\n", "\n").ToString(),
                    isTimedOut
                );
            }
        }
    }
}
