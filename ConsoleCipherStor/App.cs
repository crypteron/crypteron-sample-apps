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
using System.Globalization;
using System.IO;
using Crypteron.CipherCore.Utility;
using Crypteron.CipherStor;
using Microsoft.WindowsAzure.Storage;

namespace Crypteron.SampleApps.ConsoleCipherStor
{
    public enum Destination
    {
        Local,
        Blob
    }

    internal class App
    {
        private bool _compress = false;
        private Destination _dest = Destination.Local;
        private string _file = "TestText.txt";
        private readonly string _fileTestFolder = Directory.GetCurrentDirectory() + "\\..\\..\\FileStorage\\";

        private void PrintMenu()
        {
            Console.WriteLine("E : (E)ncrypt test file");
            Console.WriteLine("D : (D)ecrypt test file");
            Console.WriteLine("C : Toggle (C)ompression[{0}]", _compress);
            Console.WriteLine("S : Set de(S)tination[{0}]", _dest);
            Console.WriteLine("F : Change (F)ile[{0}]", _file);
            Console.WriteLine("Q : (Q)uit");
            Console.WriteLine();
            Console.WriteLine(@"All files are in the 'SampleApps\ConsoleBlob\Test\' folder");
        }

        public void Run()
        {
            bool ongoing = true;

            // All CipherStor APIs are now through the 
            // CipherStorExt object
            var cipherBlobExt = new CipherStorExt();

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

            // Set target blob
            cipherBlobExt.RaiseProgressEvent += HandleProgressEvent;

            while (ongoing)
            {
                string filePath = _fileTestFolder + _file;

                PrintMenu();
                // Get char in lower case, skipping whitespaced chars
                var userInput = Console.ReadKey().KeyChar.ToString(CultureInfo.InvariantCulture).Trim().ToLowerInvariant();
                if (userInput.Length > 0)
                {
                    char option = userInput.ToCharArray()[0];
                    Console.WriteLine(" ...");
                    switch (option)
                    {
                        case 'e':
                            if (_dest == Destination.Local)
                                cipherBlobExt.EncryptLocalFile(filePath,
                                    filePath + CipherStor.Constants.CipherFileExtension,
                                    _compress);
                            else
                            {
                                cipherBlobExt.SetBlobTarget(_file, "sampleapps-test", storageAccount);
                                cipherBlobExt.UploadFromFile(filePath, _compress);
                            }
                            break;
                        case 'd':
                            // Decrypt to same location, with just 'recover' added to filename for clarity
                            var filePathDest = filePath + ".recover" + Path.GetExtension(filePath);
                            if (_dest == Destination.Local)
                                cipherBlobExt.DecryptLocalFile(filePath + CipherStor.Constants.CipherFileExtension,
                                    filePathDest);
                            else
                            {
                                cipherBlobExt.SetBlobTarget(_file, "sampleapps-test", storageAccount);
                                cipherBlobExt.DownloadToFile(filePathDest);
                            }
                            break;
                        case 's':
                            SetDestination();
                            break;
                        case 'c':
                            _compress = !_compress;
                            Console.WriteLine("Compression is now:{0}", _compress);
                            break;
                        case 'f':
                            SetLocalTestFile();
                            break;
                        case 'q':
                            ongoing = false;
                            break;
                        default:
                            Console.WriteLine("Unknown input");
                            break;
                    }
                }
            }
        }

        private void HandleProgressEvent(object sender, CipherCoreEventArgs e)
        {
            Console.WriteLine("Event: Processed {0} bytes.", e.BytesTransferred);
        }

        private void SetLocalTestFile()
        {
            Console.WriteLine("Choices are files in test folder[{0}]. \n Available files ...", _fileTestFolder);
            string[] filePaths = Directory.GetFiles(_fileTestFolder);
            foreach (var file in filePaths)
            {
                var justFilename = Path.GetFileName(file);
                Console.WriteLine(justFilename);
            }
            Console.WriteLine("Enter filename : [{0}]", _file);
            _file = Console.ReadLine();
        }

        private void SetDestination()
        {
            Console.WriteLine("Choices are: {0}", GetPossibleEnumValues(_dest, ", "));
            Console.Write("Enter destination: [{0}]", _dest);
            bool done = false;

            do
            {
                var input = Console.ReadLine();
                if (String.IsNullOrEmpty(input))
                    done = true;
                else 
                    done = Enum.TryParse(input, true, out _dest);
            } while (!done);
        }

        private string GetPossibleEnumValues<T>(T enumHolder, string delimiter = "\n") where T : struct
        {
            string enumNames = String.Empty;
            foreach (var fieldName in enumHolder.GetType().GetFields())
            {
                if (fieldName.Name.Equals("value__")) continue;

                enumNames += fieldName.Name + delimiter;
            }
            enumNames = enumNames.TrimEnd(delimiter.ToCharArray());
            return enumNames;
        }
    }
}