using EAGetMail;
using System;
using System.IO;
using Datos.SqlData;
using System.Text.RegularExpressions;

namespace RetrieveEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            string curpath = Directory.GetCurrentDirectory();
            string mailbox = String.Format("{0}\\inbox", curpath);

            if (!Directory.Exists(mailbox))
            {
                Directory.CreateDirectory(mailbox);
            }

            MailServer oServer = new MailServer("mail.resyst.cl", "daniel@resyst.cl", "9504975Ad*", ServerProtocol.Pop3);
            MailClient oClient = new MailClient("TryIt");

            // oServer.SSLConnection = true;
            // oServer.Port = 995;
            using (var bdd = new LoteriaGGEntities())
            {
                try
                {
                    oClient.Connect(oServer);
                    MailInfo[] infos = oClient.GetMailInfos();

                    foreach (var info in infos)
                    {
                        Console.WriteLine("Index: {0}; Size: {1}; UIDL: {2}",
                            info.Index, info.Size, info.UIDL);

                        // Receive email from POP3 server
                        Mail oMail = oClient.GetMail(info);

                        Console.WriteLine("From: {0}", oMail.From.ToString());
                        Console.WriteLine("Subject: {0}\r\n", oMail.Subject);

                        string sMonto = oMail.HtmlBody.Substring(oMail.HtmlBody.IndexOf('$', 0)).Split('$', '<')[1].Replace(" ", "").Replace(",", "").Replace(".", "");

                        int monto = int.Parse(sMonto);


                        TBL_MAILS mail = new TBL_MAILS
                        {
                            MAIL_FECHA = DateTime.Now,
                            MAIL_FROM = oMail.From.ToString(),
                            MAIL_SUBJECT = oMail.Subject,
                            MAIL_HTML = oMail.HtmlBody,
                            MAIL_MONTO = monto,
                        };

                        bdd.TBL_MAILS.Add(mail);
                        bdd.Entry(mail).State = System.Data.Entity.EntityState.Added;

                        bdd.SaveChanges();

                        // Mark email as deleted from POP3 server.
                        oClient.Delete(info);
                    }

                    oClient.Quit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
