// Copyright 2001-2010 - Active Up SPRLU (http://www.agilecomponents.com)
//
// This file is part of MailSystem.NET.
// MailSystem.NET is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// MailSystem.NET is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace ActiveUp.Net.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class TrustAllCertificatePolicy : System.Net.ICertificatePolicy
    {
        /// <summary>
        /// 
        /// </summary>
        public TrustAllCertificatePolicy() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="cert"></param>
        /// <param name="req"></param>
        /// <param name="problem"></param>
        /// <returns></returns>
        public bool CheckValidationResult(ServicePoint sp,
            X509Certificate cert,
            WebRequest req,
            int problem)
        {
            return true;
        }
    }

    public static class SSLValidator
    {
        private static bool OnValidateCertificate(object sender,
            X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public static void OverrideValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                OnValidateCertificate;
            ServicePointManager.Expect100Continue = true;
        }
    }
}
