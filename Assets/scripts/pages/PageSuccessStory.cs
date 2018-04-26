using UnityEngine;
using System.Collections;
using System.Xml;
using Holoville.HOTween;
using System.Collections.Generic;

public class PageSuccessStory : BasePage {
    
    public Texture SideArrowTexture;

    public GallerySpinner SpinnerPF;

    private GallerySpinner Spinner;

    private SRGUIButton SpinnerButt;

    private Vector3 SpinnerSmallPosition = new Vector3(0, -1.4f, 9.01f);
    private Vector3 SpinnerLargePosition = new Vector3(0, 0, 3.9f);
    private Quaternion SpinnerSmallRotatiom;
    private Quaternion SpinnerLargeeRotatiom = new Quaternion();

    private GameObject SpinnerCont;
    private SRGUIButton TrueBack;
    
    private List<SRGUIContainer> labels;
    private SRGUILabel Title;
    private bool InGalleryTransition = false;
    
    override internal void InitPage(XmlNode pageData) {
        base.InitPage(pageData);


        //3D

        SpinnerCont = new GameObject();
        SpinnerCont.transform.parent = gameObject.transform;
        Spinner = Instantiate(SpinnerPF) as GallerySpinner;
        Spinner.Spinner = new GameObject();
        Spinner.Spinner.transform.parent = SpinnerCont.transform;
        SpinnerCont.transform.position = SpinnerSmallPosition;

        SpinnerCont.transform.rotation = Quaternion.LookRotation(SpinnerCont.transform.position - Camera.main.transform.position);

        SpinnerSmallRotatiom = SpinnerCont.transform.rotation;

        Spinner.setActive(false);

        //2D


        //path
        Title = createTitlePath(pageData.Attributes.GetNamedItem("path").Value, TitleTypes.Center);


        SpinnerButt = new SRGUIButton();
        SpinnerButt.Style = new GUIStyle();
        //SpinnerButt.Style = AssetHolder.GetCustomStyle(SideArrowTexture as Texture2D, CommonAssetHolder.FontNameType.FrutiBlack80, 10);
        
        SpinnerButt.setCustomSize(new Vector2(800, 400));
        SpinnerButt.Position = new Vector2(550, 700);
        

        TransitionData transitionData;

        labels = new List<SRGUIContainer>();

                foreach (XmlNode childNode in pageData) {
            if (childNode.Name == "items") {

                int colCount = 0;

                float colspace = (childNode.ChildNodes.Count == 2)?800:550;

                foreach (XmlNode collumn in childNode) {

                    SRGUIContainer[] DistributeGroup = new SRGUIContainer[collumn.ChildNodes.Count];
                    LabelDistributeData DistributeData = new LabelDistributeData();
                    DistributeData.SpaceBetweenItems = 16;

                    int itemCount = 0;
                    foreach (XmlNode caption in collumn) {
                        SRGUIContainer cont = new SRGUIContainer();
                        SRGUILabel origLabel = new SRGUILabel();
                        origLabel.Text = caption.Attributes.GetNamedItem("caption").Value;
                        origLabel.Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.FrutiBlack80, 33);
                        
                        float xOffset = 0;
                        if (origLabel.Text.Substring(0, 1) == "-") {
                            origLabel.Text = origLabel.Text.Substring(1, origLabel.Text.Length - 1);
                            xOffset = 30;
                        }

                        SRGUICharSpaceLabel label = SRGUICharSpaceLabel.FromLabel(origLabel, 0, -3);

                        SRGUITexture arrow = new SRGUITexture();
                        arrow.SetTexture(SideArrowTexture);
                        arrow.Position = new Vector2(-35, 8);

                        cont.children.Add(label);
                        cont.children.Add(arrow);

                        //cont.InPosition = new Vector2(230, 240 + itemCount * (420f / (childNode.ChildNodes.Count)));
                        cont.InPosition = new Vector2(245 + colCount * colspace + xOffset, 0);
                        cont.OutPosition = cont.InPosition + new Vector2(0, -10);

                        GUIManager.RegisterGUIElement(cont, gameObject);

                        transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha, TransitionTypes.PositionGUI }, 0.2f, 0f, 0.3f);
                        RegisterTransition(transitionData, cont);

                        //DistributeGroup3[itemCount] = cont;

                        DistributeGroup[itemCount] = cont;
                        labels.Add(cont);

                        itemCount++;
                    }

                    //RedistributeLabelsY(new ISRGUIDistributable[][] { DistributeGroup }, new LabelDistributeData[] { DistributeData }, 455, 1300);
                    RedistributeLabelsY(new ISRGUIDistributable[][] { DistributeGroup }, new LabelDistributeData[] { DistributeData }, float.Parse(pageData.Attributes.GetNamedItem("startY").Value), 1300);

                    colCount++;
                }
            }
            GUIManager.RegisterGUIElement(SpinnerButt, gameObject);

            Spinner.Init( "media/" + pageData.Attributes.GetNamedItem("id").Value);
            GUIManager.RegisterGUIElement(Spinner.CloseLargeButt, gameObject);
            GUIManager.RegisterGUIElement(Spinner.LargeImageCont, gameObject);

            GUIManager.RegisterGUIElement(Spinner.CloseButt, gameObject);
        }

        //Spinner.SetAppearence(true);

        //back butt
        TrueBack = createStandardBack();

        //logo
        createStandardLogo();

    }

    override protected void DoInternalUpdate() {
        base.DoInternalUpdate();

        Spinner.DoUpdate();

    }

    internal override void ClickReaction(SRBaseGUIElement caller) {
        
        base.ClickReaction(caller);
        if (InGalleryTransition) return;
        if (caller == SpinnerButt) {
            InGalleryTransition = true;
            SpinnerButt.Enabled = false;
            //TrueBack.Enabled = false;
            //Spinner.CloseButt.Enabled = true;

            HOTween.To(SpinnerCont.transform, 0.4f, new TweenParms().Prop("localPosition", SpinnerLargePosition).Prop("localRotation", SpinnerLargeeRotatiom).OnComplete(GrowEneded));

            for (int i = 0; i < labels.Count; i++) {
                HOTween.To(labels[i], 0.2f, "Alpha", 0);
            }

            HOTween.To(Title, 0.2f, "Alpha", 0);
            HOTween.To(TrueBack, 0.2f, "Alpha", 0);

            Invoke("GalleryEnabled", 0.4f);

            SoundController.instance.playSingleSound(SoundController.SoundT.GalleryPopUp);

        } else if (caller == Spinner.CloseButt) {
            InGalleryTransition = true;
            SpinnerButt.Enabled = true;
            TrueBack.Enabled = true;
            Spinner.setActive(false);
            Spinner.LargeImageEnabled = false;
            HOTween.To(SpinnerCont.transform, 0.4f, new TweenParms().Prop("localPosition", SpinnerSmallPosition).Prop("localRotation", SpinnerSmallRotatiom));
            for (int i = 0; i < labels.Count; i++) {
                HOTween.To(labels[i], 0.2f, "Alpha", 1);
            }

            HOTween.To(Title, 0.2f, "Alpha", 1);
            HOTween.To(TrueBack, 0.2f, "Alpha", 1);
            Invoke("GalleryDissabled", 0.4f);
        } else if (caller == Spinner.CloseLargeButt) {
            Spinner.CloseLarge();
        }

    }

    private void GalleryEnabled() {
        if (Spinner.HasAnyData) {
            TrueBack.Enabled = false;
            Spinner.CloseButt.Enabled = true;
            Spinner.setActive(true);
            InGalleryTransition = false;
            Spinner.LargeImageEnabled = true;
        }
    }

    private void GalleryDissabled() {
        InGalleryTransition = false;
    }

    override public void DoEnter(float previousDelay, Object extraParams, string extraData = null) {
        base.DoEnter(previousDelay, extraParams, extraData);

        
        Spinner.IsMouseDown = false;

        Spinner.DoFade(0.6f, previousDelay, true);
        Spinner.Spinner.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        HOTween.To(Spinner.Spinner.transform, 0.6f, new TweenParms().Prop("localScale", new Vector3(1, 1, 1)).Delay(previousDelay));

        Spinner.LargeImageEnabled = false;
    }

    public void GrowEneded(TweenEvent data) {
        Spinner.GrowEneded();
    }

    override public void DoExit(string targetID) {
        base.DoExit(targetID);

        if (Spinner.LargeImageEnabled){
            ClickReaction(Spinner.CloseButt);
        }
        

        Spinner.IsMouseDown = false;

        Spinner.DoFade(0.6f, 0, false);


    }

    void OnEnable() {
        if (SpinnerCont != null) SpinnerCont.SetActive(true);
    }

    void OnDisable() {
        if (SpinnerCont != null) {
            SpinnerCont.SetActive(false);
            Spinner.LoadAssets(false);
        }
    }

}
