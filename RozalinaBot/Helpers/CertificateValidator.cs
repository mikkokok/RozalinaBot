using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RozalinaBot.Helpers
{
    public static class CertificateValidator
    {
        private static readonly string TrustedThumbprint = AppLoader.LoadedConfig.OumanThumbPrint;

        public static bool ValidateSslCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            var certificate2 = new X509Certificate2(certificate);
            if (certificate2.Thumbprint != null && certificate2.Thumbprint.Equals(TrustedThumbprint, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
