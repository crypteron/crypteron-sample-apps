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
using Crypteron.CipherCore.Entropy;

namespace Crypteron.SampleApps.CommonCode
{
    public class UserRandomizer : Randomizer
    {
        readonly string[] _items = { "Apples", "Ball", "Cellphone", "DVD player", "Ear covers", "Firewood", "Glasses", "Headset", 
                             "Ink", "Jam", "Keyboard","Lens Cover", "Magazine", "Notebook", "Oranges", "Pail",  
                             "Quiver", "Raincoat", "Saddle", "Truffles", "Underwater camera", "Visor", "Xbox", "Yams", "Zebra"};

        readonly string[] _names = { "Albert", "Beethoven", "Cherry", "Derrek", "Einstein", "Frank", "Gina", "Hank", "Igor", "Janice", 
                             "Kelly", "Lisa", "Mike", "Nancy", "Olga", "Perry", "Quincy", "Randy", "Sid", "Tracy", "Ursula", 
                             "Victoria", "Wendy", "Xin", "Yanni", "Zack" };

        public string GetRandomItem()
        {
            return GetRandom(_items);
        }

        public string GetRandomNames()
        {
            return GetRandom(_names);
        }

        public string GetRandomPIN()
        {
            return String.Format("{0}-{1:D4}", GetRandom(new[] { "Alpha", "Bravo", "Charlie", "Echo" }),
                GetRandom(10000));
        }

        public string GetRandomSSN()
        {
            return String.Format("{0:D3}-{1:D2}-{2:D4}", GetRandom(1000), GetRandom(100), GetRandom(10000));
        }

        public string GetRandomCC()
        {
            return String.Format("{0:D4}-{1:D4}-{2:D4}-{3:D4}", GetRandom(10000),
                GetRandom(10000),
                GetRandom(10000),
                GetRandom(10000));
        }
    }
}
