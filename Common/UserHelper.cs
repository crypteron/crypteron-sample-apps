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
using System.Text;

namespace Crypteron.SampleApps.CommonCode
{
    public class UserHelper
    {

        public UserFrontEnd CreateRandomUser()
        {
            //var rng = new SecureRandom();

            // Create a new order
            var rndUser = new UserFrontEnd();
            var orderGuid = Guid.NewGuid();
            var r = new UserRandomizer();

            rndUser.InfoId = orderGuid;
            rndUser.PartId = GetPartId(orderGuid);

            rndUser.OrderItem = r.GetRandomItem();
            rndUser.CustomerName = r.GetRandomNames();
            rndUser.Timestamp = r.GetRandomTime();
            rndUser.Secure_CreditCardNumber = r.GetRandomCC();
            rndUser.Secure_LegacyPIN = r.GetRandomPIN();
            rndUser.Secure_SSN = r.GetRandomSSN();
            rndUser.Secure_MetadataDisplayField = "Overwritten by CipherDb book-keeping";

            return rndUser;
        }

        public UserFrontEnd TrimSize(UserFrontEnd input)
        {
            input.CustomerName = limitString(input.CustomerName, 64);
            input.OrderItem = limitString(input.OrderItem, 64);
            input.Secure_CreditCardNumber = limitString(input.Secure_CreditCardNumber, 64);
            input.Secure_LegacyPIN = limitString(input.Secure_LegacyPIN, 64);
            input.Secure_SSN = limitString(input.Secure_SSN, 64);
            return input;
        }

        public string PrintToString(UserFrontEnd o)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("PartId:{0}, OrderId:{1}, {2} got {3} at {4} using CC {5},SSN={6}, PIN={7}. Metadata={8}" + Environment.NewLine,
                o.PartId,
                o.InfoId,
                o.CustomerName,
                o.OrderItem,
                o.Timestamp,
                o.Secure_CreditCardNumber,
                o.Secure_SSN,
                o.Secure_LegacyPIN,
                o.Secure_MetadataDisplayField);

            sb.AppendLine("---------------------------------------------------------------");
            return sb.ToString();
        }

        public Tuple<int, Guid> GetId()
        {
            Console.Write("Select Order ID: ");
            Guid orderId;
            while (!Guid.TryParse(Console.ReadLine(), out orderId))
            {
                Console.WriteLine("Unable to parse, try again");
            }

            int partID = GetPartId(orderId);
            return new Tuple<int, Guid>(partID, orderId);
        }

        public int GetPartId(Guid guid)
        {
            var guidStr = guid.ToString().ToLowerInvariant();
            var guidLastChar = "0" + guidStr.Substring(guidStr.Length - 1, 1);
            var guidLastCharByte = Convert.ToInt32(guidLastChar, 16);
            var partId = guidLastCharByte % 2;
            //Console.WriteLine("Partion ID is: {0}", partId);
            return partId;
        }


        public UserFrontEnd AddOrEdit(UserFrontEnd editUser=null)
        {
            UserFrontEnd usr;
            if (editUser == null)
            {
                usr = new UserFrontEnd();
                var orderGuid = Guid.NewGuid();
                usr.InfoId = orderGuid;
                usr.PartId = GetPartId(orderGuid);
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

            Console.Write("SSN: [{0}]", usr.Secure_SSN);
            temp = Console.ReadLine();
            if (!String.IsNullOrEmpty(temp))
                usr.Secure_SSN = temp;

            Console.Write("Legacy PIN: [{0}]", usr.Secure_LegacyPIN);
            temp = Console.ReadLine();
            if (!String.IsNullOrEmpty(temp))
                usr.Secure_LegacyPIN = temp;

            usr.Timestamp = DateTime.Now;

            usr = TrimSize(usr);
            return usr;
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
