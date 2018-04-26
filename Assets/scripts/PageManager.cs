using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using Holoville.HOTween;
using System;

public class PageManager : MonoBehaviour {

    public GameObject[] PagesPS;
    public Management ManagementPF;

    public TextAsset SourceDoc;
    private XmlDocument SourceXml;

    private Management Manage;

    //private List<BasePage> Pages = new List<BasePage>();
    private Dictionary<string, BasePage> PagesById = new Dictionary<string, BasePage>();

    private BasePage CurrentPage = null;
    private BasePage TargetPage = null;

    public GUITexture fromBg;
    public GUITexture toBg;

    public float exitProgress;
    public float _backsTransition;
    public Color backsTransitionColor;

    public static readonly string BACK_DESTINATION = "back";
    public static readonly string ROOT_DESTINATION = "root";

    private List<string> NavigationRoute = new List<string>();
    private Tweener BackTween;

	// Use this for initialization
	void Start () {
        backsTransitionColor = new Color(0.5f, 0.5f, 0.5f);

        SourceXml = new XmlDocument();
        SourceXml.LoadXml(SourceDoc.text);

        InitPages();

	}

    private void InitPages(){
        XmlNodeList pages = SourceXml.GetElementsByTagName("page");

        foreach (XmlNode pageData in pages) {

            GameObject rawObj = null;
            int backTextureIndex = 0;
            switch (pageData.Attributes.GetNamedItem("type").Value) {
                case "carusel":
                    rawObj = Instantiate(PagesPS[0]) as GameObject;
                    backTextureIndex = 0;
                    break;
                case "mainTopic":
                    rawObj = Instantiate(PagesPS[1]) as GameObject;
                    backTextureIndex = 1;
                    break;
                case "subTopic":
                    rawObj = Instantiate(PagesPS[2]) as GameObject;
                    backTextureIndex = 2;
                    break;
                case "checkList":
                    rawObj = Instantiate(PagesPS[3]) as GameObject;
                    backTextureIndex = 3;
                    break;
                case "gallery":
                    rawObj = Instantiate(PagesPS[4]) as GameObject;
                    backTextureIndex = 4;
                    break;
                case "successStory":
                    rawObj = Instantiate(PagesPS[5]) as GameObject;
                    backTextureIndex = 5;
                    break;
                case "meetingSummary":
                    rawObj = Instantiate(PagesPS[6]) as GameObject;
                    backTextureIndex = 0;
                    break;
                case "typeRating":
                    rawObj = Instantiate(PagesPS[7]) as GameObject;
                    backTextureIndex = 4;
                    break;
                case "fbomap":
                    rawObj = Instantiate(PagesPS[8]) as GameObject;
                    backTextureIndex = 0;
                    break;
                case "certDB":
                    rawObj = Instantiate(PagesPS[9]) as GameObject;
                    backTextureIndex = 0;
                    break;
            }
                

            BasePage pageInst = rawObj.GetComponent(typeof(BasePage)) as BasePage;

            pageInst.ID = pageData.Attributes.GetNamedItem("id").Value;
            pageInst.BackgroundTexture = CommonAssetHolder.instance.BackgroundTextures[backTextureIndex];

            pageInst.InitPage(pageData);
            pageInst.OnNavigate += DoNavigate;
            PagesById[pageInst.ID] = pageInst;
            pageInst.gameObject.SetActive(false);

        }

        StatusManager.Instance.Init();

        Manage = Instantiate(ManagementPF) as Management;
        Manage.Init();
        Manage.OnNavigate += DoNavigate;


        StartCoroutine(CheckReadytoNavigate());

        PageCarousel.LoadTextures();
        
    }

    private IEnumerator CheckReadytoNavigate() {
        
        while (!PageCarousel.CheckTexturesLoaded()) yield return new WaitForSeconds(0.25f);
        
        Invoke("DelayedFirstNav", 2);
        
    }

    private void DelayedFirstNav() {
        DoNavigate("root", null, null);
        Invoke("DelayedFirstGUI", 3);
    }

    private void DelayedFirstGUI() {
        SRGUIManager.instance.Enabled = true;
    }

    private void DoNavigate(string targetID, UnityEngine.Object extraParams, string extraData) {
        if (PagesById.ContainsKey(targetID) || targetID ==  BACK_DESTINATION) {
            if (targetID == BACK_DESTINATION) {
                NavigationRoute.RemoveAt(NavigationRoute.Count - 1);
            }else{
                if (NavigationRoute.Count > 0 && targetID == NavigationRoute[NavigationRoute.Count - 1]) {
                    return;
                }
                NavigationRoute.Add(targetID);
            }


            TargetPage = PagesById[NavigationRoute[NavigationRoute.Count - 1]];
                
            exitProgress = 0;
            TargetPage.gameObject.SetActive(true);
            if (CurrentPage == null) {
                ExitComplete(null);
                TargetPage.DoEnter(0, null);
                TargetPage.RegisterListeners();
            } else {
                TargetPage.RegisterListeners();
                CurrentPage.UnRegisterListeners();
                CurrentPage.DoExit(TargetPage.ID);
                CurrentPage.NavigationEnabled = false;
                TargetPage.DoEnter(CurrentPage.ExitTime, extraParams, extraData);
                HOTween.To(this, CurrentPage.ExitTime, new TweenParms().Prop("exitProgress", 1).OnComplete(ExitComplete));
            }

           
        }
    }

    private void ExitComplete(TweenEvent data) {
        
        if (data != null) {
            //existing session
            
            CurrentPage.gameObject.SetActive(false);

            fromBg.texture = toBg.texture;
            toBg.texture = TargetPage.BackgroundTexture;

            backsTransition = 0;
            if (BackTween != null) BackTween.Kill();
            BackTween =  HOTween.To(this, 1.5f, new TweenParms().Prop("backsTransition", 1).Ease(EaseType.Linear));

        } else {
            //new session
            backsTransition = 1;
            toBg.texture = TargetPage.BackgroundTexture;
        }




        

        CurrentPage = TargetPage;
        CurrentPage.NavigationEnabled = true;
    }


    public float backsTransition { 
        get {
            return _backsTransition;
        } 
        set{
            backsTransitionColor.a = value;
            toBg.color = backsTransitionColor;
            _backsTransition = value;
        }
    }
}
