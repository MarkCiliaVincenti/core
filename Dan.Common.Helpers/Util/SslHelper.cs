﻿namespace Dan.Common.Helpers.Util;

/// <summary>
/// SSL helper methods
/// </summary>
public static class SslHelper
{
    /// <summary>
    /// Validates a certificate for use with Altinn service owners and returns the organization number
    /// </summary>
    /// <param name="certificate">The Certificate</param>
    /// <param name="verifyCertificateChain">If the certificate should be verified</param>
    /// <returns>The organization number. Throws on error.</returns>
    public static string GetValidOrgNumberFromCertificate(X509Certificate2 certificate, bool verifyCertificateChain = false)
    {
        if (!verifyCertificateChain) return GetOrgFromCertificate(certificate);

        var chain = new X509Chain();
        chain.Build(certificate);

        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;

        foreach (X509ChainElement element in chain.ChainElements)
        {
            if (!element.Certificate.Verify())
            {
                throw new CryptographicException("Unable to verify certificate chain");
            }
        }

        return GetOrgFromCertificate(certificate);
    }

    private static string GetOrgFromCertificate(X509Certificate2 certificate)
    {
        var certificateSubject = certificate.Subject;
        var orgNumber = string.Empty;
        if (string.IsNullOrEmpty(certificateSubject))
        {
            throw new CryptographicException("No subject found on certificate");
        }

        var subjectList = certificateSubject.Split(',');

        foreach (var s in subjectList)
        {
            var kvp = s.Trim().Split('=');
            if (kvp.Length != 2 || !kvp[0].Equals("SERIALNUMBER")) continue;
            orgNumber = kvp[1].Trim();
            break;
        }

        if (!OrganizationNumberValidator.IsWellFormed(orgNumber))
        {
            throw new CryptographicException("Organization number on certificate was not well-formed");
        } 
        
        return orgNumber;
    }
}