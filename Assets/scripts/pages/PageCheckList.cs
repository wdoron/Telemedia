using UnityEngine;
using System.Collections;
using System.Xml;
using Holoville.HOTween;
using System.Collections.Generic;

public class PageCheckList : BasePage, IStatusDependant {

    private Dictionary<string, XmlNode> StatusNodesByCaption = new Dictionary<string, XmlNode>();
    private StatusManager StatusManagerInst;

    private XmlNode PageStatus;

    private List<List<SRGUICheckButton>> CheckTree = new List<List<SRGUICheckButton>>();
    //private int customFieldsCounter = 0;

    public Texture InputBackTexture;
    private int ColCount;

    private string Owner;
    private SRGUIInput CustomInput;

    override internal void InitPage(XmlNode pageData) {
        base.InitPage(pageData);

        StatusManagerInst = StatusManager.Instance;

        //business logic
        StatusManagerInst.RegisterPage(this);
        
        //2D
        //back butt
        createStandardBack();

        //logo
        createStandardLogo();

        //path
        createTitlePath(pageData.Attributes.GetNamedItem("path").Value, TitleTypes.Left);

        Owner = pageData.Attributes.GetNamedItem("owner").Value;

        //TransitionData transitionData;

        //int totalItems = CountItems(pageData);

        ColCount = 0;

        foreach (XmlNode col in pageData) {

            CreateItems(col, 380, true);
            ColCount++;
        }

    }

    void IStatusDependant.ResetStatus() {
        PageStatus = StatusManagerInst.GetPage(ID);
        StatusNodesByCaption.Clear();
        foreach (List<SRGUICheckButton> cbList in CheckTree) {
            foreach (SRGUICheckButton cb in cbList) {
                XmlNode statusNode = StatusManagerInst.GetStatus(PageStatus, cb.ID, false.ToString());
                cb.Checked = bool.Parse(statusNode.Attributes.GetNamedItem("value").Value);

                StatusNodesByCaption.Add(cb.ID, statusNode);
                if (cb.ID == "_") {
                    CustomInput.Text = PageStatus.Attributes.GetNamedItem("customData").Value;
                }
            }
        }

    }

    string IStatusDependant.getId() {
        return ID;
    }

    string IStatusDependant.getOwner() {
        return Owner;
    }


    private float CreateItems(XmlNode parentNode, float yPos, bool isMain) {
        float yPosAdded = 0;
        
        float MainGap = 70;
        float SecGap = 45;
        float TabSpace = 80;

        float MainTextIdent = 85;
        float SecTextIdent = 71;


        foreach (XmlNode childNode in parentNode) {
            if (isMain) {
                CheckTree.Add(new List<SRGUICheckButton>());
            }

            SRGUIContainer cont = new SRGUIContainer();

            string caption = childNode.Attributes.GetNamedItem("caption").Value;

            if (caption != "_") {
                SRGUILabel label = new SRGUILabel();
                label.Text = caption;
                label.Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.FrutiBlack80, 32);
                label.Position = new Vector2(((isMain) ? MainTextIdent : SecTextIdent), 0);
                cont.children.Add(label);
            } else {
                //caption = "custom" + customFieldsCounter;
                

                float inputWidth = 450;
                float inputHeight = 40;

                SRGUITexture back = new SRGUITexture();
                back.SetTexture(InputBackTexture);
                back.Position = new Vector2(((isMain) ? MainTextIdent : SecTextIdent), 0);
                cont.children.Add(back);

                CustomInput = new SRGUIInput();
                CustomInput.SetSize(new Vector2(inputWidth - 10, inputHeight));
                CustomInput.Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.FrutiBlack80, 32);
                CustomInput.Position = new Vector2(((isMain) ? MainTextIdent : SecTextIdent) + 6, 0);
                CustomInput.Style.alignment = TextAnchor.MiddleLeft;
                cont.children.Add(CustomInput);

                //customFieldsCounter++;
            }

            SRGUICheckButton checkButt = new SRGUICheckButton();
            checkButt.Position = new Vector2(0, 5);
            checkButt.SetTexture(AssetHolder.CheckBoxTextures);
            cont.children.Add(checkButt);

            cont.InPosition = new Vector2(160 + ((isMain) ? 0 : TabSpace) + ColCount * 740, (yPos + yPosAdded));
            cont.OutPosition = cont.InPosition + new Vector2(0, -10);

            checkButt.ID = caption;

            /*XmlNode statusNode = statusManager.GetStatus(PageStatus, checkButt.ID, false.ToString());
            checkButt.Checked = bool.Parse(statusNode.Attributes.GetNamedItem("value").Value);

            StatusNodesByCaption.Add(checkButt.ID, statusNode);*/


            GUIManager.RegisterGUIElement(cont, gameObject);

            TransitionData transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha, TransitionTypes.PositionGUI }, 0.2f, 0.2f, 0.3f);
            RegisterTransition(transitionData, cont);

            CheckTree[CheckTree.Count - 1].Add(checkButt);
    
            yPosAdded += (isMain) ? MainGap : SecGap;
            if (isMain && childNode.ChildNodes.Count > 0) {
                yPosAdded += CreateItems(childNode, yPos + yPosAdded, false) + MainGap;

            }
        }

        return  yPosAdded;
    }

    internal override void ClickReaction(SRBaseGUIElement caller) {
        base.ClickReaction(caller);
        if (caller is SRGUICheckButton) {

            SoundController.instance.playSingleSound(SoundController.SoundT.Checklist);

            int i = -1;
            int index = -1;
            do {
                i++;
                index = CheckTree[i].IndexOf(caller as SRGUICheckButton);
            } while (index < 0);

            if (index == 0) {
                //main
                if (CheckTree[i].Count > 1) {
                    for (int j = 1; j < CheckTree[i].Count; j++) {
                        CheckTree[i][j].Checked = (caller as SRGUICheckButton).Checked;
                        SetStatus(CheckTree[i][j].ID, CheckTree[i][j].Checked);
                    }
                }
            } else {
                //secondary

                bool anyChecked = false;

                for (int j = 1; j < CheckTree[i].Count; j++) {
                    anyChecked |= CheckTree[i][j].Checked;
                }

                CheckTree[i][0].Checked = anyChecked;
                SetStatus(CheckTree[i][0].ID, CheckTree[i][0].Checked);
            }

            SetStatus((caller as SRGUICheckButton).ID, (caller as SRGUICheckButton).Checked);
        }    
    }

    override internal void InputChangedReaction(SRGUIInput caller) {
        base.InputChangedReaction(caller);
        PageStatus.Attributes.GetNamedItem("customData").Value = CustomInput.Text;
        
    }

    private void SetStatus(string id, bool val){
        StatusNodesByCaption[id].Attributes.GetNamedItem("value").Value = val.ToString();
    }

    override public void DoEnter(float previousDelay, Object extraParams, string extraData = null) {
        base.DoEnter(previousDelay, extraParams, extraData);
    }

    override public void DoExit(string targetID) {
        base.DoExit(targetID);

    }

    override protected void DoInternalUpdate() {
        base.DoInternalUpdate();
    }

    /*void IStatusDependant.UpdateFromStatusNode(XmlNode page) {
        for (int i = 1; i < CheckTree.Count; i++) {
            for (int j = 1; j < CheckTree[i].Count; j++) {
                CheckTree[i][j].Checked = page
                    StatusNodesByCaption[id].Attributes.GetNamedItem("value").Value = val.ToString();
            }
        }
    }*/

}
