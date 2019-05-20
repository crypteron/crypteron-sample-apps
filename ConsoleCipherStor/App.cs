using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Crypteron.CipherStor;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Crypteron.SampleApps.ConsoleCipherStor
{
    public class App
    {
        private bool _compress = false;
        private string _file = "text.txt";
        private readonly string _workPath;
        private readonly string _workPathEncrypted;
        private readonly string _workPathDecrypted;
        private const string DecryptedFileSuffix = ".decrypted";
        private const string _containerName = "crypteron-sampleapps";

        public App()
        {
            _workPath = Path.Combine(Directory.GetCurrentDirectory() + "\\..\\..\\..\\FileStorage");
            _workPath = Path.GetFullPath(_workPath); // trim the ..'s above in path

            // Setup and create directories if needed
            _workPathEncrypted = Path.Combine(_workPath, "encrypt-multiple-files");
            _workPathDecrypted = Path.Combine(_workPath, "decrypt-multiple-files");
            Directory.CreateDirectory(_workPathEncrypted);
            Directory.CreateDirectory(_workPathDecrypted);
        }

        private int PrintMenuGetChoice()
        {
            do
            {
                // Print menu
                Console.WriteLine(" 0 : select file (currently: {0})", _file);
                Console.WriteLine(" 1 : encrypt file", _file);
                Console.WriteLine(" 2 : decrypt file", _file);
                Console.WriteLine(" 3 : encrypt multiple files (multiple threads, parallel)");
                Console.WriteLine(" 4 : decrypt multiple files (multiple threads, parallel)");
                Console.WriteLine(" 5 : encrypt local to azure blob storage");
                Console.WriteLine(" 6 : decrypt encrypted azure blob to local");
                Console.WriteLine(" 7 : toggle compression (currently: {0})", _compress);
                Console.WriteLine("99 : quit");
                Console.WriteLine();
                Console.WriteLine($"local files in '{_workPath}' folder");
                Console.WriteLine("====================================================");
                Console.Write("Enter your selection: ");
                // Get Input
                var userInputStr = Console.ReadLine()?.Trim().ToLowerInvariant();
                var userInput = 0;

                try
                {
                    userInput = Convert.ToInt32(userInputStr);
                    return userInput;
                }
                catch
                {
                    Console.WriteLine("** Invalid selection **");
                }
            } while (true);
        }

        public async Task Run()
        {
            bool exitProgram = false;

            var cipherBlobExt = new CipherStorClient();

            // Set target blob
            cipherBlobExt.RaiseProgressEvent += HandleProgressEvent;

            while (!exitProgram)
            {
                string inputFile;
                string outputFile;

                int choice = PrintMenuGetChoice();
                switch (choice)
                {
                    case 0:
                        SetLocalTestFile();
                        break;
                    case 1:
                        // Encrypt a file
                        // e.g. c:\path\filename.ext
                        inputFile = _workPath + "\\" + _file;
                        // e.g. c:\path\filename.ext.cipherstor
                        outputFile = inputFile + CipherStor.Constants.CipherFileExtension;
                        cipherBlobExt.EncryptLocalFile(inputFile, outputFile, _compress);
                        break;
                    case 2:
                        // Decrypt a file
                        // e.g. c:\path\filename.ext.cipherstor
                        inputFile = _workPath + "\\" + _file + CipherStor.Constants.CipherFileExtension;
                        // e.g. c:\path\filename.ext.decrypted.ext
                        //      different name from original to avoid overwriting original file
                        outputFile = _workPath + "\\" + _file + DecryptedFileSuffix + Path.GetExtension(_file);
                        cipherBlobExt.DecryptLocalFile(inputFile, outputFile);
                        break;
                    case 3:
                        // Encrypts multiple files in parallel across multiple threads
                        EncryptAllFilesInFolderParallel();
                        break;
                    case 4:
                        // Decrypts multiple files in parallel across multiple threads
                        DecryptAllFilesInFolderParallel();
                        break;
                    case 5:
                        await EncryptFileToAzureStorageAsync();
                        break;
                    case 6:
                        await DecryptFilesFromAzureStorageAsync();
                        break;
                    case 7:
                        _compress = !_compress;
                        Console.WriteLine("Compression is now:{0}", _compress);
                        break;
                    case 99:
                        exitProgram = true;
                        break;
                    default:
                        Console.WriteLine("** Invalid selection **");
                        break;
                }
            }
        }

        /// <summary>
        /// This will encrypt all files in a folder in parallel
        /// and also return back the performance to the console
        /// </summary>
        private void EncryptAllFilesInFolderParallel()
        {
            string[] clearFilePaths = Directory.GetFiles(_workPath);
            
            // Select all the original files, minus any encrypted/decrypted versions of those files
            clearFilePaths = clearFilePaths
                .Where(f => !f.Contains(CipherStor.Constants.CipherFileExtension)) // minus encrypted files
                .Where(f => !f.Contains(DecryptedFileSuffix))  // minus any decrypted files
                .ToArray();

            // Create output folder if non-existent 
            Directory.CreateDirectory(_workPathEncrypted);

            // Encrypt phase in parallel
            var sw = Stopwatch.StartNew();
            Parallel.ForEach(clearFilePaths, clearFilePath =>
            {
                var clearFilename = Path.GetFileName(clearFilePath);
                var encFileName = _workPathEncrypted + "\\" + clearFilename + CipherStor.Constants.CipherFileExtension;

                // Encrypt the file
                var cipherBlobExt = new CipherStorClient();
                cipherBlobExt.EncryptLocalFile(clearFilePath, encFileName, _compress);                
            });
            sw.Stop();

            // Only to get statistics
            long totalSizeBytes = 0;
            long numOfFiles = 0;
            foreach (var filePath in clearFilePaths)
            {
                numOfFiles++;
                totalSizeBytes += new FileInfo(filePath).Length;
            }
            var sizeInMb = totalSizeBytes/1024/2014;
            var rateInMBps = 1000*(sizeInMb/(1.0*sw.ElapsedMilliseconds));

            // Display stats
            Console.WriteLine("Encrypt:");
            Console.WriteLine($"{rateInMBps} MBytes/sec with {numOfFiles} files totalling {sizeInMb} MB ");
            Console.WriteLine($"Total time {sw.Elapsed.Seconds}.{sw.Elapsed.Milliseconds} seconds");
        }

        /// <summary>
        /// This will decrypt all files in a folder in parallel
        /// and also return back the performance to the console
        /// </summary>
        private void DecryptAllFilesInFolderParallel()
        {
            string[] encFilePaths = Directory.GetFiles(_workPathEncrypted, "*" + CipherStor.Constants.CipherFileExtension);

            // Create output folder if non-existent 
            Directory.CreateDirectory(_workPathDecrypted);

            // Decrypt phase in parallel
            var sw = Stopwatch.StartNew();
            Parallel.ForEach(encFilePaths, encFilePath =>
            {
                var encFileOrigName = Path.GetFileName(encFilePath).Replace(CipherStor.Constants.CipherFileExtension, string.Empty);
                //var encFileOrigExt = Path.GetExtension(encFileOrigName);
                var clearFilePath = _workPathDecrypted + "\\" + encFileOrigName;

                // Decrypt the file
                var cipherBlobExt = new CipherStorClient();
                cipherBlobExt.DecryptLocalFile(encFilePath, clearFilePath);
            });
            sw.Stop();

            // Only to get statistics
            long totalSizeBytes = 0;
            long numOfFiles = 0;
            foreach (var filePath in encFilePaths)
            {
                numOfFiles++;
                totalSizeBytes += new FileInfo(filePath).Length;
            }

            var sizeInMb = totalSizeBytes/1024/2014;
            var rateInMBps = 1000*(sizeInMb/(1.0*sw.ElapsedMilliseconds));

            // Display stats
            Console.WriteLine("Decrypt:");
            Console.WriteLine($"{rateInMBps} MBytes/sec with {numOfFiles} files totalling {sizeInMb} MB ");
            Console.WriteLine($"Total time {sw.Elapsed.Seconds}.{sw.Elapsed.Milliseconds} seconds");
        }

        /// <summary>
        /// This example shows how you can use the Crypteron streaming API along with 
        /// the standard Azure Storage SDK. 
        /// </summary>
        private async Task EncryptFileToAzureStorageAsync()
        {
            var inputFile = _workPath + "\\" + _file;
            var outputFile = _file + CipherStor.Constants.CipherFileExtension;

            // Get reference to remote blob
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_containerName);
            await container.CreateIfNotExistsAsync();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(outputFile);

            // Instantiated a CipherStor client
            var cipherStor = new CipherStorClient();

            using (var clearInStream = File.OpenRead(inputFile)) // Open local input stream
            using (var cipherWriteStream = await blockBlob.OpenWriteAsync()) // Open remote blob stream
            {
                // Let Crypteron CipherStor do work, key management happens behind the scenes
                // The streaming API means you can use any other storage client (e.g. AWS S3)
                // in a similar manner as above
                cipherStor.EncryptStream(clearInStream, cipherWriteStream, _compress);
            }
        }

        /// <summary>
        /// This example shows how you can use the Crypteron streaming API along with 
        /// the standard Azure Storage SDK
        /// </summary>
        private async Task DecryptFilesFromAzureStorageAsync()
        {
            var inputFile = _file + CipherStor.Constants.CipherFileExtension;
            var outputFile = _workPath + "\\" + _file + DecryptedFileSuffix + Path.GetExtension(_file);

            // Get reference to remote blob
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_containerName);
            await container.CreateIfNotExistsAsync();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(inputFile);

            // Instantiated a CipherStor client
            var cipherStor = new CipherStorClient();

            using (var clearOutStream = File.OpenWrite(outputFile)) // Open local output stream
            using (var cipherReadStream = await blockBlob.OpenReadAsync()) // Open remote blob stream
            {
                // Let Crypteron CipherStor do work, key management happens behind the scenes
                // The streaming API means you can use any other storage client (e.g. AWS S3)
                // in a similar manner as above
                cipherStor.DecryptStream(cipherReadStream, clearOutStream);
            }
        }

        private void HandleProgressEvent(object sender, CipherStorEventArgs e)
        {
            Console.WriteLine("Event: Processed {0} bytes.", e.BytesTransferred);
        }

        private void SetLocalTestFile()
        {
            // Erase any temp files that may exist from prev runs
            string[] filePaths = Directory.GetFiles(_workPath);            
            foreach (var file in filePaths.Where(f => f.Contains(DecryptedFileSuffix) || f.EndsWith("cipherstor")))
                File.Delete(file);

            // Read again after delete
            filePaths = Directory.GetFiles(_workPath);
            Console.WriteLine("Choices are files in test folder[{0}]: ", _workPath);            
            foreach (var file in filePaths)
            {
                var justFilename = Path.GetFileName(file);
                Console.WriteLine(justFilename);
            }
            Console.WriteLine("Enter filename : [{0}]", _file);
            _file = Console.ReadLine();
        }
    }
}