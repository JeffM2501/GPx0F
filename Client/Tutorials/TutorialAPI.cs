﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.IO;

using Game.Maps;
using Client.Game;

using Urho;

namespace Client.Tutorials
{

    public static class TutorialAPI
    {
        public static App GameApp = null;
        public static Arena CurrentArena { get; internal set; } = null;
        public static LocalPlayer UserPlayer { get; internal set; } = null;

        public static void AddTextMessage(string messageText)
        {
            GameApp.SetHudMessage(messageText);
            if ( !string.IsNullOrEmpty(messageText))
            {
                var lines = messageText.Split("\n".ToCharArray());
                Array.Reverse(lines);
                foreach (var line in lines)
                    Game.Hud.ChatPanel.AddChatText(line, Game.Hud.ChatPanel.TutorialSource);
            }  
        }

        public static void SpawnTutorialPlayer(Vector3 position, Quaternion orintation)
        {
            if (UserPlayer == null)
            {
                GameApp.SpawnPlayer();
            }

            UserPlayer = GameApp.Me;
        }

        public interface ITutorial
        {
            string DisplayName { get; }
            string DisplayText { get; }

            bool UseSimpleArena { get; }

            int ArenaSize { get; }

            bool Enabled { get; }

            int SortOrder { get; }

            void Init(string langauge);

            void Startup();

            void Update(double timeStep);

            void Cleanup();
        }

        public static Dictionary<string, ITutorial> AvalilbleTutorials = new Dictionary<string, ITutorial>();

        internal static ITutorial CurrentTutorial = null;

        internal static void LoadTutorialsFromFile(Assembly file)
        {
            try
            {
                foreach (Type t in file.GetExportedTypes())
                {
                    if (t.GetInterface(typeof(ITutorial).Name) != null)
                    {
                        ITutorial tut = Activator.CreateInstance(t) as ITutorial;
                        tut.Init(CultureInfo.CurrentCulture.Name);

                        if (tut.Enabled)
                            AvalilbleTutorials.Add(tut.DisplayName, tut);
                    }
                }
            }
            catch (Exception)
            {
            }
           
        }
        internal static void LoadTutorialDir(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (var file in dir.GetFiles("*.dll"))
                {
                    try
                    {
                        LoadTutorialsFromFile(Assembly.LoadFrom(file.FullName));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        internal static void LoadTutorials()
        {
            Assembly thisAss = Assembly.GetExecutingAssembly();
            Tutorials.TutorialAPI.LoadTutorialsFromFile(thisAss);

            LoadTutorialDir(Path.Combine(Path.GetDirectoryName(thisAss.Location),"Tutorials"));

            SortTutorials();
        }

        internal static void SortTutorials()
        {
            SortedDictionary<int, List<ITutorial>> sortedTuts = new SortedDictionary<int, List<ITutorial>>();

            foreach (var tut in AvalilbleTutorials.Values)
            {
                if (!sortedTuts.ContainsKey(tut.SortOrder))
                    sortedTuts.Add(tut.SortOrder, new List<ITutorial>());

                sortedTuts[tut.SortOrder].Add(tut);
            }

            AvalilbleTutorials.Clear();
            foreach (var tutList in sortedTuts)
            {
                foreach (var tut in tutList.Value)
                {
                    AvalilbleTutorials.Add(tut.DisplayName, tut);
                }
            }
        }

        internal static bool StartTutorial(string name)
        {
            if (!AvalilbleTutorials.ContainsKey(name))
                return false;

            if (CurrentTutorial != null)
                CurrentTutorial.Cleanup();

            CurrentTutorial = AvalilbleTutorials[name];
            return true;
        }

        internal static void StopTutorial()
        {
            if (CurrentTutorial != null)
                CurrentTutorial.Cleanup();

            CurrentTutorial = null;
        }

        internal static void UpdateTutorial(double timeStep)
        {
            if (CurrentTutorial != null)
                CurrentTutorial.Update(timeStep);
        }
    }
}
