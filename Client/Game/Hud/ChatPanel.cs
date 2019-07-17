using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;
using Urho.Resources;

namespace Client.Game.Hud
{
    public class ChatPanel : BorderImage
    {
        public class LogLine : EventArgs
        {
            public int From = -1;
            public string Text = string.Empty;
        }

        public List<LogLine> TextLines = new List<LogLine>();
        public int TopVisLine = -1;

        public int FontSize = 12;
        public int LineSize = 16;

        public List<Text> VisibleLines = new List<Text>();

        public static readonly int LogSource = -2;
        public static readonly int ConnectionSource = -3;
        public static readonly int SystemSource = -1;
        public static readonly int TutorialSource = -4;

        private static event EventHandler<LogLine> DoLogAdd;

        protected class SoruceInfo
        {
            public int ID = 0;
            public string SourceName = string.Empty;
            public Color SourceColor = Color.White;
        }
        protected Dictionary<int, SoruceInfo> Sources = new Dictionary<int, SoruceInfo>();

        public void SetSourceInfo(int id, string name, Color color)
        {
            if (name == string.Empty)
            {
                if (Sources.ContainsKey(id))
                    Sources.Remove(id);
            }
            else
            {
                if (!Sources.ContainsKey(id))
                    Sources.Add(id, new SoruceInfo() { ID = id });

                Sources[id].SourceColor = color;
                Sources[id].SourceName = name;
            }
        }
        protected Color GetSourceColor(int id)
        {
            if (Sources.ContainsKey(id))
                return Sources[id].SourceColor;

            return Color.Red;
        }

        protected string GetSourceName(int id)
        {
            if (Sources.ContainsKey(id))
                return Sources[id].SourceName + ":";

            return string.Empty;
        }

        protected Font TextFont = null;

        public ChatPanel(): base()
        {
            DoLogAdd += ChatPanel_DoLogAdd;
        }

        public void Setup(ResourceCache res)
        {
            ImageRect = new IntRect(48, 0, 64, 16);
            Border = new IntRect(4, 4, 4, 4);

            TextFont = res.GetFont("Fonts/Exo2-Regular.otf");

            this.Resized += ChatPanel_Resized;

            SetSourceInfo(LogSource, ClientResources.LogSourceName, Color.FromHex("ff6738"));
            SetSourceInfo(SystemSource, ClientResources.SystemSourceName, Color.FromHex("6dcff6"));
            SetSourceInfo(ConnectionSource, ClientResources.NetworkSourceName, Color.FromHex("197b30"));
            SetSourceInfo(TutorialSource, ClientResources.TutorialSourceName, Color.FromHex("04CB68"));

            ComputeTextLines();
            LoadTextLines();
        }

        private void ChatPanel_Resized(ResizedEventArgs obj)
        {
            ComputeTextLines();
            LoadTextLines();
        }

        protected void ComputeTextLines()
        {
            foreach (var line in VisibleLines)
                line.Remove();

            VisibleLines.Clear();

            int h = Height / LineSize;

            for (int i = 0; i <= Height - LineSize; i += LineSize)
            {
                Text t = new Text();
                AddChild(t);
                t.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);
                t.SetSize(Width, LineSize);
                t.SetPosition(0, i * -1);
                t.SetFont(TextFont, FontSize);
                VisibleLines.Add(t);
                t.Visible = false;
            }
        }

        protected void LoadTextLines()
        {
            int index = 0;
            int startLine = TopVisLine;
            if (startLine < 0)
                startLine = TextLines.Count - 1;

            foreach(var line in VisibleLines)
            {
                int lineIndex = startLine - index;

                if (lineIndex >= TextLines.Count || lineIndex < 0)
                {
                    line.Visible = false;
                }
                else
                {
                    line.Visible = true;
                    line.Value = GetSourceName(TextLines[lineIndex].From) + TextLines[lineIndex].Text;
                    line.SetColor(GetSourceColor(TextLines[lineIndex].From));
                }

                index++;
            }
        }

        public static void AddChatText(string text, int from)
        {
            DoLogAdd?.Invoke(null, new LogLine() { From = from, Text = text });
        }

        private void ChatPanel_DoLogAdd(object sender, LogLine e)
        {
            TextLines.Add(e);
            LoadTextLines();
        }

        public void DoUpdate(float deltaTime, float elapsedTime)
        {
        }
    }
}
