﻿using EvllyEngine;
using ProjectEvlly.src.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using ProjectEvlly.src.UI.Font;
using OpenTK.Graphics;

namespace ProjectEvlly.src.UI.GUIElements
{
    public class GUIButtom : GUIBase
    {
        public string TextureName;

        public string FontName = "PixelFont2";
        public Color TextColor = Color.Black;

        private FontRender fontRender;

        public GUIButtom(string Text, Rectangle rec) : base(rec)
        {
            Start(Text);
        }

        public GUIButtom(string Text,Rectangle rec, UIDock uIDock) : base(rec, uIDock)
        {
            Start(Text);
        }

        private void Start(string text)
        {
            fontRender = new FontRender(text, 22, FontName, new Vector2(0f, 0f), GetRectangle.Width, GetRectangle);
        }

        public override void OnResize()
        {
            if (fontRender != null)
            {
                fontRender.Resize(GetRectangle);
            }
            base.OnResize();
        }

        public override void RenderAfter()
        {
            if (fontRender != null)
            {
                fontRender.TickRender();
            }
            base.RenderAfter();
        }

        public override void Dispose()
        {
            if (fontRender != null)
            {
                fontRender.Dispose();
            }
            //_drawing.Dispose();
            //RendeTextOption = null;
            base.Dispose();
        }

        public override void RenderCustomValues()
        {
            if (TextureName != null)
            {
                if (!TextureName.Equals(string.Empty))
                {
                    AssetsManager.UseTexture(TextureName);
                    GUI.GetShader.Setbool("HaveTexture", true);
                }
                else
                {
                    GUI.GetShader.Setbool("HaveTexture", false);
                }
            }
            base.RenderCustomValues();
        }

        public override void Click()
        {
            base.Click();
        }

        public void SetText(string text)
        {
            if (fontRender != null)
            {
                fontRender.UpdateText(text);
            }
        }

        public void SetColor(Color4 color)
        {
            if (fontRender != null)
            {
                fontRender.SetColor(color);
            }
        }

        public void SetTextAling(TextAling textAling)
        {
            if (fontRender != null)
            {
                fontRender.textAling = textAling;
            }
        }
    }
}
