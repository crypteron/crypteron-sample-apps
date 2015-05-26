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
using Crypteron.CipherDb;

namespace Crypteron.SampleApps.ConsoleCipherDbEfCodeFirst.Models
{
    public partial class User
    {
        public int OrderId { get; set; }
        
        public string OrderItem { get; set; }
        
        public DateTime? Timestamp { get; set; }
        
        // For Code-first one can additionally mark properties as secure
        // without renaming the property itself. Unlike the Db-First sample
        // in this Code-first sample app, we also encrypt CustomerName
        [Secure]
        public string CustomerName { get; set; }

        public string Secure_CreditCardNumber { get; set; }
        
        public byte[] Secure_SocialSecurityNumber { get; set; }
        
        public string Secure_LegacyPIN { get; set; }
    }
}
