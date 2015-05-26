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

using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Linq;
using System.Text;

namespace Crypteron.SampleApps.CommonCode
{
    public abstract class ProcessCustomerBase
    {
        public void Benchmark(Action wipeDb, Action<int> secureWrite, Func<bool, int> secureRead, Action<int> insecureWrite, Func<bool, int> insecureRead)
        {
            ////////////////////////////////////////////////
            // TEST SETUP
            ////////////////////////////////////////////////
            // How many entites? 
            // Maximum: ORM guidelines suggest breaking down into batches for over 500 entities
            // Minimum: Less than 50 entities can be misleading
            const int benchmarkSize = 500;
            float insecureTimeAvgWrite, secureTimeAvgWrite, insecureTimeAvgRead, secureTimeAvgRead;
            var sw = new Stopwatch();
            // Perform warmup to minimize cold-start latencies (eg. n/w connection setup etc)
            Warmup(wipeDb, secureWrite, insecureWrite, secureRead, insecureRead);
            // Reset Db to known state i.e. empty table
            wipeDb();


            ////////////////////////////////////////////////
            // Regular/non-secure mode without CipherDB
            ////////////////////////////////////////////////
            Console.WriteLine("Regular/Non-secure mode ...");
            // WRITE
            sw.Restart();
            insecureWrite(benchmarkSize);
            sw.Stop();
            insecureTimeAvgWrite = sw.ElapsedMilliseconds/(float) benchmarkSize;
            // READ
            sw.Restart();
            int numPlainEntries = insecureRead(true);
            sw.Stop();
            insecureTimeAvgRead = sw.ElapsedMilliseconds / (float)numPlainEntries;

            // Reset Db to known state i.e. empty table
            wipeDb();

            ////////////////////////////////////////////////
            // CipherDB
            ////////////////////////////////////////////////
            Console.WriteLine("CipherDB mode ...");
            // WRITE
            sw.Restart();
            secureWrite(benchmarkSize);
            sw.Stop();
            secureTimeAvgWrite = sw.ElapsedMilliseconds / (float)benchmarkSize;
            // READ
            sw.Restart();
            int numSecureEntries = secureRead(true);
            sw.Stop();
            secureTimeAvgRead = sw.ElapsedMilliseconds / (float)numSecureEntries;


            ////////////////////////////////////////////////
            // Reporting
            ////////////////////////////////////////////////
            Console.WriteLine("====================================================");
            Console.WriteLine("     BENCHMARK RESULTS over {0} database records", benchmarkSize);
            Console.WriteLine("     All times are (avg) milliseconds per entity");
            Console.WriteLine("\t\tReads\tWrites");
            // CipherDB
            Console.WriteLine("CipherDB\t{0:F2}\t{1:F2}", secureTimeAvgRead, secureTimeAvgWrite);
            // Regular/non-secure
            Console.WriteLine("Non-secure\t{0:F2}\t{1:F2}", insecureTimeAvgRead, insecureTimeAvgWrite);
            // Delta
            Console.WriteLine("Difference\t{0:F2}\t{1:F2}", secureTimeAvgRead - insecureTimeAvgRead, secureTimeAvgWrite-insecureTimeAvgWrite);

            if (secureTimeAvgRead - insecureTimeAvgRead < 1.0)
                // This is the common case
                Console.WriteLine("Read impact of entire security processing pipeline is less than a single millisecond on average!");
            else
            {
                // Rare situation
                Console.WriteLine("Read impact of entire security processing pipeline is {0:F2} milliseconds",
                    secureTimeAvgRead - insecureTimeAvgRead);

                // If here, something is really wrong. Likely free license or VS profiling
                if (secureTimeAvgRead - insecureTimeAvgRead > 10)
                {
                    Console.WriteLine("Are you using the free license?\n" +
                                      "For free licenses, the security framework limits performance to 5 " +
                                      "transactions per second. Benchmarking the free license is not meaningful.\n" +
                                      "Thank you");
                }
            }
        }

        private void Warmup(Action wipeDb, Action<int> secureWrite, Action<int> insecureWrite, Func<bool, int> secureRead, Func<bool, int> insecureRead)
        {
            const int warmUpSize = 2;
            insecureWrite(warmUpSize);
            insecureRead(false);
            wipeDb(); // needed since writes to secure tables without CipherDB will fail decryption/reads

            secureWrite(warmUpSize);
            secureRead(false);            
        }
    }
}