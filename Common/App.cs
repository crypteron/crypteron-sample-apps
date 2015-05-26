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
using System.Globalization;

namespace Crypteron.SampleApps.CommonCode
{
    public class App<T> where T : IProcessCustomer, new()
    {
        public void Run()
        {
            var cust = new T();
            var ongoing = true;

            while (ongoing)
            {
                PrintMenu();
                // Get char in lower case, skipping whitespaced chars
                var userInput = Console.ReadKey().KeyChar.ToString(CultureInfo.InvariantCulture).Trim().ToLowerInvariant();
                if (userInput.Length > 0)
                {
                    var option = userInput.ToCharArray()[0];
                    Console.WriteLine(" ...");
                    switch (option)
                    {
                        case 'r':
                            cust.ReadAll();
                            break;
                        case 'c':
                            cust.Create();
                            break;
                        case '+':
                            cust.CreateAuto(1);
                            break;
                        case 'f':
                            cust.CreateAuto(10);
                            break;
                        case 'u':
                            cust.Update();
                            break;
                        case 'd':
                            cust.Delete();
                            break;
                        case '!':
                            cust.WipeAllViaSql();
                            break;
                        case 'n':
                            cust.ReadAllInsecure();
                            break;
                        case 'b':
                            cust.Benchmark();
                            break;
                        case 's':
                            cust.StoredProcedure();
                            break;
                        case 'p':
                            cust.EncryptedSearch();
                            break;
                        case 't':
                            cust.Test();
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

        public void RunSingle()
        {
            var cust = new T();
            cust.Benchmark();
        }

        private void PrintMenu()
        {
            Console.WriteLine("====================================================");
            Console.WriteLine("R : Read all customer orders in table");
            Console.WriteLine("C : Create a new customer order manually via the keyboard");
            Console.WriteLine("+ : Auto-create a new customer order");
            Console.WriteLine("F : Auto-create 10 customer orders");
            Console.WriteLine("U : Update an existing customer order");
            Console.WriteLine("D : Delete a particular customer order in table");
            Console.WriteLine("! : Wipe all customer orders via direct SQL");
            Console.WriteLine("N : No CipherDB => data is secure/undecipherable");
            Console.WriteLine("B : Run secure vs insecure benchmark (Commercial licenses only)");
            Console.WriteLine("S : Run a stored procedure via CipherDB");
            Console.WriteLine("P : Perform a partial search over encrypted entities");
            //Console.WriteLine("T : Test");
            Console.WriteLine("Q : Quit");

            Console.WriteLine("\nNOTE: Sample/test database is a LocalDb SQL database (see app.config file) that gets overwritten when run from Visual Studio. To witness persistence, run the sample .exe from \\bin\\debug or \\bin\\Release folders directly");
            Console.Write("\nEnter your selection: ");
        }
    }
}
