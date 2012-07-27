using System;
using System.Net.Mail;
using EmailSender;

namespace CommonLib
{
    public class EmailUtility
    {
        const string KEYNAME = "activate_url";
        static string prefix = Setting.Instance.GetSetting(KEYNAME);

        public static void SendActivateEmail(string InywhereId)
        {
            EMailItem item = new EMailItem();
            item.SenderAddress = @"info@inywhere.com";
            item.SenderName = "Inywhere Products Center";
            item.Priority = MailPriority.High;
            item.ReplyTo = "test@inywhere.com";
            //item.CC = new string[] { "yjddd412213@hotmail.com" };   //"kuo_ren@hotmail.com"
            //item.BCC = new string[] { "yjddd412213@163.com" };
            item.To = new string[] { InywhereId }; // TODO:data.InywhereId
            item.Subject = "Thanks For Registering Inywhere.";
            item.Body = string.Format("<p>Welcome to Inywhere Accounts. To activate your account and verify your email" +
                    "address, please click the following link:</p><p>{0}</p><br />" +

                    "<p>***NOTE*** </p>" +
                    "<p>If you've received this mail in error, it's likely that another user" +
                    "entered your email address while trying to create an account for a different" +
                    "email address. If you don't click the verification link, the account won't" +
                    "be activated.</p>" +

                     "<p>If clicking the link above does not work, copy and paste the URL in a new browser window instead.</p>" +
                     "<p>Sincerely,</p>" +
                     "<p>The Inywhere Accounts Team (www.inywhere.com)</p>",
                    prefix + SymmetricCryptoHelper.Instance.GetEncryptedValue(InywhereId)); // TODO:

            item.EntityID = Guid.NewGuid().ToString();
            // item.Callback = new SendMailCallback(OnSent); // TODO: Define Send callback 
            //item.Context = context; // TODO: context can be used for send callback processing.
            item.IsHtmlBody = false;

            //MailSender.Instance.Send(item);

            SendGridMailSender.SendMail(item);
        }
    }
}