using UnityEngine;
using System.Collections;
using System.Xml;
using Holoville.HOTween;
using System.Collections.Generic;

public class PageSubTopic : BasePage {

    public Texture SideArrowTexture;

    private List<SRGUIButton> NextPageButtons = new List<SRGUIButton>();
    private List<GUIStyle[]> NextPageStyles = new List<GUIStyle[]>();
    private List<SRGUIContainer> NextPageButtConts = new List<SRGUIContainer>();
    private SRBaseGUIElement SelectedButt;

    /*private GUIStyle NavigationStyleLeftNormal;
    private GUIStyle NavigationStyleLeftSelected;
    private GUIStyle NavigationStyleRightNormal;
    private GUIStyle NavigationStyleRightSelected;*/

    override internal void InitPage(XmlNode pageData) {
        base.InitPage(pageData);

        /*NavigationStyleLeftNormal = AssetHolder.GetCustomStyle(AssetHolder.NextPageButtTextures[0], CommonAssetHolder.FontNameType.FrutiGray, 42, false, false, true);
        NavigationStyleLeftSelected = AssetHolder.GetCustomStyle(AssetHolder.NextPageButtTextures[0], CommonAssetHolder.FontNameType.CorpBold, 42, true);
        NavigationStyleRightNormal = AssetHolder.GetCustomStyle(AssetHolder.NextPageButtTextures[1], CommonAssetHolder.FontNameType.FrutiGray, 42, false, false, true);
        NavigationStyleRightSelected = AssetHolder.GetCustomStyle(AssetHolder.NextPageButtTextures[1], CommonAssetHolder.FontNameType.CorpBold, 42, true);

        NavigationStyleLeftNormal.alignment = NavigationStyleLeftSelected.alignment = NavigationStyleRightNormal.alignment = NavigationStyleRightSelected.alignment = TextAnchor.MiddleCenter;*/

        //back butt
        createStandardBack();

        //logo
        createStandardLogo();

        //path
        createTitlePath(pageData.Attributes.GetNamedItem("path").Value, TitleTypes.Center);

        TransitionData transitionData;
        
        foreach (XmlNode childNode in pageData) {
            if (childNode.Name == "items") {

                ISRGUIDistributable[] DistributeGroup = new ISRGUIDistributable[childNode.ChildNodes.Count];

                int itemCount = 0;
                foreach (XmlNode caption in childNode) {
                    SRGUIContainer cont = new SRGUIContainer();
                    SRGUILabel label = new SRGUILabel();
                    label.Text = caption.Attributes.GetNamedItem("caption").Value;
                    label.Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.FrutiBlack80, 36);
                    
                    float xOffset = 0;
                    if (label.Text.Substring(0, 1) == "-") {
                        xOffset = 45;
                        label.Text = label.Text.Substring(1, label.Text.Length - 1);
                    }

                    SRGUITexture arrow = new SRGUITexture();
                    arrow.SetTexture(SideArrowTexture);
                    arrow.Position = new Vector2(-34, 9);

                    cont.children.Add(label);
                    cont.children.Add(arrow);

                    //cont.InPosition = new Vector2(230, 240 + itemCount * (420f / (childNode.ChildNodes.Count)));
                    cont.InPosition = new Vector2(248 + xOffset, 270 + itemCount * 56);
                    cont.OutPosition = cont.InPosition + new Vector2(0, -10);

                    GUIManager.RegisterGUIElement(cont, gameObject);

                    transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha, TransitionTypes.PositionGUI }, 0.2f, 0f , 0.3f);
                    RegisterTransition(transitionData, cont);

                    DistributeGroup[itemCount] = cont;

                    itemCount++;
                }

                LabelDistributeData DistributeData = new LabelDistributeData();
                DistributeData.SpaceBetweenItems = 43;

                RedistributeLabelsY(new ISRGUIDistributable[][] { DistributeGroup }, new LabelDistributeData[] { DistributeData}, 405, 700, -1);

            } else if (childNode.Name == "navigation") {
                int itemCount = 0;
                float itemWidth = AssetHolder.NextPageButtTextures[0].width;

                GUIContent testContent = new GUIContent();

                foreach (XmlNode button in childNode) {


                    SRGUIContainer NextPageButtCont = new SRGUIContainer();
                    NextPageButtCont.Position = new Vector2(SRGUIManager.UiWidth / 2 - itemWidth * childNode.ChildNodes.Count / 2 + itemCount * itemWidth - 5, 820);

                    NextPageButtCont.InScale = new Vector2(1, 1);
                    NextPageButtCont.OutScale = new Vector2(0.2f, 0.2f);


                    SRGUIButton NextPageButt = new SRGUIButton();
                    NextPageButt.destination = button.Attributes.GetNamedItem("dest").Value;


                    NextPageStyles.Add(new GUIStyle[2]);
                    NextPageStyles[itemCount][0] = (itemCount == 0) ? AssetHolder.GetCustomStyle(AssetHolder.NextPageButtTextures[0], CommonAssetHolder.FontNameType.FrutiGray, 42, false, false, true) : AssetHolder.GetCustomStyle(AssetHolder.NextPageButtTextures[1], CommonAssetHolder.FontNameType.FrutiGray, 42, false, false, true);
                    NextPageStyles[itemCount][1] = (itemCount == 0) ? AssetHolder.GetCustomStyle(AssetHolder.NextPageButtTextures[0], CommonAssetHolder.FontNameType.CorpBold, 42, true):AssetHolder.GetCustomStyle(AssetHolder.NextPageButtTextures[1], CommonAssetHolder.FontNameType.CorpBold, 42, true);
                    NextPageStyles[itemCount][0].alignment = NextPageStyles[itemCount][1].alignment = TextAnchor.MiddleCenter;

                    NextPageButt.Style = NextPageStyles[NextPageStyles.Count - 1][0];
                    
                    NextPageButt.Text = button.Attributes.GetNamedItem("caption").Value;
                    NextPageButt.HasSound = true;
                    SRGUILabel subButtTitle = null;

                    if (button.Attributes.GetNamedItem("subCaption") != null) {
                        subButtTitle = new SRGUILabel();
                        NextPageButt.Text = NextPageButt.Text + "\n ";
                        
                        subButtTitle.Text = button.Attributes.GetNamedItem("subCaption").Value;
                        subButtTitle.Style = AssetHolder.GetCustomStyle(AssetHolder.NextPageButtTextures[0], CommonAssetHolder.FontNameType.FrutiGray, 25, false, false, true);
                        subButtTitle.Position = new Vector2(150 - subButtTitle.Size.x / 2, 60);
                        
                    }
                    
                    
                    testContent.text = NextPageButt.Text;


                    while (NextPageButt.Style.CalcSize(testContent).x > NextPageButt.Size.x || NextPageButt.Style.CalcSize(testContent).y > NextPageButt.Size.y) {
                            
                        NextPageButt.Style.fontSize--;
                    }


                    NextPageButtCont.Axis = new Vector2(NextPageButt.Size.x / 2, NextPageButt.Size.y / 2);

                    NextPageButtons.Add(NextPageButt);
                    

                    SRGUITexture NextPageshadow = new SRGUITexture();
                    NextPageshadow.SetTexture(AssetHolder.NextPageButtTextures[2]);
                    NextPageshadow.Position = new Vector2(NextPageButt.Position.x - ((itemCount == 0) ? 10 : 15), NextPageButt.Position.y + 110);



                    GUIManager.RegisterGUIElement(NextPageButtCont, gameObject);


                    NextPageButtCont.children.Add(NextPageButt);
                    NextPageButtCont.children.Add(NextPageshadow);
                    if (subButtTitle != null) {
                        NextPageButtCont.children.Add(subButtTitle);
                    }

                    NextPageButtConts.Add(NextPageButtCont);




                    itemCount++;
                }
            }
        }


    }

    override public void DoEnter(float previousDelay, Object extraParams, string extraData = null) {
        base.DoEnter(previousDelay, extraParams, extraData);

        for (int i = 0; i < NextPageButtons.Count; i++) {
            //NextPageButtons[i].Style = (i == 0) ? NavigationStyleLeftNormal : NavigationStyleRightNormal;
            NextPageButtons[i].Style = NextPageStyles[i][0];
            NextPageButtConts[i].Alpha = 0;
            NextPageButtConts[i].Scale = new Vector2(0.2f, 0.2f);
            TransitionTweens.Add(HOTween.To(NextPageButtConts[i], 0.2f, new TweenParms().Prop("Alpha", 1).Delay(0.1f + i * 0.2f)));
            TransitionTweens.Add(HOTween.To(NextPageButtConts[i], 0.3f, new TweenParms().Prop("Scale", new Vector2(1, 1)).Delay(0.2f + i * 0.2f)));
            
        }

    }

    override public void DoExit(string targetID) {
        base.DoExit(targetID);
        for (int i = 0; i < NextPageButtons.Count; i++) {
            if (SelectedButt == NextPageButtons[i]) {



                NextPageButtons[i].Style = NextPageStyles[i][1];
                NextPageButtConts[i].Scale = new Vector2(1, 1);
                ClickTransition(NextPageButtConts[i], 0.3f, new TweenParms().Prop("Scale", new Vector2(0.1f, 0.1f)), new TweenParms().Prop("Scale", new Vector2(1.2f, 1.2f)), 0.2f, 0f, false);
                TransitionTweens.Add(HOTween.To(NextPageButtConts[i], 0f, new TweenParms().Prop("Alpha", 0).Delay(0.5f)));
                
            } else {
                NextPageButtConts[i].Alpha = 1;
                TransitionTweens.Add(HOTween.To(NextPageButtConts[i], 0.6f, new TweenParms().Prop("Alpha", 0).Delay(0.2f)));
            }
        }
    }

    override protected void DoInternalUpdate() {
        base.DoInternalUpdate();
    }

    override internal void ClickReaction(SRBaseGUIElement caller) {
        SelectedButt = caller;
        base.ClickReaction(caller);

    }
}
