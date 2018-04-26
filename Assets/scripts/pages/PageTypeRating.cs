using UnityEngine;
using System.Collections;
using System.Xml;
using Holoville.HOTween;

public class PageTypeRating : BasePage {
    private SRGUILabel Title;

    private string[] titles;

    public Texture2D[] contents;
    private SRGUITexture Content;

    override internal void InitPage(XmlNode pageData) {
        base.InitPage(pageData);


        //back butt
        createStandardBack();

        //logo
        createStandardLogo();

        titles = new string[pageData.ChildNodes.Count];

        int count = 0;

        foreach (XmlNode type in pageData) {
            titles[count] = type.Attributes.GetNamedItem("caption").Value;
            count++;
        }


        Title = createTitlePath(pageData.Attributes.GetNamedItem("path").Value + "%Temp", TitleTypes.LargeInPath);

        Content = new SRGUITexture();
        GUIManager.RegisterGUIElement(Content, gameObject);

    }

    override public void DoEnter(float previousDelay, Object extraParams, string extraData = null) {
        base.DoEnter(previousDelay, extraParams, extraData);

        Title.Text = titles[int.Parse(extraData)];

        Content.SetTexture(contents[int.Parse(extraData)]);
        Content.InPosition = new Vector2(0, SRGUIManager.UiHeight - Content.Size.y);
        Content.OutPosition = Content.InPosition + new Vector2(0, 10);
        
        Content.Alpha = 0;
        Content.Position = Content.OutPosition;
        HOTween.To(Content, 0.2f, new TweenParms().Prop("Alpha", 1).Prop("Position", Content.InPosition).Delay(previousDelay) );
    }

    override public void DoExit(string targetID) {
        base.DoExit(targetID);

        Content.Alpha = 1;
        Content.Position = Content.InPosition;
        HOTween.To(Content, 0.2f, new TweenParms().Prop("Alpha", 0).Prop("Position", Content.OutPosition));
    }
}
