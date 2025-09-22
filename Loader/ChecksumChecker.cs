using System.IO;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace ModShardLauncher.Loader
{
    static public class ChecksumChecker
    {
        private static string ComputeChecksum(FileStream stream)
        {
            using MD5 md5 = MD5.Create();
            return Convert.ToHexString(md5.ComputeHash(stream));
        }
        /// <summary>
        /// Return True if the MD5 checksum of a file is equal a valid precomputed checksum.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool CompareChecksum(FileStream stream)
        {
            string hash = ComputeChecksum(stream);

            string[] checksums =
            {
                "5F91989CA7E2A2B1234B2CD2A6AF9821", // Steam 0.9.1.16-vm
                "2BD331F728428746FA337D6C7B67040A", // Steam 0.9.1.17-vm
                "6F9F1E29275EEF60E3A725ECA1033DF8", // Steam 0.9.1.18-vm
                "47282D0C650216D88AE25FA99615F9CB", // Steam 0.9.3.9-vm
            };
            return checksums.Contains(hash);
        }
    }
}