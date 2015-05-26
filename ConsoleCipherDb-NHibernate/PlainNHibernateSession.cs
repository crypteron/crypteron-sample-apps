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

using Crypteron.SampleApps.ConsoleCipherDbNHibernate.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Crypteron.SampleApps.ConsoleCipherDbNHibernate
{
    // This is the only non-transparent interface into CipherDB and is used for
    // security related tasks and configuration. Beyond this, the rest of your 
    // your application code is unchanged
    public class PlainNHibernateSession
    {
        private static ISessionFactory _sessionFactory;
        private static Configuration _configuration;

        #region [NHibernate's general boilerplate code]

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        public static void DropAndGenerateSchema()
        {
            // This will create SQL statements (DDL) to export the XML
            // schema into the database. Used here for the creation of the
            // schema in database (it can do more than just creation though)
            new SchemaExport(Configuration).Execute(script: false, export: true, justDrop: false);
        }


        private static Configuration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = new Configuration();
                    _configuration.Configure();
                    _configuration.AddAssembly(typeof(NUser).Assembly);
                }
                return _configuration;
            }
        }

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    // very expensive, run just once!
                    // Even on a local SQLCE it can take 400ms (cold run) 
                    // to 40 ms (warm run i.e. blind rerun method)
                    _sessionFactory = Configuration.BuildSessionFactory();
                }

                return _sessionFactory;
            }
        }

        #endregion
    }
}
