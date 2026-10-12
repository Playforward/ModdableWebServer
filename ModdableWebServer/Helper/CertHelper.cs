using System;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using NetCoreServer;

namespace ModdableWebServer.Helper;

public static class CertHelper
{
	public static X509Certificate GetCert(string pfxPath, string password)
	{
		if (string.IsNullOrEmpty(pfxPath))
		{
			throw new ArgumentNullException("pfxPath");
		}
		if (!File.Exists(pfxPath))
		{
			throw new FileNotFoundException(pfxPath + " File not found");
		}
		return X509CertificateLoader.LoadPkcs12FromFile(pfxPath, password);
	}

	public static X509Certificate GetCertPem(string certPath, string keyPath)
	{
		if (string.IsNullOrEmpty(certPath))
		{
			throw new ArgumentNullException("certPath");
		}
		if (string.IsNullOrEmpty(keyPath))
		{
			throw new ArgumentNullException("keyPath");
		}
		if (!File.Exists(certPath))
		{
			throw new FileNotFoundException(certPath + " File not found");
		}
		if (!File.Exists(keyPath))
		{
			throw new FileNotFoundException(keyPath + " File not found");
		}
		X509Certificate2 x509Certificate = X509Certificate2.CreateFromPemFile(certPath, keyPath);
		byte[] data = x509Certificate.Export(X509ContentType.Pfx);
		x509Certificate.Dispose();
		return X509CertificateLoader.LoadCertificate(data);
	}

	public static SslContext GetContext(SslProtocols sslprotocol, string pfxPath, string password)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		return new SslContext(sslprotocol, GetCert(pfxPath, password));
	}

	public static SslContext GetContextNoValidate(SslProtocols sslprotocol, string pfxPath, string password)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		return new SslContext(sslprotocol, GetCert(pfxPath, password), (RemoteCertificateValidationCallback)NoCertificateValidator);
	}

	public static SslContext GetContextPem(SslProtocols sslprotocol, string certPath, string keyPath)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		return new SslContext(sslprotocol, GetCertPem(certPath, keyPath));
	}

	public static SslContext GetContextPamNoValidate(SslProtocols sslprotocol, string certPath, string keyPath)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		return new SslContext(sslprotocol, GetCertPem(certPath, keyPath), (RemoteCertificateValidationCallback)NoCertificateValidator);
	}

	public static bool NoCertificateValidator(object? sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
	{
		return true;
	}
}
