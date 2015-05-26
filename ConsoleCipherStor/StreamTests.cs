// Crypteron Inc CONFIDENTIAL
// Copyright (c) 2012 Crypteron Inc, All Rights Reserved.
// 
// NOTICE: All information contained herein is, and remains the property of Crypteron Inc. 
// The intellectual and technical concepts contained herein are proprietary to Crypteron Inc 
// and may be covered by U.S. and Foreign Patents, patents in process, and are protected 
// by trade secret or copyright law. Dissemination of this information or reproduction of 
// this material is strictly forbidden unless prior written permission is obtained from 
// Crypteron Inc. Access to the source code contained herein is hereby forbidden to anyone 
// except current Crypteron Inc employees, managers or contractors who have executed 
// Confidentiality and Non-disclosure agreements explicitly covering such access.
// 
// The copyright notice above does not evidence any actual or intended publication or disclosure 
// of this source code, which includes information that is confidential and/or proprietary, and 
// is a trade secret, of Crypteron Inc. ANY REPRODUCTION, MODIFICATION, DISTRIBUTION, PUBLIC 
// PERFORMANCE, OR PUBLIC DISPLAY OF OR THROUGH USE OF THIS SOURCE CODE WITHOUT THE EXPRESS 
// WRITTEN CONSENT OF CRYPTERON INC IS STRICTLY PROHIBITED, AND IN VIOLATION OF APPLICABLE LAWS 
// AND INTERNATIONAL TREATIES. THE RECEIPT OR POSSESSION OF THIS SOURCE CODE AND/OR RELATED 
// INFORMATION DOES NOT CONVEY OR IMPLY ANY RIGHTS TO REPRODUCE, DISCLOSE OR DISTRIBUTE ITS 
// CONTENTS, OR TO MANUFACTURE, USE, OR SELL ANYTHING THAT IT MAY DESCRIBE, IN WHOLE OR IN PART. 

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using CipherCore;
using CipherCore.Blob;
using CipherCore.Crypto.Ciphers;
using CipherCore.Utility;
using Microsoft.WindowsAzure.Storage;
using ServiceStack.Text;

namespace BlobStorage
{
    internal class StreamTests
    {

        private void FileStreamToCompressedGCMEncryptedFileStream(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                using (var destFs = new FileStream(filePath + Constants.CipherFileExtension, FileMode.OpenOrCreate))
                {
                    var aesKey = new byte[128 / 8];
                    aesKey = ByteUtils.InitTo(aesKey, 0x55);
                    var aes = new AesGcmTransform(aesKey);

                    var IV = new byte[128 / 8];
                    IV = ByteUtils.InitTo(IV, 0xaa);
                    var addtnlAuthData = Encoding.UTF8.GetBytes("This is auth info");

                    using (ICryptoTransform encryptor = aes.CreateEncryptor(addtnlAuthData, IV))
                    using (var encryptStream = new CryptoStream(destFs, encryptor, CryptoStreamMode.Write))
                    {
                        using (var compressionStream = new GZipStream(encryptStream, CompressionMode.Compress))
                        {
                            var buffer = new byte[4 * 1024];
                            int bytesRead;
                            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                compressionStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
        }

        private void FileStreamToGCMDecryptDecompressFileStream(string filePath)
        {
            var filePathDest = filePath.Remove(filePath.Length - Constants.CipherFileExtension.Length);
            filePathDest += ".recover" + Path.GetExtension(filePathDest);
            using (var compEncFs = new FileStream(filePath, FileMode.Open))
            {
                using (var plainFs = new FileStream(filePathDest, FileMode.OpenOrCreate))
                {
                    var aesKey = new byte[128 / 8];
                    aesKey = ByteUtils.InitTo(aesKey, 0x55);
                    var aes = new AesGcmTransform(aesKey);
                    var IV = new byte[128 / 8];
                    IV = ByteUtils.InitTo(IV, 0xaa);
                    var addtnlAuthData = Encoding.UTF8.GetBytes("This is auth info");

                    using (ICryptoTransform decryptor = aes.CreateDecryptor(addtnlAuthData, IV))
                    using (var decryptStream = new CryptoStream(compEncFs, decryptor, CryptoStreamMode.Read))
                    {
                        using (var zipStream = new GZipStream(decryptStream, CompressionMode.Decompress))
                        {
                            var buffer = new byte[4 * 1024];
                            int bytesRead;
                            while ((bytesRead = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                plainFs.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
        }

        private void FileStreamToGCMEncryptedFileStream(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                using (var destFs = new FileStream(filePath + Constants.CipherFileExtension, FileMode.OpenOrCreate))
                {
                    var aesKey = new byte[128 / 8];
                    aesKey = ByteUtils.InitTo(aesKey, 0x55);
                    var aes = new AesGcmTransform(aesKey);

                    var aadString = "This is auth info";
                    var addtnlAuthData = Encoding.UTF8.GetBytes(aadString);
                    Console.WriteLine("Enter AAD[\"{0}\"]:", aadString);
                    aadString = Console.ReadLine();
                    if (aadString.Length > 0)
                    {
                        Console.WriteLine("Using new AAD of : \"{0}\"", aadString);
                        addtnlAuthData = Encoding.UTF8.GetBytes(aadString);
                    }

                    using (ICryptoTransform encryptor = aes.CreateEncryptor(addtnlAuthData))
                    using (var encryptStream = new CryptoStream(destFs, encryptor, CryptoStreamMode.Write))
                    {
                        var buffer = new byte[4 * 1024];
                        int bytesRead;
                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            encryptStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }

        private void FileStreamToGCMDecryptFileStream(string filePath)
        {
            var filePathDest = filePath.Remove(filePath.Length - Constants.CipherFileExtension.Length);
            filePathDest += ".recover" + Path.GetExtension(filePathDest);
            try
            {
                using (var readFs = new FileStream(filePath, FileMode.Open))
                {
                    using (var plainFs = new FileStream(filePathDest, FileMode.OpenOrCreate))
                    {
                        var aesKey = new byte[128 / 8];
                        aesKey = ByteUtils.InitTo(aesKey, 0x55);
                        var aes = new AesGcmTransform(aesKey);

                        var IV = new byte[128 / 8];
                        IV = ByteUtils.InitTo(IV, 0xaa);
                        var addtnlAuthData = Encoding.UTF8.GetBytes("This is auth info");

                        using (ICryptoTransform decryptor = aes.CreateDecryptor(addtnlAuthData, IV))
                        using (var decryptStream = new CryptoStream(readFs, decryptor, CryptoStreamMode.Read))
                        {
                            var buffer = new byte[4 * 1024];
                            int bytesRead;
                            while ((bytesRead = decryptStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                plainFs.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, TraceEventType.Critical);
            }
        }

        private void UploadFileToBlob(string filePath)
        {
            // Config keys
            var cfg = new CipherCoreConfig
                {
                    Location = CipherDbLocation.Local,
                    //ConnectionStringName = "StorageConnectionString",
                    //ContainerName = "cipher-db-demo",   // new cert, new deployment
                    //ContainerName = "cipher-db-test", // old cert, old deployment
                    Filename = "newkeytest.der",
                    KeychainFormat = KeychainFormat.DER,
                    MigrationPolicy = MigrationPolicy.ReadOld,
                    MigrationLimit = 50
                };
            var cipherBlob = new CipherBlobExt(cfg);

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Set target blob
            var fileName = Path.GetFileName(filePath) + ".cipher";
            cipherBlob.SetBlobTarget(fileName, "cipher-db-test", storageAccount);

            using (var plainFs = new FileStream(filePath, FileMode.Open))
            {
                cipherBlob.UploadFromStream(plainFs);
            }
        }

        private void FileStreamToCompressedEncryptedFileStream(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                using (var destFs = new FileStream(filePath + Constants.CipherFileExtension, FileMode.OpenOrCreate))
                {
                    var aes = new AesManaged();
                    var key = new byte[128/8];
                    var iv = new byte[128/8];
                    aes.Key = ByteUtils.InitTo(key, 0x55);
                    aes.IV = ByteUtils.InitTo(iv, 0xaa);

                    using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var encryptStream = new CryptoStream(destFs, encryptor, CryptoStreamMode.Write))
                    {
                        using (var compressionStream = new GZipStream(encryptStream, CompressionMode.Compress))
                        {
                            var buffer = new byte[4 * 1024];
                            int bytesRead;
                            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                compressionStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
        }

        private void FileStreamToDecryptDecompressFileStream(string filePath)
        {
            var filePathDest = filePath.Remove(filePath.Length - Constants.CipherFileExtension.Length);
            filePathDest += ".recover" + Path.GetExtension(filePathDest);
            using (var compEncFs = new FileStream(filePath, FileMode.Open))
            {
                using (var plainFs = new FileStream(filePathDest, FileMode.OpenOrCreate))
                {
                    var aes = new AesManaged();
                    var key = new byte[128 / 8];
                    var iv = new byte[128 / 8];
                    aes.Key = ByteUtils.InitTo(key, 0x55);
                    aes.IV = ByteUtils.InitTo(iv, 0xaa);

                    using (ICryptoTransform encryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var decryptStream = new CryptoStream(compEncFs, encryptor, CryptoStreamMode.Read))
                    {
                        using (var zipStream = new GZipStream(decryptStream, CompressionMode.Decompress))
                        {
                            var buffer = new byte[4 * 1024];
                            int bytesRead;
                            while ((bytesRead = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                plainFs.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
        }

        private void FileStreamToFileStream(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                // insert 'cipher' *before* extension for visual reference
                var ext = Path.GetExtension(filePath);
                var filename = Path.GetFileNameWithoutExtension(filePath);
                var dir = Path.GetDirectoryName(filePath);
                ext = "cipher" + ext;
                var outFile = dir + filename + ext;

                using (var destFs = new FileStream(outFile, FileMode.OpenOrCreate))
                {
                    var buffer = new byte[4*1024];
                    int bytesRead;
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        destFs.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        private void FileStreamToCompressFileStream(string filePath)
        {

            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                using (var destFs = new FileStream(filePath + ".zip", FileMode.OpenOrCreate))
                {
                    using (var compressionStream = new GZipStream(destFs, CompressionMode.Compress))
                    {
                        var buffer = new byte[4 * 1024];
                        int bytesRead;
                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            compressionStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }
    }
}