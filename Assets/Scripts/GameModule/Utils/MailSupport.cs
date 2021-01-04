using UnityEngine;
using System;

public class MailSupport
{
    public static void RunMail(string email, string subject, string body)
	{
		Uri uri = new Uri(string.Format("mailto:{0}?subject={1}&body={2}", email, subject, body));
		Application.OpenURL(uri.AbsoluteUri);
	}
}