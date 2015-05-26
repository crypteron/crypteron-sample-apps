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
using Crypteron.CipherCore.Entropy;

// This way to minimize code duplication
#if ConsoleCodeFirst
using Crypteron.SampleApps.ConsoleCipherDbEfCodeFirst.Models;
#endif

#if ConsoleDbFirst
using Crypteron.SampleApps.ConsoleCipherDbEfDbFirst;
#endif

namespace Crypteron.SampleApps.CommonCode
{
    public class ProcessCustomer : ProcessCustomerBase, IProcessCustomer
    {
        public void Create()
        {
            var dbUsr = CreateOrEdit();
            using (var secDb = new CipherDbContext())
            {
                secDb.Users.Add(dbUsr);
                secDb.SaveChanges();
            }
        }

        public void CreateAuto(int numToAdd)
        {
            using (var secDb = new CipherDbContext())
            {
                Console.Write("[CipherDB] Adding user records " + Environment.NewLine + "[");
                for (int i = 0; i < numToAdd; i++)
                {
                    secDb.Users.Add(CreateRandomUser());
                    Console.Write(".");
                }
                Console.WriteLine("]");
                secDb.SaveChanges();
            }
        }

        public void CreateAutoInsecure(int numToAdd)
        {
            using (var secDb = new PlainDbContext())
            {
                Console.Write("[Non-Secure] Adding user records " + Environment.NewLine + "[");
                for (int i = 0; i < numToAdd; i++)
                {
                    secDb.Users.Add(CreateRandomUser());
                    Console.Write(".");
                }
                Console.WriteLine("]");
                secDb.SaveChanges();
            }
        }

        public int ReadAll()
        {
            return ReadAll(true);
        }

        public int ReadAllInsecure()
        {
            return ReadAllInsecure(true);
        }

        public void Update()
        {
            using (var secDb = new CipherDbContext())
            {
                var id = GetId();
                var beforeUser = secDb.Users.Find(id);
                if (beforeUser != null)
                {
                    var afterUser = CreateOrEdit(beforeUser);
                    secDb.Entry(beforeUser).CurrentValues.SetValues(afterUser);
                    secDb.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Order ID {0} not found!", id);
                }
            }
        }

        public void Delete()
        {
            using (var secDb = new CipherDbContext())
            {
                var id = GetId();
                var deleteThis = secDb.Users.Find(id);
                if (deleteThis != null)
                {
                    secDb.Users.Remove(deleteThis);
                    secDb.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Order ID {0} not found!", id);
                }
            }
        }

        public void DeleteAll()
        {
            using (var secDb = new CipherDbContext())
            {
                int deleted = 0;
                foreach (var deleteThis in secDb.Users)
                {
                    secDb.Users.Remove(deleteThis);
                    deleted++;
                }
                
                var written = secDb.SaveChanges();
                Console.WriteLine("Deleted {0} entities, write to {1} entites", deleted, written);
            }
        }

        public void WipeAllViaSql()
        {
            // Deletions can be done directly without passing 
            // through CipherDB 
            const string rawSqlCmd = "DELETE FROM Users";
            using (var secDb = new CipherDbContext())
            {
                secDb.Database.ExecuteSqlCommand(rawSqlCmd);
            }

            Console.WriteLine("Wiped entire table via SQL command: {0}", rawSqlCmd);
        }
        
        public void LiveMigrate()
        {
            using (var secDbCtx = new CipherDbContext())
            {
                foreach (User o in secDbCtx.Users)
                {
                    // If CipherDB detects any rows still using the old
                    // encryption keys AND if Migration Policy allows it
                    // track all such older rows.
                }
                // To minimize write pressure, CipherDB will update old 
                // entities to newer the latest encryption key
                // ONLY when application generates a write, like below
                secDbCtx.SaveChanges();
            }
        }

        public void Benchmark()
        {
            base.Benchmark(WipeAllViaSql, CreateAuto, ReadAll, CreateAutoInsecure, ReadAllInsecure);
        }

        public void StoredProcedure()
        {
#if ConsoleDbFirst
            Console.WriteLine("Enter first few characters of customer name to search:");
            var custName = Console.ReadLine();

            using (var secDbCtx = new CipherDbContext())
            {
                // 1. The SQL Stored Proc needs to be imported into EntityFramework
                // 2. The resulting object from the SP needs to be mapped to an 
                //    EntityFramework entity
                var results = secDbCtx.usp_SearchUserByName(custName);
                foreach (var r in results)
                    DisplayEntity(r);
            }
#else
            throw new NotImplementedException("SPs work fine with CipherDB but currently demonstrated only for Db First");
#endif
        }

        public void EncryptedSearch()
        {
            // Since only the Application/CipherDB Agent can access the decrypted data,
            // the search must be performed withing CipherDB agent itself. For performance
            // reasons it's best to prefilter the search so the search query runs on a sub-set of 
            // potential dataset rather than the entire database table.

            Console.WriteLine("Enter the credit card search string:");
            var ccSearchStr = Console.ReadLine();
            using (var secDbCtx = new CipherDbContext())
            {
                // Prefilter off non-secure attributes (eg. City or Date etc), for demo we just use OrderId for prefilter
                var prefiltered = secDbCtx.Users.Where(user => user.OrderId > 3 && user.OrderId < 9).ToList();

                // Perform encrypted search with CipherDB Agent
                var hits = prefiltered.Where(user => user.Secure_CreditCardNumber.Contains(ccSearchStr));
                foreach (var o in hits)
                {
                    DisplayEntity(o);
                }
            }
        }

        public void Test()
        {
            // Placeholder for anything you want ...
        }

        private int ReadAll(bool printToScreen)
        {
            int totalEntries = 0;
            using (var secDbCtx = new CipherDbContext())
            {
                foreach (var o in secDbCtx.Users)
                {
                    totalEntries++;
                    if (printToScreen)
                        DisplayEntity(o);
                }
            }
            return totalEntries;
        }

        private int ReadAllInsecure(bool printToScreen)
        {
            Console.WriteLine("This is WRONG usage. Without CipherDB, data is secured and is undecipherable by anyone - even SQL server itself.");
            int totalEntries = 0;
            using (var secDbCtx = new PlainDbContext())
            {
                foreach (var o in secDbCtx.Users)
                {
                    totalEntries++;
                    if (printToScreen)
                        DisplayEntity(o, true);
                }
            }
            return totalEntries;
        }

        private void DisplayEntity(User o, bool rawBits = false)
        {
            string ssnString;

            if (o.Secure_SocialSecurityNumber == null)
            {
                ssnString = "null";
            }
            else
            {
                ssnString = rawBits
                                    ? Convert.ToBase64String(o.Secure_SocialSecurityNumber)
                                    : Encoding.UTF8.GetString(o.Secure_SocialSecurityNumber);
            }

            var sb = new StringBuilder();
            sb.AppendFormat("OrderId:{0}, {1} got {2} at {3} using CC {4},SSN={5}, PIN={6}." + Environment.NewLine,
                o.OrderId,
                o.CustomerName,
                o.OrderItem,
                o.Timestamp,
                o.Secure_CreditCardNumber,
                ssnString,
                o.Secure_LegacyPIN);

            sb.AppendLine("---------------------------------------------------------------");
            Console.Write(sb.ToString());
        }

        private int GetId()
        {
            Console.Write("Select Order ID: ");
            int orderId;
            while (!int.TryParse(Console.ReadLine(), out orderId))
            {
                Console.WriteLine("Unable to parse, try again");
            }
            return orderId;
        }

        private User CreateRandomUser()
        {
            var rndUser = new User();
            var r = new UserRandomizer();

            rndUser.OrderId = 0; //db overwrites this
            rndUser.OrderItem = r.GetRandomItem();
            rndUser.CustomerName = r.GetRandomNames();
            rndUser.Timestamp = Randomizer.GetRandomTime();
            rndUser.Secure_CreditCardNumber = r.GetRandomCC();
            rndUser.Secure_LegacyPIN = r.GetRandomPIN();
            rndUser.Secure_SocialSecurityNumber = Encoding.UTF8.GetBytes(r.GetRandomSSN());
            return rndUser;
        }

        private User CreateOrEdit(User editUser = null)
        {
            User usr;
            if (editUser == null)
            {
                usr = new User
                    {
                        OrderId = 0
                    };
            }
            else
            {
                usr = editUser;
            }

            Console.Write("Customer Name: [{0}]", usr.CustomerName);
            var temp = Console.ReadLine();
            if (!String.IsNullOrEmpty(temp))
                usr.CustomerName = temp;

            Console.Write("Item purchased: [{0}]", usr.OrderItem);
            temp = Console.ReadLine();
            if (!String.IsNullOrEmpty(temp))
                usr.OrderItem = temp;

            Console.Write("Credit Card number: [{0}]", usr.Secure_CreditCardNumber);
            temp = Console.ReadLine();
            if (!String.IsNullOrEmpty(temp))
                usr.Secure_CreditCardNumber = temp;

            Console.Write("SSN: [{0}]", usr.Secure_SocialSecurityNumber==null ? null : Encoding.UTF8.GetString(usr.Secure_SocialSecurityNumber));
            temp = Console.ReadLine();
            if (!String.IsNullOrEmpty(temp))
                usr.Secure_SocialSecurityNumber = Encoding.UTF8.GetBytes(temp);

            Console.Write("Legacy PIN: [{0}]", usr.Secure_LegacyPIN);
            temp = Console.ReadLine();
            if (!String.IsNullOrEmpty(temp))
                usr.Secure_LegacyPIN = temp;

            usr.Timestamp = DateTime.Now;

            usr = TrimSize(usr);
            return usr;
        }

        private User TrimSize(User input)
        {
            input.CustomerName = limitString(input.CustomerName, 64);
            input.OrderItem = limitString(input.OrderItem, 64);
            input.Secure_CreditCardNumber = limitString(input.Secure_CreditCardNumber, 64);
            input.Secure_LegacyPIN = limitString(input.Secure_LegacyPIN, 64);

            string temp;
            if (input.Secure_SocialSecurityNumber == null)
                temp = String.Empty;
            else
                temp = Encoding.UTF8.GetString(input.Secure_SocialSecurityNumber);
            
            input.Secure_SocialSecurityNumber = Encoding.UTF8.GetBytes(limitString(temp, 64));
            
            return input;
        }

        private string limitString(string inputStr, int lengthLimit)
        {
            if (inputStr != null)
            {
                var limit = Math.Min(inputStr.Length, lengthLimit);
                var output = inputStr.Substring(0, limit);
                return output;
            }

            return String.Empty;
        }
    }
}
