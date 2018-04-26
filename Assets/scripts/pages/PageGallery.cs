using UnityEngine;
using System.Collections;
using System.Xml;
using Holoville.HOTween;

public class PageGallery : BasePage {


    private SRGUILabel ImageTitle;

    public GallerySpinner SpinnerPF;

    private GallerySpinner Spinner;
    
    override internal void InitPage(XmlNode pageData) {
        base.InitPage(pageData);
        ExitTime = 1f;
        

        //3D

        Spinner = Instantiate(SpinnerPF) as GallerySpinner;
        Spinner.Spinner = new GameObject();
        Spinner.Spinner.transform.parent = gameObject.transform;
        //Spinner.Spinner.transform.position = new Vector3(0, 0, 4.5f);
        Spinner.Spinner.transform.position = new Vector3(0, -0.02f, 4.03f);

        /*Texture[] ThumbsTextures = new Texture[pageData.ChildNodes.Count];
        string[] ThumbsTitles = new string[pageData.ChildNodes.Count];


        string dir = "media/" + pageData.Attributes.GetNamedItem("id").Value;
        Texture[] textures = Resources.LoadAll<Texture>(dir);

        if (textures.Length > 0) {
            int medCount = 0;
            foreach (Texture text in textures) {
                ThumbsTextures[medCount] = text;
                //ThumbsTitles[medCount] = thumbData.Attributes.GetNamedItem("caption").Value;
                medCount++;
            }

        } else {
            //backup only
            int medCount = 0;
            foreach (XmlNode thumbData in pageData) {
                ThumbsTextures[medCount] = Resources.Load("media/" + pageData.Attributes.GetNamedItem("id").Value + "/" + thumbData.Attributes.GetNamedItem("asset").Value) as Texture;
                ThumbsTitles[medCount] = thumbData.Attributes.GetNamedItem("caption").Value;
                medCount++;
            }
        }*/


        Spinner.Init("media/" + pageData.Attributes.GetNamedItem("id").Value, pageData);
        Spinner.OnNavigate += SoinnerNavigationClicked;

        //2D

        
        GUIManager.RegisterGUIElement(Spinner.CloseLargeButt, gameObject);

        //back butt
        createStandardBack();

        
        //path
        createTitlePath(pageData.Attributes.GetNamedItem("path").Value, TitleTypes.InPath);


        GUIManager.RegisterGUIElement(Spinner.LargeImageCont, gameObject);

        //logo
        createStandardLogo();
    }

    private void SoinnerNavigationClicked(string targetID, string extra) {
        DipatchNavigate(targetID, null, extra);
    }

    override public void DoEnter(float previousDelay, Object extraParams, string extraData = null) {
        base.DoEnter(previousDelay, extraParams, extraData);

        Spinner.IsMouseDown = false;

        Spinner.DoFade(0.6f, previousDelay, true);
        Spinner.Spinner.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        //Spinner.LargeImageEnabled = true;
        HOTween.To(Spinner.Spinner.transform, 0.6f, new TweenParms().Prop("localScale", new Vector3(1, 1, 1)).Delay(previousDelay).OnComplete(EnterComplete).OnStart(EnterStart));

        

        //TransitionTweens.Add(HOTween.To(this, 2, new TweenParms().Prop("CenterItemIndex", 0).Delay(previousDelay)));
        
    }

    private void EnterStart(TweenEvent data) {
        SoundController.instance.playSingleSound(SoundController.SoundT.GalleryPopUp);
    }

    private void EnterComplete(TweenEvent data) {
        Spinner.LargeImageEnabled = true;
    }

    override public void DoExit(string targetID) {
        base.DoExit(targetID);

        Spinner.LargeImageEnabled = false;

        Spinner.IsMouseDown = false;

        Spinner.DoFade(0.6f, 0, false);

        //TransitionTweens.Add(HOTween.To(this, 2, new TweenParms().Prop("CenterItemIndex", CenterItemIndex - TotalGalleryItems)));

    }

    internal override void ClickReaction(SRBaseGUIElement caller) {
        base.ClickReaction(caller);

        if (caller == Spinner.CloseLargeButt) {
            Spinner.CloseLarge();
            
        }

    }   

    override protected void DoInternalUpdate() {
        base.DoInternalUpdate();

        Spinner.DoUpdate();
    }


    void OnEnable(){
        if (Spinner != null) {
            Spinner.gameObject.SetActive(true);
        }
    }

    void OnDisable() {
        if (Spinner != null) {
            Spinner.gameObject.SetActive(false);
            Spinner.LoadAssets(false);
        }
    }
    

}
