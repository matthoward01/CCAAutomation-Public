using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;

namespace CCAAutomation.Lib
{
    public class Email
    {
        private ImapClient client = new();

        public static void ReadEmails()
        {
            var messages = new List<string>();
            using (var client = new ImapClient())
            {
                Console.WriteLine("--------------------------------");

                Settings.MailSettings mailSettings = Settings.GetMailSettings();

                client.Connect(mailSettings.MailServer, mailSettings.Port, mailSettings.SSL);

                client.Authenticate(mailSettings.User, mailSettings.Password);

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);
                var results = inbox.Search(SearchOptions.All, SearchQuery.Not(SearchQuery.Seen));
                foreach (var uniqueId in results.UniqueIds)
                {
                    var message = inbox.GetMessage(uniqueId);
                    if (message.Subject.ToLower().Contains("insite"))
                    {
                        Console.WriteLine(message.From);
                        string plateId = message.Subject.Split('(', ')')[1];
                        Console.WriteLine(plateId);
                        bool isSoftSurface = false;
                        if (message.Subject.ToLower().Contains("ss_auto"))
                        {
                            isSoftSurface = true;
                        }
                        if (message.Subject.ToLower().Contains("approved"))
                        {
                            Console.WriteLine("Approved");
                            SqlMethods.SqlUpdateStatus(plateId, "Approved", isSoftSurface);
                        }
                        else if (message.Subject.ToLower().Contains("rejected pages") || message.Subject.ToLower().Contains("requested corrections"))
                        {
                            Console.WriteLine("Rejected");
                            SqlMethods.SqlUpdateStatus(plateId, "Rejected", isSoftSurface);

                            List<string> textBody = message.TextBody.Replace("\r", "").Split('\n').ToList();
                            int commentIndex = textBody.FindIndex(s => s.EqualsString("Comments"));
                            if (commentIndex != -1)
                            {
                                string change = textBody[commentIndex + 1];
                                Console.WriteLine(change);
                                SqlMethods.SqlUpdateChange(plateId, change, isSoftSurface);
                            }
                        }
                        else if (message.Subject.ToLower().Contains("requested approval"))
                        {
                            Console.WriteLine("Waiting for Approval");
                            SqlMethods.SqlUpdateStatus(plateId, "Waiting for Approval", isSoftSurface);
                        }
                        //Console.WriteLine(message.Subject.Split('(', ')')[1]);
                        //Console.WriteLine(message.TextBody);
                    }

                    //Mark message as read
                    inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
                Console.WriteLine("--------------------------------");

            }
        }
    }
}
