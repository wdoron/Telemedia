using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using System.Collections.Generic;
using System;

public class Management : MonoBehaviour {
    public static Management Instance;

    public Texture BGTtexture;
    public Texture2D SliderTtexture;
    public Texture2D TransparentText;

    private SessionInfo[] CurrentSessions;
    private StatusManager StatusMgr;

    private SRGUIContainer cont;

    private SRGUIButton slider;
    //private bool IsMouseDown = false;

    public bool IsOpen = false;

    private int _FoldState = -1;

    private List<SRGUIInput> MarkedInputs = new List<SRGUIInput>();
    private List<SRGUILabel> MarkedTags = new List<SRGUILabel>();

    public delegate void NavigationCallback(string targetID, UnityEngine.Object extraParams, string extraData);
    public event NavigationCallback OnNavigate;

    private SRGUIButton MeetingSummaryButt;
    private SRGUIButton ClientDetailsButt;
    private SRGUIButton EndMeetingButt;
    private SRGUIButton LastMeetingsButt;


    //client data
    private SRGUIContainer ClientDetailsCont;
    private SRGUIInput ClientDetailsFirstName;
    private SRGUIInput ClientDetailsSurname;
    private SRGUIInput ClientDetailsTitle;
    private SRGUIInput ClientDetailsMail;
    private SRGUIInput ClientDetailsPhone;
    private SRGUIInput ClientDetailsAircraft;
    private SRGUIInput ClientDetailsNotes;
    private SRGUIButton TakePictureButt;
    private SRGUIButton SaveClientButt;
    public Texture ClientBackText;

	private WebCamTexture webCamTexture;
    private SRGUITexture camTexture;
    private Texture2D photo;

    //end meeting
    private SRGUIContainer EndMeetingCont;
    private SRGUILabel SendToHint;
    private SRGUIInput SendToInput;
    private SRGUICheckButton RememberMeCheck;
    private SRGUIInput SendtitleInput;
    private SRGUIButton SendMeetingButt;
    private SRGUIButton SaveMeetingButt;
    public Texture MeetingBackText;
    
    //last meetings
    private SRGUIContainer LastSessionsCont;
    private const int TotalMettingFields = 5;
    private SRGUIContainer[] SessionDescriptionConts;
    private SRGUIButton[] ReopenExistingMeetingButts;
    private SRGUILabel[] SessionDescriptionLabels;
    private int meetingPage = 0;

    private SRGUILabel PageLabel;
    private SRGUIButton NextPageButt;
    private SRGUIButton PrevPageButt;
    public Texture LastSessionTexture;

    //unsent meetings
    private SRGUIContainer UnsentSessionsCont;
    private SRGUILabel UnsentSessionsLabel;
    public Texture2D DotTexture;
    private SRGUIButton SendAllButt;
    public Texture2D SendAllText;

    private const string DEFAULT_MAIL_ADRESS = "DefaultMailAdress";

    public Management() {
        Instance = this;
    }

	void Awake(){
		if(webCamTexture == null) webCamTexture = new WebCamTexture();
	}

    internal void Init() {
		if(webCamTexture == null) webCamTexture = new WebCamTexture();
        StatusMgr = StatusManager.Instance;

        cont = new SRGUIContainer();
        
        slider = new SRGUIButton();
        slider.GroupID = 0;
        slider.Style = CommonAssetHolder.instance.GetCustomStyle(SliderTtexture, CommonAssetHolder.FontNameType.FrutiGray, 10);
        slider.Position = new Vector2(BGTtexture.width + 10, 0);
        cont.children.Add(slider);

        SRGUIManager.instance.Click += ClickReaction;

        SRGUITexture bgTexture = new SRGUITexture();
        bgTexture.SetTexture(BGTtexture);
        cont.children.Add(bgTexture);


        cont.Position = new Vector2(-BGTtexture.width, 0);
        SRGUIManager.instance.RegisterGUIElement(cont, gameObject);

        //meeting summary
        MeetingSummaryButt = new SRGUIButton();
        MeetingSummaryButt.GroupID = 0; 
        MeetingSummaryButt.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 30);
        MeetingSummaryButt.Text = "Meeting Summary";
        MeetingSummaryButt.Position = new Vector2(80, 80);
        MeetingSummaryButt.setCustomSize(new Vector2(250, 50));
        MeetingSummaryButt.destination = "meetingSummary";
        cont.children.Add(MeetingSummaryButt);



        ClientDetailsButt = new SRGUIButton();
        ClientDetailsButt.GroupID = 0;
        ClientDetailsButt.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 30);
        ClientDetailsButt.Text = "Client Details";
        ClientDetailsButt.Position = new Vector2(80, 140);
        ClientDetailsButt.setCustomSize(new Vector2(200, 50));
        cont.children.Add(ClientDetailsButt);


        EndMeetingButt = new SRGUIButton();
        EndMeetingButt.GroupID = 0;
        EndMeetingButt.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 30);
        EndMeetingButt.Text = "End Meeting";
        EndMeetingButt.InPosition = new Vector2(80, 200);
        EndMeetingButt.setCustomSize(new Vector2(200, 50));
        cont.children.Add(EndMeetingButt);

        LastMeetingsButt = new SRGUIButton();
        LastMeetingsButt.GroupID = 0;
        LastMeetingsButt.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 30);
        LastMeetingsButt.Text = "Last Meetings";
        LastMeetingsButt.InPosition = new Vector2(80, 260);
        LastMeetingsButt.setCustomSize(new Vector2(200, 50));
        cont.children.Add(LastMeetingsButt);


        //client Details

        float inputWidth = 280;
        float inputHeight = 30;

        ClientDetailsCont = new SRGUIContainer();
        cont.children.Add(ClientDetailsCont);
        ClientDetailsCont.Alpha = 0;
        ClientDetailsCont.Position = new Vector2(45, 210);
        
        SRGUITexture clientBackText = new SRGUITexture();
        clientBackText.SetTexture(ClientBackText);
        ClientDetailsCont.children.Add(clientBackText);

        TakePictureButt = new SRGUIButton();
        TakePictureButt.GroupID = 0;
        TakePictureButt.Style = new GUIStyle();
        TakePictureButt.setCustomSize(new Vector2(50, 50));
        TakePictureButt.Position = new Vector2(260, 400);
        ClientDetailsCont.children.Add(TakePictureButt);
        ClientDetailsFirstName = new SRGUIInput();
        ClientDetailsFirstName.Style = CommonAssetHolder.instance.GetCustomStyle( CommonAssetHolder.FontNameType.ManagementTitle, 25);
        ClientDetailsFirstName.Text = "name";
        ClientDetailsFirstName.SetSize(new Vector2(inputWidth, inputHeight));
        ClientDetailsFirstName.Position = new Vector2(30, 0);
        ClientDetailsCont.children.Add(GetHintTag(ClientDetailsFirstName, "Name"));
        ClientDetailsCont.children.Add(ClientDetailsFirstName);
        ClientDetailsSurname = new SRGUIInput();
        ClientDetailsSurname.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 25);
        ClientDetailsSurname.Text = "last";
        ClientDetailsSurname.SetSize(new Vector2(inputWidth, inputHeight));
        ClientDetailsSurname.Position = new Vector2(30, 54);
        ClientDetailsCont.children.Add(GetHintTag(ClientDetailsSurname, "Surame"));
        ClientDetailsCont.children.Add(ClientDetailsSurname);
        ClientDetailsTitle = new SRGUIInput();
        ClientDetailsTitle.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 25);
        ClientDetailsTitle.Text = "title";
        ClientDetailsTitle.SetSize(new Vector2(inputWidth, inputHeight));
        ClientDetailsTitle.Position = new Vector2(30, 108);
        ClientDetailsCont.children.Add(GetHintTag(ClientDetailsTitle, "Title"));
        ClientDetailsCont.children.Add(ClientDetailsTitle);
        ClientDetailsMail = new SRGUIInput();
        ClientDetailsMail.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 25);
        ClientDetailsMail.Text = "mail";
        ClientDetailsMail.SetSize(new Vector2(inputWidth, inputHeight));
        ClientDetailsMail.Position = new Vector2(30, 162);
        ClientDetailsCont.children.Add(GetHintTag(ClientDetailsMail, "Mail"));
        ClientDetailsCont.children.Add(ClientDetailsMail);
        ClientDetailsPhone = new SRGUIInput();
        ClientDetailsPhone.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 25);
        ClientDetailsPhone.Text = "phone";
        ClientDetailsPhone.SetSize(new Vector2(inputWidth, inputHeight));
        ClientDetailsPhone.Position = new Vector2(30, 216);
        ClientDetailsCont.children.Add(GetHintTag(ClientDetailsPhone, "Phone Number"));
        ClientDetailsCont.children.Add(ClientDetailsPhone);
        ClientDetailsAircraft = new SRGUIInput();
        ClientDetailsAircraft.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 25);
        ClientDetailsAircraft.Text = "aircraft";
        ClientDetailsAircraft.SetSize(new Vector2(inputWidth, inputHeight));
        ClientDetailsAircraft.Position = new Vector2(30, 270);
        ClientDetailsCont.children.Add(GetHintTag(ClientDetailsAircraft, "Aircraft Type"));
        ClientDetailsCont.children.Add(ClientDetailsAircraft);
        ClientDetailsNotes = new SRGUIInput();
        ClientDetailsNotes.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 20);
        ClientDetailsNotes.Text = "notes";
        ClientDetailsNotes.isMultyLine = true;
        ClientDetailsNotes.SetSize(new Vector2(inputWidth, inputHeight * 4));
        ClientDetailsNotes.Position = new Vector2(30, 324);
        ClientDetailsCont.children.Add(GetHintTag(ClientDetailsNotes, "Notes"));
        ClientDetailsCont.children.Add(ClientDetailsNotes);
        SaveClientButt = new SRGUIButton();
        SaveClientButt.GroupID = 0;
        SaveClientButt.Style = new GUIStyle();
        SaveClientButt.setCustomSize(new Vector2(100, 30));
        SaveClientButt.Position = new Vector2(210, 470);
        ClientDetailsCont.children.Add(SaveClientButt);
        camTexture = new SRGUITexture();
        camTexture.Position = new Vector2(0, 660);
        ClientDetailsCont.children.Add(camTexture);

        //end meeting
        EndMeetingCont = new SRGUIContainer();
        EndMeetingCont.Position = new Vector2(40, 245);
        EndMeetingCont.Alpha = 0;
        cont.children.Add(EndMeetingCont);

        SRGUITexture endMeetingBackText = new SRGUITexture();
        endMeetingBackText.SetTexture(MeetingBackText);
        EndMeetingCont.children.Add(endMeetingBackText);

        SendToHint = new SRGUILabel();
        //SendToHint.SetSize(new Vector2(inputWidth - 10, inputHeight));
        SendToHint.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 20);
        SendToHint.Style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
        SendToHint.Position = new Vector2(40, 5);
        SendToHint.Text = "";
        EndMeetingCont.children.Add(SendToHint);

        SendToInput = new SRGUIInput();
        SendToInput.SetSize(new Vector2(inputWidth - 10, inputHeight));
        SendToInput.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 20);
        SendToInput.Position = new Vector2(40, 5);
        SendToInput.Text = PlayerPrefs.HasKey(DEFAULT_MAIL_ADRESS)? PlayerPrefs.GetString(DEFAULT_MAIL_ADRESS):"";
        EndMeetingCont.children.Add(GetHintTag(SendToInput, "Send To"));
        EndMeetingCont.children.Add(SendToInput);
        
        RememberMeCheck = new SRGUICheckButton();
        RememberMeCheck.Position = new Vector2(192, 36);
        RememberMeCheck.Scale = new Vector2(0.7f, 0.7f);
        RememberMeCheck.SetTexture(CommonAssetHolder.instance.CheckBoxTextures);
        RememberMeCheck.Checked = true;
        EndMeetingCont.children.Add(RememberMeCheck);


        SendtitleInput = new SRGUIInput();
        SendtitleInput.isMultyLine = true;
        SendtitleInput.SetSize(new Vector2(inputWidth - 10, inputHeight * 3));
        SendtitleInput.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 20);
        SendtitleInput.Position = new Vector2(40, 110);
        SendtitleInput.Text = "";
        EndMeetingCont.children.Add(GetHintTag(SendtitleInput, "Mail Body"));
        EndMeetingCont.children.Add(SendtitleInput);

        SendMeetingButt = new SRGUIButton();
        SendMeetingButt.GroupID = 0;
        SendMeetingButt.Style = new GUIStyle();
        SendMeetingButt.setCustomSize(new Vector2(80, 30));
        SendMeetingButt.Position = new Vector2(250, 230);
        EndMeetingCont.children.Add(SendMeetingButt);

        SaveMeetingButt = new SRGUIButton();
        SaveMeetingButt.GroupID = 0;
        SaveMeetingButt.Style = new GUIStyle();
        //SaveMeetingButt.Style = CommonAssetHolder.instance.GetCustomStyle(new Texture2D(10, 10), CommonAssetHolder.FontNameType.ManagementTitle, 10);
        
        SaveMeetingButt.setCustomSize(new Vector2(80, 30));
        SaveMeetingButt.Position = new Vector2(40, 230);
        EndMeetingCont.children.Add(SaveMeetingButt);

        //last meetings

        LastSessionsCont = new SRGUIContainer();
        LastSessionsCont.Position = new Vector2(40, 360);
        LastSessionsCont.Alpha = 0;
        cont.children.Add(LastSessionsCont);

        SessionDescriptionConts = new SRGUIContainer[TotalMettingFields];
        ReopenExistingMeetingButts = new SRGUIButton[TotalMettingFields];
        SessionDescriptionLabels = new SRGUILabel[TotalMettingFields];

        for (int i = 0; i < TotalMettingFields; i++) {

            SessionDescriptionConts[i] = new SRGUIContainer();
            SessionDescriptionConts[i].Position = new Vector2(0, i * 100);
            LastSessionsCont.children.Add(SessionDescriptionConts[i]);

            SRGUITexture lastSessText = new SRGUITexture();
            lastSessText.SetTexture(LastSessionTexture);
            SessionDescriptionConts[i].children.Add(lastSessText);

            ReopenExistingMeetingButts[i] = new SRGUIButton();
            ReopenExistingMeetingButts[i].GroupID = 0;
            ReopenExistingMeetingButts[i].Style = new GUIStyle();
            ReopenExistingMeetingButts[i].Text = "Reopen";
            ReopenExistingMeetingButts[i].setCustomSize(new Vector2(50, 40));
            ReopenExistingMeetingButts[i].Position = new Vector2(260, 70);
            SessionDescriptionConts[i].children.Add(ReopenExistingMeetingButts[i]);

            SessionDescriptionLabels[i] = new SRGUILabel();
            SessionDescriptionLabels[i].Position = new Vector2(60, 10);
            SessionDescriptionLabels[i].Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 30);
            SessionDescriptionLabels[i].setCustomSize(new Vector2(200, 90));
            SessionDescriptionConts[i].children.Add(SessionDescriptionLabels[i]);
        }

        NextPageButt = new SRGUIButton();
        NextPageButt.GroupID = 0;
        NextPageButt.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 30);
        NextPageButt.Text = "Next";
        NextPageButt.setCustomSize(new Vector2(100, 40));
        NextPageButt.Position = new Vector2(200, 500);
        LastSessionsCont.children.Add(NextPageButt);

        PrevPageButt = new SRGUIButton();
        PrevPageButt.GroupID = 0;
        PrevPageButt.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 30);
        PrevPageButt.Text = "Prev";
        PrevPageButt.setCustomSize(new Vector2(100, 40));
        PrevPageButt.Position = new Vector2(30, 500);
        LastSessionsCont.children.Add(PrevPageButt);

        PageLabel = new SRGUILabel();
        PageLabel.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 30);
        PageLabel.Position = new Vector2(120, 500);
        LastSessionsCont.children.Add(PageLabel);


        UnsentSessionsCont = new SRGUIContainer();
        UnsentSessionsCont.InPosition = new Vector2(0, 315);

        SRGUITexture unsetDotTexture = new SRGUITexture();
        unsetDotTexture.SetTexture(DotTexture);
        unsetDotTexture.Position = new Vector2(80, 23);
        UnsentSessionsCont.children.Add(unsetDotTexture);

        UnsentSessionsLabel = new SRGUILabel();
        UnsentSessionsLabel.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.FrutiWhite, 15);
        UnsentSessionsLabel.Text = "0";
        UnsentSessionsLabel.Position = new Vector2(85, 25);
        UnsentSessionsCont.children.Add(UnsentSessionsLabel);
        cont.children.Add(UnsentSessionsCont);

        SRGUILabel unsentTitle = new SRGUILabel();
        unsentTitle.Text = "Unsent Mails";
        unsentTitle.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.ManagementTitle, 30); 
        unsentTitle.Position = new Vector2(110, 15);
        UnsentSessionsCont.children.Add(unsentTitle);

        SendAllButt = new SRGUIButton();
        SendAllButt.Style = CommonAssetHolder.instance.GetCustomStyle(SendAllText);
        SendAllButt.Position = new Vector2(290, 15);
        SendAllButt.GroupID = 0;
        UnsentSessionsCont.children.Add(SendAllButt);

        meetingPage = 0;
        UpdateSessionsList(true);

        FoldState = -1;

    }

    private SRGUILabel GetHintTag(SRGUIInput targetInput, string targetText) {
        SRGUILabel retLabel = new SRGUILabel();
        retLabel.Position = targetInput.Position;
        retLabel.Style = CommonAssetHolder.instance.GetCustomStyle(CommonAssetHolder.FontNameType.FrutiGray, targetInput.Style.fontSize);
        retLabel.Text = targetText;

        MarkedTags.Add(retLabel);
        MarkedInputs.Add(targetInput);

        return retLabel;
    }

    void ClickReaction(SRBaseGUIElement caller) {
        if ((caller == slider) || (caller == MeetingSummaryButt)) {
            //IsMouseDown = true;
            IsOpen = !IsOpen;
            HOTween.To(cont, 0.4f, new TweenParms().Prop("Position", new Vector2((IsOpen) ? 0 : -BGTtexture.width, 0)));

            if (!IsOpen) {
                FoldState = -1;
                SRGUIManager.instance.ActiveButtonGroup = -1;
            } else {
                SRGUIManager.instance.ActiveButtonGroup = 0;
            }
            if (caller == MeetingSummaryButt) {
                OnNavigate(MeetingSummaryButt.destination, null, null);
            }
        } else if (caller == ClientDetailsButt) {
            FoldState = (FoldState == 0) ? -1 : 0;
        } else if (caller == EndMeetingButt) {
            FoldState = (FoldState == 1) ? -1 : 1;
        } else if (caller == LastMeetingsButt) {
            FoldState = (FoldState == 2) ? -1 : 2;
        } else if (caller == SendMeetingButt) {
           
            ProcessEmailAdress();

            StatusMgr.TryToMailStatus(SendToInput.Text, SendtitleInput.Text);
            FoldState = -1;
            UpdateSessionsList(true);
            StatusMgr.ResetCurrentStatus();
        } else if (caller == SaveMeetingButt) {
            StatusMgr.SaveLocally(false);
            FoldState = -1;
            UpdateSessionsList(true);
            StatusMgr.ResetCurrentStatus();
        } else if (caller == NextPageButt) {
            meetingPage++;
            UpdateSessionsList(false);
        } else if (caller == PrevPageButt) {
            meetingPage--;
            UpdateSessionsList(false);
        } else if (caller == SaveClientButt) {
            SaveClientDetails();
            FoldState = -1;
        } else if (caller == TakePictureButt) {
            TakePicture();
        } else if (caller == SendAllButt) {
            StatusMgr.TryToMailUnsent();
            UpdateSessionsList(true);
        } else {
            for (int i = 0; i < ReopenExistingMeetingButts.Length; i++) {
                if (ReopenExistingMeetingButts[i] == caller) {
                    StatusMgr.LoadStatus(i + meetingPage * TotalMettingFields);
                    FoldState = -1;
                    UpdateSessionsList(true);
                    //OnNavigate(PageManager.ROOT_DESTINATION, null, null);
                }
			}
            
        }

    }

    private void ProcessEmailAdress() {
        
        if (RememberMeCheck.Checked) {
            PlayerPrefs.SetString(DEFAULT_MAIL_ADRESS, SendToInput.Text);
        } else {
            PlayerPrefs.SetString(DEFAULT_MAIL_ADRESS, "");
        }

        StringArrJSONWrapper mailsObj;

        if (PlayerPrefs.HasKey("UsedEmails")) {
            string JSONEmails = PlayerPrefs.GetString("UsedEmails");
            mailsObj = JsonUtility.FromJson<StringArrJSONWrapper>(JSONEmails);

            int i = 0;

            string processedMail = mailsObj.mails[0];
            string tobeWrittenMail = "";

            while (i < mailsObj.mails.Length && processedMail != SendToInput.Text.ToLower()) {
                processedMail = mailsObj.mails[i];
                mailsObj.mails[i] = tobeWrittenMail;
                tobeWrittenMail = processedMail;
                i++;
            }
            
        } else {
            mailsObj = new StringArrJSONWrapper();
            mailsObj.mails = new string[20];
        }

        mailsObj.mails[0] = SendToInput.Text.ToLower();
        
        string ToJSONEmails = JsonUtility.ToJson(mailsObj);
        PlayerPrefs.SetString("UsedEmails", ToJSONEmails);
    }

    private void UpdateSessionsList(bool fromFiles) {
        if (fromFiles) {
            CurrentSessions = StatusMgr.GetFileList();
        }
        int completeSessions = 0;
        int incompleteSessions = 0;
        for (int i = 0; i < CurrentSessions.Length; i++) {
            if (CurrentSessions[i].IsComplete) {
                completeSessions++;
            } else {
                incompleteSessions++;
            }
        }

        PageLabel.Text = (meetingPage + 1) + "/" + Mathf.Ceil((float)incompleteSessions / (float)TotalMettingFields);

        LastMeetingsButt.Text = "Last Meetings";

        for (int i = meetingPage * TotalMettingFields; i < TotalMettingFields + meetingPage * TotalMettingFields; i++) {
            SessionDescriptionConts[i % TotalMettingFields].Enabled = (i < incompleteSessions);
            if (i < incompleteSessions) {
                SessionDescriptionLabels[i % TotalMettingFields].Text = CurrentSessions[i].Customer + "\n" + CurrentSessions[i].Date;
            }
        }

        NextPageButt.Enabled = meetingPage < Mathf.Ceil((float)incompleteSessions / (float)TotalMettingFields) - 1;
        PrevPageButt.Enabled = meetingPage > 0;

        UnsentSessionsLabel.Text = (completeSessions).ToString();
        UnsentSessionsCont.Enabled = completeSessions > 0;
    }

    void Update() {
        
        /*if (IsMouseDown) {
            print(Input.GetMouseButton(0));
        }
        IsMouseDown = IsMouseDown && (Input.GetMouseButton(0) || Input.touchCount > 0);


        if (IsMouseDown) {
            cont.Position.x = Input.mousePosition.x - 270;
        }*/

        for (int i = 0; i < MarkedInputs.Count; i++) {
            MarkedTags[i].Enabled = MarkedInputs[i].Text.Trim() == "";
        }

        if (SendToInput.IsModified) {
            UpdateSuggestedEmails();
        }
    }

    private void UpdateSuggestedEmails() {
        if (SendToInput.Text == "") {
            SendToHint.Text = "";
        } else {
            if (PlayerPrefs.HasKey("UsedEmails")) {
                string JSONEmails = PlayerPrefs.GetString("UsedEmails");
                StringArrJSONWrapper mailsObj = JsonUtility.FromJson<StringArrJSONWrapper>(JSONEmails);
                for (int i = 0; i < mailsObj.mails.Length; i++) {
                    if (mailsObj.mails[i].Contains(SendToInput.Text.ToLower())) {
                        print(mailsObj.mails[i]);
                    }
                }
            }
        }
        
    }

    public int FoldState {
        get {
            return _FoldState;
        }
        set {
            if (webCamTexture.isPlaying) {
                
                if (photo != null) {
                    camTexture.SetTexture(photo, new Vector2(300, 200), true);
                } else {
                    camTexture.SetTexture();
                }
                

                webCamTexture.Stop();
            }
            switch (value) {
            case -1:
                HOTween.To(EndMeetingButt, 0.4f, "Position", EndMeetingButt.InPosition);
                HOTween.To(LastMeetingsButt, 0.4f, "Position", LastMeetingsButt.InPosition);
                HOTween.To(UnsentSessionsCont, 0.4f, "Position", UnsentSessionsCont.InPosition);

                HOTween.To(ClientDetailsCont, 0.4f, "Alpha", 0);
                HOTween.To(EndMeetingCont, 0.4f, "Alpha", 0);
                HOTween.To(LastSessionsCont, 0.4f, "Alpha", 0);
                break;
            case 0:
                float shift = 545;

                HOTween.To(EndMeetingButt, 0.4f, "Position", EndMeetingButt.InPosition + new Vector2(0, shift));
                HOTween.To(LastMeetingsButt, 0.4f, "Position", LastMeetingsButt.InPosition + new Vector2(0, shift));
                HOTween.To(UnsentSessionsCont, 0.4f, "Position", UnsentSessionsCont.InPosition + new Vector2(0, shift));
                ClientDetailsCont.Enabled = true;
                HOTween.To(ClientDetailsCont, 0.4f, "Alpha", 1);
                HOTween.To(EndMeetingCont, 0.4f, "Alpha", 0);
                HOTween.To(LastSessionsCont, 0.4f, "Alpha", 0);

                RefreshClientDetails();

                break;
            case 1:
                shift = 270;

                HOTween.To(EndMeetingButt, 0.4f, "Position", EndMeetingButt.InPosition);
                HOTween.To(LastMeetingsButt, 0.4f, "Position", LastMeetingsButt.InPosition + new Vector2(0, shift));
                HOTween.To(UnsentSessionsCont, 0.4f, "Position", UnsentSessionsCont.InPosition + new Vector2(0, shift));

                HOTween.To(ClientDetailsCont, 0.4f, "Alpha", 0);
                EndMeetingCont.Enabled = true;
                HOTween.To(EndMeetingCont, 0.4f, "Alpha", 1);
                HOTween.To(LastSessionsCont, 0.4f, "Alpha", 0);
                break;
            case 2:
                shift = 590;

                HOTween.To(EndMeetingButt, 0.4f, "Position", EndMeetingButt.InPosition);
                HOTween.To(LastMeetingsButt, 0.4f, "Position", LastMeetingsButt.InPosition);
                HOTween.To(UnsentSessionsCont, 0.4f, "Position", UnsentSessionsCont.InPosition + new Vector2(0, shift));

                HOTween.To(ClientDetailsCont, 0.4f, "Alpha", 0);
                HOTween.To(EndMeetingCont, 0.4f, "Alpha", 0);
                LastSessionsCont.Enabled = true;
                HOTween.To(LastSessionsCont, 0.4f, "Alpha", 1);
                break;

            }

            Invoke("ListFolded", 0.5f);

            _FoldState = value;
        }
    }

    private void ListFolded() {
        ClientDetailsCont.Enabled = ClientDetailsCont.Alpha > 0; ;
        EndMeetingCont.Enabled = EndMeetingCont.Alpha > 0; ;
        LastSessionsCont.Enabled = LastSessionsCont.Alpha > 0; ;
    }

    private void SaveClientDetails() {
        ClientDetails details = new ClientDetails();
        details.FirstName = ClientDetailsFirstName.Text;
        details.Surname = ClientDetailsSurname.Text;
        details.Title = ClientDetailsTitle.Text;
        details.Mail = ClientDetailsMail.Text;
        details.PhoneNumber = ClientDetailsPhone.Text;
        details.AircraftType = ClientDetailsAircraft.Text;
        details.Notes = ClientDetailsNotes.Text;


        StatusMgr.SetClientDetails(details);
    }

    private void TakePicture() {
        if (webCamTexture.isPlaying) {
            
            

            photo = new Texture2D(webCamTexture.width, webCamTexture.height);
            photo.SetPixels(webCamTexture.GetPixels());
            photo.Apply();

            camTexture.SetTexture(photo, new Vector2(300, 200), true);

            webCamTexture.Stop();
        } else {
            
            webCamTexture.Play();
            camTexture.SetTexture(webCamTexture, new Vector2(300, 200), true);
        }
        
    }

    private void RefreshClientDetails() {
        ClientDetails details = StatusMgr.GetClientDetails();
        ClientDetailsFirstName.Text = details.FirstName;
        ClientDetailsSurname.Text = details.Surname;
        ClientDetailsTitle.Text = details.Title;
        ClientDetailsMail.Text = details.Mail;
        ClientDetailsPhone.Text = details.PhoneNumber;
        ClientDetailsAircraft.Text = details.AircraftType;
        ClientDetailsNotes.Text = details.Notes;
    }

    [Serializable]
    public class StringArrJSONWrapper{
        public string[] mails;
    }
}
