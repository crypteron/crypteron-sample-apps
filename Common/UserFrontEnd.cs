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
using System.ComponentModel.DataAnnotations;

namespace Crypteron.SampleApps.CommonCode
{
    public class UserFrontEnd
    {
        [Range(0, 1)]
        public int PartId { get; set; }

        //[Range(0, Int32.MaxValue)]
        public Guid InfoId { get; set; }

        [StringLength(32, ErrorMessage = "{0} must be under {1} characters")]        
        public string OrderItem { get; set; }
        
        public DateTime? Timestamp { get; set; }

        [StringLength(32, ErrorMessage = "{0} must be under {1} characters")]        
        public string CustomerName { get; set; }

        [StringLength(20, ErrorMessage = "{0} must be under {1} characters")]        
        public string Secure_CreditCardNumber { get; set; }

        [StringLength(12, ErrorMessage = "{0} must be under {1} characters")]
        public string Secure_LegacyPIN { get; set; }

        [StringLength(60, ErrorMessage = "{0} must be under {1} characters")]
        public string Secure_MetadataDisplayField { get; set; }
        
        [StringLength(12, ErrorMessage = "{0} must be under {1} characters")]
        public string Secure_SSN { get; set; }
    }
}