#region MIT

// 
// GMLib.
// Copyright (C) 2011, 2012, 2013, 2014 Michael Mercado
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//

#endregion

using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using GameMaker.Common;
using GameMaker.Resource;

namespace GameMaker.Project
{
    public class GMProject
    {
        #region Fields

        public event ProgressChangedHandler OnProgressChanged;
        public delegate void ProgressChangedHandler(string message, int percentage);

        public static int InstanceIdMin = 100000;
        public static int TileIdMin = 10000000;
        public int LastInstanceId = InstanceIdMin;
        public int LastTileId = TileIdMin;
        public int LastDataFileId = 0;
        public List<string> Assets = new List<string>();
        public GMVersionType GameMakerVersion = GMVersionType.GameMakerStudio;
        public GMNode ProjectTree = new GMNode();
        public GMSettings Settings = new GMSettings();
        public GMGameInformation GameInformation = new GMGameInformation();
        public GMList<GMTrigger> Triggers = new GMList<GMTrigger>();
        public GMList<GMSound> Sounds = new GMList<GMSound>();
        public GMList<GMSprite> Sprites = new GMList<GMSprite>();
        public GMList<GMBackground> Backgrounds = new GMList<GMBackground>();
        public GMList<GMPath> Paths = new GMList<GMPath>();
        public GMList<GMScript> Scripts = new GMList<GMScript>();
        public GMList<GMDataFile> DataFiles = new GMList<GMDataFile>();
        public GMList<GMFont> Fonts = new GMList<GMFont>();
        public GMList<GMTimeline> Timelines = new GMList<GMTimeline>();
        public GMList<GMObject> Objects = new GMList<GMObject>();
        public GMList<GMRoom> Rooms = new GMList<GMRoom>();
        public GMList<GMPackage> Packages = new GMList<GMPackage>();
        public GMList<GMLibrary> Libraries = new GMList<GMLibrary>();
        public GMList<GMShader> Shaders = new GMList<GMShader>();

        #endregion

        #region Methods

        #region Refactor

        /// <summary>
        /// Refactors the project's instance ids.
        /// </summary>
        public void RefactorInstanceIds()
        {
            // Reset the instance id.
            LastInstanceId = InstanceIdMin;

            // Iterate through rooms.
            foreach (GMRoom room in Rooms)
            {
                // If instances are empty, continue.
                if (room.Instances == null)
                    continue;

                // Iterate through instances.
                foreach (GMInstance instance in room.Instances)
                {
                    // Set instance id.
                    instance.Id = LastInstanceId;

                    // Increment project's last instance id.
                    LastInstanceId++;
                }
            }
        }

        /// <summary>
        /// Refactors the project's tile ids.
        /// </summary>
        public void RefactorTileIds()
        {
            // Reset the tile id.
            LastTileId = TileIdMin;

            // Iterate through rooms.
            foreach (GMRoom room in Rooms)
            {
                // If tiles are empty, continue.
                if (room.Tiles == null)
                    continue;

                // Iterate through tiles.
                foreach (GMTile tile in room.Tiles)
                {
                    // Set tile id.
                    tile.Id = LastTileId;

                    // Increment project's last tile id.
                    LastTileId++;
                }
            }
        }

        #endregion

        #region General

        /// <summary>
        /// Gets a valid random project id number.
        /// </summary>
        /// <returns>A valid random number for a project id.</returns>
        public int GetRandomProjectId()
        {
            Random random = new Random();
            return random.Next(0, 100000000);
        }

        #endregion

        #region Read

        /// <summary>
        /// Reads a Game Maker Studio project file
        /// </summary>
        private void ReadProjectGMS(string file)
        {
            // Set version
            GameMakerVersion = GMVersionType.GameMakerStudio;

            // Path with project file removed
            string folder = file.Remove(file.LastIndexOf("\\"));

            // Set up resource directory strings
            Dictionary<GMResourceType, string> directories = new Dictionary<GMResourceType, string>();
            directories.Add(GMResourceType.Assets, file);
            directories.Add(GMResourceType.DataFiles, file);
            directories.Add(GMResourceType.Configs, file);
            directories.Add(GMResourceType.Constants, file);
            directories.Add(GMResourceType.Hash, file);
            directories.Add(GMResourceType.Backgrounds, folder + "\\" + "background");
            directories.Add(GMResourceType.Objects, folder + "\\" + "objects");
            directories.Add(GMResourceType.Rooms, folder + "\\" + "rooms");
            directories.Add(GMResourceType.Sprites, folder + "\\" + "sprites");
            directories.Add(GMResourceType.Sounds, folder + "\\" + "sound");
            directories.Add(GMResourceType.TimeLines, folder + "\\" + "timelines");
            directories.Add(GMResourceType.Shaders, folder + "\\" + "shaders");
            directories.Add(GMResourceType.Scripts, folder + "\\" + "scripts");
            directories.Add(GMResourceType.Paths, folder + "\\" + "paths");

            // Resource load index
            int index = 0;

            // Iterate through directories
            foreach (KeyValuePair<GMResourceType, string> item in directories)
            {
                // Increment directory index
                index++;

                // If the directory does not exist, continue
                if (Path.GetExtension(item.Value) != ".gmx" && !Directory.Exists(item.Value))
                    continue;

                // Progress changed
                ProgressChanged("Reading " + item.Key.ToString() + "...", index, directories.Count);

                // Load data based on resource type
                switch (item.Key)
                {
                    case GMResourceType.Hash: Settings.Hash = ReadHashGMX(item.Value); break;
                    case GMResourceType.Assets: ProjectTree = GMNode.ReadTreeGMX(item.Value); Assets = (List<string>)ProjectTree.Tag; break;
                    case GMResourceType.DataFiles: DataFiles = GMDataFile.ReadDataFilesGMX(item.Value, out LastDataFileId); break;
                    case GMResourceType.Sprites: Sprites = GMSprite.ReadSpritesGMX(item.Value, ref Assets); break;
                    //case GMResourceType.Configs: Settings.Configs = GMSettings.GetConfigsGMX(item.Value); break;
                    //case GMResourceType.Constants: Settings.Constants = GMSettings.ReadConstantsGMX(item.Value); break;
                    case GMResourceType.Backgrounds: Backgrounds = GMBackground.ReadBackgroundsGMX(item.Value, ref Assets); break;
                    case GMResourceType.Objects: Objects = GMObject.ReadObjectsGMX(item.Value, ref Assets); break;
                    case GMResourceType.Rooms: Rooms = GMRoom.ReadRoomsGMX(item.Value, ref Assets, out LastTileId); break;
                    //case GMResourceType.TimeLines: Timelines = GMTimeline.ReadTimelinesGMX(item.Value, Assets); break;
                    //case GMResourceType.Sounds: Sounds = GMSound.ReadSoundsGMX(item.Value, ref Assets); break;
                    //case GMResourceType.Shaders: Shaders = GMShader.ReadShadersGMX(item.Value, ref Assets); break;
                    //case GMResourceType.Scripts: Scripts = GMScript.ReadScriptsGMX(item.Value, ref Assets); break;
                    //case GMResourceType.Paths: Paths = GMPath.ReadPathsGMX(item.Value, ref Assets); break;
                    //case GMResourceType.TimeLines: Timelines = GMTimeline.ReadTimelinesGMX(item.Value, Assets); break;
                }
            }

            // Retrieve tutorial data
            foreach (GMNode node in ProjectTree.Nodes)
            {
                // If the node is the tutorial state node and it has the nodes we're looking for
                if (node.ResourceType == GMResourceType.TutorialState && node.Nodes != null && node.Nodes.Length == 3)
                {
                    Settings.IsTutorial = node.Nodes[0].Nodes == null ? Settings.IsTutorial : GMResource.GMXBool(node.Nodes[0].Nodes[0].Name, true);
                    Settings.TutorialName = node.Nodes[1].Nodes == null ? Settings.TutorialName : GMResource.GMXString(node.Nodes[1].Nodes[0].FilePath, "");
                    Settings.TutorialPage = node.Nodes[2].Nodes == null ? Settings.TutorialPage : GMResource.GMXInt(node.Nodes[2].Nodes[0].Name, 0);
                }
            }

            // Progress event
            ProgressChanged("Finished Reading Project.", index, directories.Count);
        }

        /// <summary>
        /// Gets hash from project file
        /// </summary>
        /// <param name="path">File path to project file</param>
        private static string ReadHashGMX(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (XmlTextReader reader = new XmlTextReader(new StreamReader(fs)))
                {
                    reader.MoveToContent();
                    return reader.HasAttributes ? reader.GetAttribute(0) : "";
                }
            }
        }

        /// <summary>
        /// Reads a Game Maker project file
        /// </summary>
        public void ReadProject(string file)
        {
            // If the file does not exist, throw exception
            if (File.Exists(file) == false)
            {
                throw new Exception("The Game Maker project file does not exist.");
            }

            ReadProjectGMS(file);
        }

        /// <summary>
        /// Call the progress changed event
        /// </summary>
        /// <param name="message">Object state message</param>
        private void ProgressChanged(string message, long position, long length)
        {
            // If no event subscribers, return
            if (OnProgressChanged == null)
                return;

            // Progress event
            OnProgressChanged(message, (int)(position * 100 / length));
        }

        #endregion

        #endregion
    }
}
