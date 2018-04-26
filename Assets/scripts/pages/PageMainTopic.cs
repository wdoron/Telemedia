using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using System.Xml;
using System.Collections.Generic;

public class PageMainTopic : BasePage {

    private GameObject CentralObject;
    
    private Matrix4x4 BkpMat;


    private List<SRGUIContainer> TopMenuItems = new List<SRGUIContainer>();
    private GUIStyle TopStyleNormal;
    private GUIStyle TopStyleSelected;
    private SRBaseGUIElement SelectedButt;

    private GUIStyle TopMenuButtStyle;
    public Texture[] TopMenuTextures;
    public Texture[] SideMenuBacks;

    public Texture SideMenuDot;

    private SRGUITexture FlatTransitionTexture;
    private float FlatTransitionTime;
    private int FlatTransitionFrame;
    private const float FlatTransitionFPS = 30;
    /*public Sprite[] FlatTransitionSeq0;
    public Sprite[] FlatTransitionSeq1;
    public Sprite[] FlatTransitionSeq2;
    public Sprite[] FlatTransitionSeq3;
    public Sprite[] FlatTransitionSeq4;
    public Sprite[] FlatTransitionSeq5;*/
    public static Texture[][] FlatTransitionSeq;
    private bool FlatTransitionPlaying = false;
    private TransitionData SideMenuTransitionData;
    private Vector3 SelectedItemInPosition;

    public static int SelectedFlatTransition;

    override internal void InitPage(XmlNode pageData) {
        base.InitPage(pageData);
        ExitTime = 1f;
        //3D
        gameObject.transform.position = new Vector3(0, 0, -10f);

        SelectedItemInPosition = new Vector3(0.1683824f, -0.05769891f, 11.72f);
        
        //back butt
        createStandardBack();

        //logo
        createStandardLogo();

        FlatTransitionTexture = new SRGUITexture();
        FlatTransitionTexture.Enabled = false;
        FlatTransitionTexture.Scale = new Vector2(1.65f, 1.65f);
        FlatTransitionTexture.Position = new Vector2(295, 110);
        //FlatTransitionTexture.Position = new Vector2(47 * 2 + 46, 26 * 2 + 23);
        //FlatTransitionTexture.Scale = new Vector2(2, 2);
        GUIManager.RegisterGUIElement(FlatTransitionTexture, gameObject);

        //side menu back
        SRGUITexture SideBackStretch = new SRGUITexture();
        //SideBackStretch.SetTexture(SideMenuBacks[0]);
        SRGUITexture SideBackCorner = new SRGUITexture();
        SideBackCorner.SetTexture(SideMenuBacks[1]);

        SRGUIContainer SideMenuCont = new SRGUIContainer();
        SideMenuCont.children.Add(SideBackStretch);
        SideMenuCont.children.Add(SideBackCorner);

        SideMenuTransitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.PositionGUI }, 0.4f, 0.2f, 0.2f);
        RegisterTransition(SideMenuTransitionData, SideMenuCont);

        TopMenuButtStyle = new GUIStyle();
        TopStyleNormal =  AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.FrutiGray, 30);
        TopStyleNormal.alignment = TextAnchor.MiddleLeft;
        TopStyleSelected = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.CorpBold, 30, true);
        TopStyleSelected.alignment = TextAnchor.MiddleLeft;

        foreach (XmlNode toppic in pageData) {
            int i;

            //top menu
            if (toppic.Name == "topmenu") {
                i = 0;

                float xPos = 390;

                foreach (XmlNode item in toppic) {

                    //float buttWidth = float.Parse(item.Attributes.GetNamedItem("width").Value);

                    SRGUILabel label = new SRGUILabel();
                    label.Style = TopStyleNormal;
                    label.Text = item.Attributes.GetNamedItem("caption").Value;
                    
                    label.Position = new Vector2(55, 0);

                    float buttWidth = label.Size.x + 110;

                    label.setCustomSize(new Vector2(label.Size.x, TopMenuTextures[0].height - 10)); 

                    SRGUIButton butt = new SRGUIButton();
                    butt.Style = TopMenuButtStyle;
                    butt.setCustomSize(new Vector2(buttWidth, TopMenuTextures[0].height));
                    butt.destination = item.Attributes.GetNamedItem("dest").Value;
                    butt.HasSound = true;

                    SRGUITexture LeftText = new SRGUITexture();
                    LeftText.SetTexture(TopMenuTextures[(i == 0)?0:1]);

                    SRGUITexture MidText = new SRGUITexture();
                    MidText.SetTexture(TopMenuTextures[2], new Vector2(buttWidth - TopMenuTextures[0].width - TopMenuTextures[3].width, TopMenuTextures[0].height));
                    MidText.Position = new Vector2(TopMenuTextures[0].width, 0);

                    SRGUITexture RightText = new SRGUITexture();
                    RightText.SetTexture(TopMenuTextures[(i == toppic.ChildNodes.Count - 1) ? 3 : 4]);
                    RightText.Position = new Vector2(buttWidth - TopMenuTextures[4].width, 0);


                    SRGUIContainer cont = new SRGUIContainer();
                    cont.children.Add(LeftText);
                    cont.children.Add(MidText);
                    cont.children.Add(RightText);
                    cont.children.Add(label);
                    cont.children.Add(butt);

                    cont.OutScale = new Vector2(0.2f, 0.2f);
                    cont.InScale = new Vector2(1f, 1f);
                    cont.ClickInScale = new Vector2(1.02f, 1.02f);

                    cont.Axis = new Vector2(buttWidth / 2, 45);

                    cont.Position = new Vector2(xPos, 80);
                    GUIManager.RegisterGUIElement(cont, gameObject, false);

                    TopMenuItems.Add(cont);

                    xPos += buttWidth - 50;

                    i++;
                }
            }

            //side menu
            if (toppic.Name == "sidemenu") {
                i = 0;

                SRGUILabel labelOrig = new SRGUILabel();
                labelOrig.Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.CorpBold, 69, true);
                labelOrig.Text = toppic.Attributes.GetNamedItem("caption").Value;
                
                SRGUICharSpaceLabel label = SRGUICharSpaceLabel.FromLabel(labelOrig, 0, -15);
                label.Position = new Vector2(60, 0);
                
                SideMenuCont.children.Add(label);

                SideBackCorner.Position = new Vector2(450, 0);
                SideBackCorner.Position = new Vector2(Mathf.Max(SideBackCorner.Position.x, label.Position.x + label.Size.x), 0);

                ISRGUIDistributable[] DistributeGroup1 = new ISRGUIDistributable[] { label };
                LabelDistributeData DistributeData1 = new LabelDistributeData();

                ISRGUIDistributable[] DistributeGroup2 = new ISRGUIDistributable[toppic.ChildNodes.Count];
                LabelDistributeData DistributeData2 = new LabelDistributeData();
                DistributeData2.LeadingSpace = 5;
                DistributeData2.SpaceBetweenItems = 40;
                DistributeData2.MaxSpaceBetweenItems = 80;

                i++;
                foreach (XmlNode item in toppic) {

                    labelOrig = new SRGUILabel();
                    labelOrig.Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.FrutiBlack80, 32);
                    labelOrig.Text = item.Attributes.GetNamedItem("caption").Value;

                    label = SRGUICharSpaceLabel.FromLabel(labelOrig, -1.2f, 0);

                    SideMenuCont.children.Add(label);

                    label.Position = new Vector2(95, 0);
                    
                    DistributeGroup2[i - 1] = label;

                    SideBackCorner.Position = new Vector2(Mathf.Max(SideBackCorner.Position.x, label.Position.x + label.Size.x - SideMenuBacks[1].width / 2), 0);

                    i++;
                }

                RedistributeLabelsY(new ISRGUIDistributable[][] { DistributeGroup1, DistributeGroup2 }, new LabelDistributeData[] { DistributeData1, DistributeData2 }, 55, 550, 1);

                SideBackStretch.SetTexture(SideMenuBacks[0], new Vector2(SideBackCorner.Position.x, SideMenuBacks[0].height));
                SideMenuCont.OutPosition = new Vector2(-SideBackCorner.Position.x - SideMenuBacks[1].width, 300);
                SideMenuCont.InPosition = new Vector2(0, 300);

                foreach (ISRGUIDistributable lb in DistributeGroup2) {
                    SRGUITexture dot = new SRGUITexture();
                    dot.SetTexture(SideMenuDot);
                    dot.Position = new Vector2(60, (lb as SRBaseGUIElement).Position.y + 16);
                    SideMenuCont.children.Add(dot);
                }
            }

            GUIManager.RegisterGUIElement(SideMenuCont, gameObject);

        }

        /*FlatTransitionSeq = new Sprite[6][];
        FlatTransitionSeq[0] = FlatTransitionSeq0;
        FlatTransitionSeq[1] = FlatTransitionSeq1;
        FlatTransitionSeq[2] = FlatTransitionSeq2;
        FlatTransitionSeq[3] = FlatTransitionSeq3;
        FlatTransitionSeq[4] = FlatTransitionSeq4;
        FlatTransitionSeq[5] = FlatTransitionSeq5;*/

    }


    override public void DoEnter(float previousDelay, Object extraParams, string extraData = null) {
        float FlatTransitionDelay = ((extraData == null) ? 0 : 0.8f);

        SideMenuTransitionData.InDelay = 0.2f + FlatTransitionDelay;

        base.DoEnter(previousDelay, extraParams, extraData);

        if (extraParams != null) {
            CentralObject = (extraParams as Component).gameObject;
        }


        if (extraData == null) { //normal transition
            CentralObject.transform.parent = this.transform;
            //HOTween.To(CentralObject.transform, 0.6f, new TweenParms().Prop("localPosition", new Vector3(0.373f, 0.009f, 12.32f)).Delay(0.2f));

            HOTween.To(CentralObject.transform, 0.6f, new TweenParms().Prop("localPosition", SelectedItemInPosition).Delay(0.2f));
            
            unloadFlatTransitionAssets();

        } else {
            FlatTransitionTexture.SetTexture(FlatTransitionSeq[SelectedFlatTransition][0]);
            FlatTransitionTexture.Enabled = true;
            FlatTransitionTime = 0;
            FlatTransitionFrame = 0;
            FlatTransitionPlaying = true;
            SoundController.instance.playSingleSound(SoundController.SoundT.Transformation);
        }

        for (int i = 0; i < TopMenuItems.Count; i++) {
            (TopMenuItems[i].children[3] as SRGUILabel).Style = TopStyleNormal;
            float itemDelay = i * 0.06f + previousDelay - 0.3f;

            TopMenuItems[i].Scale = new Vector2(0.2f, 0.2f);
            TransitionTweens.Add(HOTween.To(TopMenuItems[i], 0.2f, new TweenParms().Prop("Scale", new Vector2(1, 1)).Delay(itemDelay + FlatTransitionDelay)));
            TopMenuItems[i].Alpha = 0;
            TransitionTweens.Add(HOTween.To(TopMenuItems[i], 0f, new TweenParms().Prop("Alpha", 1).Delay(itemDelay + FlatTransitionDelay)));
        }
    }

    override protected void DoInternalUpdate() {
        base.DoInternalUpdate();
        if (FlatTransitionPlaying) {
            FlatTransitionTime += Time.deltaTime;
            int frame = Mathf.FloorToInt(FlatTransitionTime * FlatTransitionFPS);

            if (frame > FlatTransitionFrame) {
                if (FlatTransitionFrame < FlatTransitionSeq[SelectedFlatTransition].Length) {
                    FlatTransitionTexture.SetTexture(FlatTransitionSeq[SelectedFlatTransition][FlatTransitionFrame]);



                /*string path = "";
                switch (SelectedFlatTransition) {
                    case 0:
                        path = "media/transitions/t0/Movie_00_(limo.RGB_color.0000)0000";
                        break;
                    case 1:
                        path = "media/transitions/t1/Movie_03_(stamp.RGB_color.0000)0000";
                        break;
                    case 2:
                        path = "media/transitions/t2/Movie_00_(knife.RGB_color.0000)0000";
                        break;
                    case 3:
                        path = "media/transitions/t3/Movie_04_(system.RGB_color.0000)0000";
                        break;
                    case 4:
                        path = "media/transitions/t4/Movie_02_(sofa.RGB_color.0000)0000";
                        break;
                    case 5:
                        path = "media/transitions/t5/Movie_01_(pantone.RGB_color.0000)0000";
                        break;
                }

                path += string.Format("{0:D3}", frame);

                print(path);

                Texture LoadedText = Resources.Load<Texture>(path);
                if (LoadedText != null) { 
                    FlatTransitionTexture.SetTexture(LoadedText);*/
                    
                    FlatTransitionFrame = frame;
                } else {
                    //Resources.UnloadUnusedAssets();
                    FlatTransitionTexture.Enabled = false;
                    FlatTransitionPlaying = false;
                    unloadFlatTransitionAssets();

                    CentralObject.transform.parent = this.transform;

                    CentralObject.transform.localPosition = SelectedItemInPosition;
                    CentralObject.transform.rotation = Quaternion.Euler(0, 2.32194f, 0);
                    CentralObject.transform.localScale = new Vector3(1.342f, 1.2f, 1.1f);
                    CentralObject.GetComponent<Renderer>().material.color = AlphaColorOpaque;

                }
            }
        }
    }

    private void unloadFlatTransitionAssets() {
        /*if (FlatTransitionSeq[SelectedFlatTransition] != null) {
            for (int i = 0; i < FlatTransitionSeq[SelectedFlatTransition].Length; i++) {
                Resources.UnloadAsset(FlatTransitionSeq[SelectedFlatTransition][i]);
            }

            FlatTransitionSeq[SelectedFlatTransition] = null;

        }*/
    }

    override public void DoExit(string targetID) {
        base.DoExit(targetID);

        if (targetID != PageManager.ROOT_DESTINATION) {
            //ClickTransition(CentralObject.transform, 1.5f, new TweenParms().Prop("localPosition", new Vector3(3f, 0.01f, 13.01f)), new TweenParms().Prop("localPosition", new Vector3(0.45f, 0.01f, 13.01f)), 0.2f, 0, false);
            HOTween.To(CentralObject.transform, 1.5f, new TweenParms().Prop("localPosition", new Vector3(3f, 0.01f, 13.01f)));

            ExitTime = 1f;
        } else {
            ExitTime = 0.5f;  
        }
        
        for (int i = 0; i < TopMenuItems.Count; i++) {
            if (SelectedButt == TopMenuItems[i].children[4]) {
                (TopMenuItems[i].children[3] as SRGUILabel).Style = TopStyleSelected;
                TopMenuItems[i].Scale = new Vector2(1, 1);
                ClickTransition(TopMenuItems[i], 0.3f, new TweenParms().Prop("Scale", new Vector2(0.1f, 0.1f)), new TweenParms().Prop("Scale", new Vector2(1.1f, 1.1f)), 0.15f, 0f, false);
                TransitionTweens.Add(HOTween.To(TopMenuItems[i], 0f, new TweenParms().Prop("Alpha", 0).Delay(0.5f)));
            } else {
                TransitionTweens.Add(HOTween.To(TopMenuItems[i], 0.6f, new TweenParms().Prop("Alpha", 0).Delay(0f)));
            }   
        }
    }

    override internal void ClickReaction(SRBaseGUIElement caller) {
        SelectedButt = caller;
        base.ClickReaction(caller);

    }

}
