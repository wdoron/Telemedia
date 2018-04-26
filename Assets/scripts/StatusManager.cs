using UnityEngine;
using System.Collections;
using System.Xml;
//using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;



public class StatusManager : MonoBehaviour {
    
    public static StatusManager Instance;

    public TextAsset TemplateStatus;
    private XmlDocument CurrnetStatusXml;

    private XmlNodeList StatusPages;
    private XmlNode StatusNode;
    private XmlNode ClientNode;
    
    public StatusManager() {
        Instance = this;
    }

    private string SessionsFolderPath;

    private List<IStatusDependant> RegisteredPages = new List<IStatusDependant>();

    public void Init() {

        SessionsFolderPath = Application.persistentDataPath + "/sesss/";
        //print(SessionsFolderPath);

        if (!Directory.Exists(SessionsFolderPath)){
            Directory.CreateDirectory(SessionsFolderPath);
        }


        ResetCurrentStatus();
    }

    public void ResetCurrentStatus(){

        CurrnetStatusXml = new XmlDocument();
        CurrnetStatusXml.LoadXml(TemplateStatus.text);

        StatusNode = CurrnetStatusXml.GetElementsByTagName("status")[0];
        ClientNode = CurrnetStatusXml.GetElementsByTagName("client")[0];

        foreach (IStatusDependant page in RegisteredPages) {
            page.ResetStatus();
        }


        XmlNode detailNode = CurrnetStatusXml.CreateNode(XmlNodeType.Element, "firstname", "");
        detailNode.InnerText = "";
        detailNode.Attributes.Append(CurrnetStatusXml.CreateNode(XmlNodeType.Attribute, "mailText", "") as XmlAttribute);
        detailNode.Attributes.GetNamedItem("mailText").Value = "Name";
        ClientNode.AppendChild(detailNode);

        detailNode = CurrnetStatusXml.CreateNode(XmlNodeType.Element, "surname", "");
        detailNode.InnerText = "";
        detailNode.Attributes.Append(CurrnetStatusXml.CreateNode(XmlNodeType.Attribute, "mailText", "") as XmlAttribute);
        detailNode.Attributes.GetNamedItem("mailText").Value = "Surname";
        ClientNode.AppendChild(detailNode);

        detailNode = CurrnetStatusXml.CreateNode(XmlNodeType.Element, "title", "");
        detailNode.InnerText = "";
        detailNode.Attributes.Append(CurrnetStatusXml.CreateNode(XmlNodeType.Attribute, "mailText", "") as XmlAttribute);
        detailNode.Attributes.GetNamedItem("mailText").Value = "Title";
        ClientNode.AppendChild(detailNode);


        detailNode = CurrnetStatusXml.CreateNode(XmlNodeType.Element, "mail", "");
        detailNode.InnerText = "";
        detailNode.Attributes.Append(CurrnetStatusXml.CreateNode(XmlNodeType.Attribute, "mailText", "") as XmlAttribute);
        detailNode.Attributes.GetNamedItem("mailText").Value = "eMail";
        ClientNode.AppendChild(detailNode);


        detailNode = CurrnetStatusXml.CreateNode(XmlNodeType.Element, "phoneNumber", "");
        detailNode.InnerText = "";
        detailNode.Attributes.Append(CurrnetStatusXml.CreateNode(XmlNodeType.Attribute, "mailText", "") as XmlAttribute);
        detailNode.Attributes.GetNamedItem("mailText").Value = "Phone";
        ClientNode.AppendChild(detailNode);

        detailNode = CurrnetStatusXml.CreateNode(XmlNodeType.Element, "aircraftType", "");
        detailNode.InnerText = "";
        detailNode.Attributes.Append(CurrnetStatusXml.CreateNode(XmlNodeType.Attribute, "mailText", "") as XmlAttribute);
        detailNode.Attributes.GetNamedItem("mailText").Value = "Aircraft type";
        ClientNode.AppendChild(detailNode);

        detailNode = CurrnetStatusXml.CreateNode(XmlNodeType.Element, "notes", "");
        detailNode.InnerText = "";
        detailNode.Attributes.Append(CurrnetStatusXml.CreateNode(XmlNodeType.Attribute, "mailText", "") as XmlAttribute);
        detailNode.Attributes.GetNamedItem("mailText").Value = "Notes";
        ClientNode.AppendChild(detailNode);

        
    }

    internal XmlNode GetPage(string ID) {
        StatusPages = CurrnetStatusXml.GetElementsByTagName("page");
        foreach (XmlNode statusPageData in StatusPages) {
            if (statusPageData.Attributes.GetNamedItem("id").Value == ID) {
                return statusPageData;
            }
        }

        XmlNode node = CurrnetStatusXml.CreateNode(XmlNodeType.Element, "page", "");
        XmlAttribute attr = CurrnetStatusXml.CreateAttribute("id");
        attr.Value = ID;
        node.Attributes.Append(attr);
        StatusNode.AppendChild(node);


        attr = CurrnetStatusXml.CreateAttribute("customData");
        attr.Value = "";
        node.Attributes.Append(attr);

        StatusPages = CurrnetStatusXml.GetElementsByTagName("page");

        return node;

    }

    internal XmlNode GetStatus(XmlNode pageStatus, string propName, string defaultVal) {
        foreach (XmlNode prop in pageStatus) {
            if (prop.Attributes.GetNamedItem("caption").Value == propName) {
                return prop;
            }
        }

        //node does not exists
        XmlNode node = CurrnetStatusXml.CreateNode(XmlNodeType.Element, "status", "");

        node.Attributes.SetNamedItem(CurrnetStatusXml.CreateNode(XmlNodeType.Attribute, "caption", ""));
        node.Attributes.GetNamedItem("caption").Value = propName;

        node.Attributes.SetNamedItem(CurrnetStatusXml.CreateNode(XmlNodeType.Attribute, "value", ""));
        node.Attributes.GetNamedItem("value").Value = defaultVal;

        pageStatus.AppendChild(node);

        return node;
    }

    internal bool TryToMailStatus(string email, string title) {
        XmlNode agentMailNode = CurrnetStatusXml.CreateNode(XmlNodeType.Element, "agentMail", "");
        agentMailNode.InnerText = email;
        ClientNode.AppendChild(agentMailNode);
        XmlNode mailHeaderNode = CurrnetStatusXml.CreateNode(XmlNodeType.Element, "mailHeader", "");
        mailHeaderNode.InnerText = title;
        ClientNode.AppendChild(mailHeaderNode);

        XmlNode detailNode = CurrnetStatusXml.CreateNode(XmlNodeType.Element, "date", "");
        detailNode.InnerText = "";
        detailNode.Attributes.Append(CurrnetStatusXml.CreateNode(XmlNodeType.Attribute, "mailText", "") as XmlAttribute);
        detailNode.Attributes.GetNamedItem("mailText").Value = "Date";

        detailNode.InnerText = String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now);

        ClientNode.AppendChild(detailNode);

        string filename = SaveLocally(true);

        bool res = SendSingleMail(email, title, CurrnetStatusXml);

        if (res) {
            File.Delete(SessionsFolderPath + filename);
        } else {
        }
        return res;

    }

    internal void TryToMailUnsent() {
        
        string[] files = Directory.GetFiles(SessionsFolderPath);

        for (int i = 0; i < files.Length; i++) {
            if (files[i].Substring(SessionsFolderPath.Length, 1) == "C") {
                XmlDocument doc = new XmlDocument();
                doc.Load(files[i]);
                string mail = doc.GetElementsByTagName("agentMail")[0].InnerText;
                string header = doc.GetElementsByTagName("mailHeader")[0].InnerText;
                print(mail + " " + header);

                bool res = SendSingleMail(mail, header, doc);

                if (res) {
                    File.Delete(files[i]);
                }
                break;
            }
        }
    }

    private bool SendSingleMail(string email, string title, XmlDocument srcDoc){
        
        string body = title + "\n" + ConstractMailBody(srcDoc);
       
        /*MailMessage mail = new MailMessage();
        mail.From = new MailAddress("ruag.ba.app@gmail.com");
        mail.To.Add(email);
        mail.Subject = "Meeting Summary";
        mail.Body = body;

        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
        //SmtpServer.Port = 587;
        SmtpServer.Port = 25;
        SmtpServer.Credentials = new NetworkCredential("ruag.ba.app@gmail.com", "ruagapp2014") as ICredentialsByHost;
        SmtpServer.UseDefaultCredentials = false;
        SmtpServer.Timeout = 20000;
        SmtpServer.EnableSsl = false;

        ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };


        try {
            SmtpServer.Send(mail);

            print("Mail sent");
            return true;
        } catch (Exception ex) {
            print("Exception caught: " + ex.ToString());
            return false;
        }*/



        /*body = body.Replace(new String((char)20, 1), "%20");
        body = body.Replace("\n", "%0D%0A");
        body = body.Replace(new String((char)0x0D, 1), "%0D");
        body = body.Replace(new String((char)0x0A, 1), "%0A");
        body = body.Replace(new String((char)0x26, 1), "%26");*/

        print("Shimon - Body constructed");


        ConnectionTesterStatus connectionTestResult = Network.TestConnection();
        print("Shimon - Testconection: " + connectionTestResult);

        WebClient client = null;
        Stream stream = null;
        
        try {

            client = new WebClient();
            
            stream = client.OpenRead("http://" + WWW.EscapeURL("www.google.com"));
            print("Shimon - connection test ok");

        } catch (Exception ex) {
            print("Shimon - Connection Exception caught: " + ex.ToString());
        } finally {
            if (client != null) client.Dispose();
            if (stream != null) stream.Dispose();
        }

        print("Shimon - Sending mail");

        try {
            string escapedBody = WWW.EscapeURL(body);

            escapedBody = escapedBody.Replace("+", "%20");

            Application.OpenURL("mailto:" + email + "?subject=Meeting%20Summary&body=" + escapedBody);
            print("Shimon - mail sending ok");
            return true;
        } catch (Exception ex) {
            print("Shimon - Mail Exception caught: " + ex.ToString());
            return false;
        } 




    }

    internal string SaveLocally(bool isComplete) {

        string firstName = CurrnetStatusXml.GetElementsByTagName("firstname")[0].InnerText;
        firstName = Regex.Replace(firstName, @"[^a-zA-Z0-9 ]", "");

        string surName = CurrnetStatusXml.GetElementsByTagName("surname")[0].InnerText;
        surName = Regex.Replace(surName, @"[^a-zA-Z0-9 ]", "");

        string targetFilename = ((isComplete) ? "C" : "I") + String.Format("{0:dd-MM-yyyy_HH-mm-ss}", DateTime.Now) + firstName + " " + surName + ".xml";

        CurrnetStatusXml.Save(SessionsFolderPath + targetFilename);

        return targetFilename;


        /*DateTime latest = DateTime.MinValue;
        FileInfo latestInfo = null;

        foreach (FileInfo fi in files) {
            string currFilename = fi.Name.Substring(0, fi.Name.Length - 4);
            bool isValidTest = currFilename.Contains("_C_");

            if (isValidTest) {
                string format = "yyyy-MM-dd HH-mm";

                string[] dateStrings = currFilename.Split('_');

                DateTime date = DateTime.ParseExact(dateStrings[2] + " " + dateStrings[3], format, null);
                if (latest == DateTime.MinValue || DateTime.Compare(latest, date) > 0) {
                    latest = date;
                    latestInfo = fi;
                }
            }
        }*/
    }

    public SessionInfo[] GetFileList() {
        string[] files = Directory.GetFiles(SessionsFolderPath);

        SessionInfo[] retSessions = new SessionInfo[files.Length];

        for (int i = 0; i < files.Length; i++) {
            retSessions[i] = new SessionInfo();
            retSessions[i].IsComplete = files[i].Substring(SessionsFolderPath.Length, 1) == "C";

            int initDataLen = 20;

            string name = files[i].Substring(SessionsFolderPath.Length + initDataLen, files[i].Length - SessionsFolderPath.Length - initDataLen - 4); //-4 is the .xml suffix
            string date = files[i].Substring(SessionsFolderPath.Length + 1, 10);

            date.Replace("-"[0], "/"[0]);

            retSessions[i].Customer = name;
            retSessions[i].Date = date;
        }

        return retSessions;
    }

    internal void RegisterPage(IStatusDependant page) {
        RegisteredPages.Add(page);
    }

    internal ClientDetails GetClientDetails() {
        ClientDetails retVal =  new ClientDetails();

        foreach(XmlNode node in ClientNode){
            switch (node.Name) {
                case "firstname":
                    retVal.FirstName = node.InnerText;
                    break;
                case "surname":
                    retVal.Surname = node.InnerText;
                    break;
                case "title":
                    retVal.Title = node.InnerText;
                    break;
                case "mail":
                    retVal.Mail = node.InnerText;
                    break;
                case "phoneNumber":
                    retVal.PhoneNumber = node.InnerText;
                    break;
                case "aircraftType":
                    retVal.AircraftType = node.InnerText;
                    break;
                case "notes":
                    retVal.Notes = node.InnerText;
                    break;
            }
        }


        return retVal;
    }

    internal void SetClientDetails(ClientDetails details) {
        foreach (XmlNode node in ClientNode) {
            switch (node.Name) {
                case "firstname":
                    node.InnerText = details.FirstName;
                    break;
                case "surname":
                    node.InnerText = details.Surname;
                    break;
                case "title":
                    node.InnerText = details.Title;
                    break;
                case "mail":
                    node.InnerText = details.Mail;
                    break;
                case "phoneNumber":
                    node.InnerText = details.PhoneNumber;
                    break;
                case "aircraftType":
                    node.InnerText = details.AircraftType;
                    break;
                case "notes":
                    node.InnerText = details.Notes;
                    break;
            }
        }
    }

    private string ConstractMailBody(XmlDocument srcDoc) {
        string str = "Client Details:\n";

        foreach (XmlNode detail in srcDoc.GetElementsByTagName("client")[0]) {
            if (detail.InnerText != "" && detail.Name != "agentMail" && detail.Name != "mailHeader") {
                str += detail.Attributes.GetNamedItem("mailText").Value + ": " + detail.InnerText + "\n";
            }
        }


        str += "---------------------------------\n\nSelected Items:\n\n";

        string[][] status = getFullStatus(srcDoc.GetElementsByTagName("status")[0]);

        foreach(string[] statType in status){
            if (statType.Length > 1) {
                str = str + statType[0] + "\n\n"; //title
                for (int i = 1; i < statType.Length; i++) {
                    str = str + statType[i] + "\n";
                }
            }

            str = str + "\n"; 
            
        }

        str += "---------------------------------";

        return str;
    }

    internal string[][] getFullStatus(XmlNode srcNode = null) {

        if (srcNode == null) {
            srcNode = StatusNode;
        }

        Dictionary<string, List<string>> pagesByOwner = new Dictionary<string, List<string>>();

        foreach (XmlNode page in srcNode) {

            List<string> currentList = null;;

            foreach(IStatusDependant regPage in RegisteredPages){
                string owner = "";
                if (page.Attributes.GetNamedItem("id").Value == regPage.getId()) {
                    owner = regPage.getOwner();

                    if (pagesByOwner.ContainsKey(owner)) {
                        currentList = pagesByOwner[owner];
                    }else{
                        currentList = new List<string>();
                        pagesByOwner[owner] = currentList;
                    }

                    break;
                }
            }

            foreach (XmlNode selectedItem in page) {
                if (bool.Parse(selectedItem.Attributes.GetNamedItem("value").Value)){
                    string caption = selectedItem.Attributes.GetNamedItem("caption").Value;
                    if (caption != "_") {
                        currentList.Add(caption);
                    } else {
                        currentList.Add(page.Attributes.GetNamedItem("customData").Value);
                    }
                }
            }

        }


        string[][] retVal = new string[pagesByOwner.Keys.Count][];

        int keyCount = 0;

        foreach(KeyValuePair<string, List<string>> pair in pagesByOwner){
            retVal[keyCount] = new string[pair.Value.Count + 1];

            retVal[keyCount][0] = pair.Key;
            int itemcount = 1;
            foreach(string itemName in pair.Value){
                retVal[keyCount][itemcount] = itemName;
                itemcount++;
            }

            keyCount++;
        }

        return retVal;
    }

    internal void LoadStatus(int index) {
        string[] files = Directory.GetFiles(SessionsFolderPath);

        int unfinishCount = 0;

        for (int i = 0; i < files.Length; i++) {
            if (files[i].Substring(SessionsFolderPath.Length, 1) == "I") {
                if (index == unfinishCount) {

                    CurrnetStatusXml.Load(files[i]);
                    StatusNode = CurrnetStatusXml.GetElementsByTagName("status")[0];
                    ClientNode = CurrnetStatusXml.GetElementsByTagName("client")[0];

                    //string mail = doc.GetElementsByTagName("agentMail")[0].InnerText;
                    //string header = doc.GetElementsByTagName("mailHeader")[0].InnerText;
                    //print(mail + " " + header);

                    foreach (XmlNode page in StatusNode) {

                        foreach (IStatusDependant regPage in RegisteredPages) {
                            if (page.Attributes.GetNamedItem("id").Value == regPage.getId()) {
                                //regPage.UpdateFromStatusNode(page);
                                regPage.ResetStatus();

                            }
                        }
                    }


                    File.Delete(files[i]);
                } else {
                    unfinishCount++;
                }

                break;
            }
        }
    }
}

public class SessionInfo {
    public string Date;
    public string Customer;
    public bool IsComplete;
}

public class ClientDetails {
    public string FirstName = "";
    public string Surname = "";
    public string Title = "";
    public string Mail = "";
    public string PhoneNumber = "";
    public string AircraftType = "";
    public string Notes = "";
}

public interface IStatusDependant {
    void ResetStatus();

    string getId();

    string getOwner();

    //void UpdateFromStatusNode(XmlNode page);
}