﻿using System;
using System.Collections.Generic;
using TiledMapParser;
using GLXEngine;

namespace GameProject
{
    class TiledSceneContructor
    {
        static Map map;

        static public void LoadObjects(Scene a_scene, string a_mapFile)
        {
            map = tileMapLoad(a_mapFile);

            a_scene.width = map.Width * map.TileWidth;
            a_scene.height = map.Height * map.TileHeight;

            int[,] backgroundTiles = tilesLoad(0);
            List<TiledObject> tiledObjects = new List<TiledObject>(objectsLoad());
            List<TiledObject> tilesCircles = new List<TiledObject>(circlesLoad());
            string tileSheetFileName = map.TileSets[0].Source;
            TileSet tileSet = TiledParser.ReadTileSet(tileSheetFileName);
            List<string> objectTypes = new List<string>();
            tiledObjects.ForEach(tiledObject => { objectTypes.Add(tiledObject.Type); });
            List<string> objectNames = new List<string>();
            tiledObjects.ForEach(tiledObject => { objectNames.Add(tiledObject.Name); });

            //load background tiles
            for (int i = 0; i < backgroundTiles.GetLength(1); i++)
            {
                for (int j = 0; j < backgroundTiles.GetLength(0); j++)
                {
                    if (backgroundTiles[j, i] == 0) continue;
                    AnimationSprite sprite = new AnimationSprite(tileSet.Image.FileName, tileSet.Columns, tileSet.Rows);
                    sprite.SetFrame(backgroundTiles[j, i] - 1);
                    BackgroundTile tile = new BackgroundTile(a_scene, sprite);
                    tile.x = j * map.TileWidth;
                    tile.y = i * map.TileHeight;
                    a_scene.AddChild(tile);
                }
            }

            //load objectTypes
            for (int i = 0; i < objectTypes.Count; i++)
            {
                if (objectTypes[i] == null) continue;

                GameObject gameObject = null;

                if (objectTypes[i] == "PickUp")
                {
                    Sprite sprite = new Sprite("Textures/key.png");
                    gameObject = new PickUp(a_scene, sprite, tiledObjects[i].Name);
                }
                else if (objectTypes[i] == "Door")
                {
                    AnimationSprite sprite = new AnimationSprite(tileSet.Image.FileName, tileSet.Columns, tileSet.Rows);
                    sprite.SetFrame(tiledObjects[i].GID - 1);
                    gameObject = new Door(a_scene, sprite, a_scene.m_player as Player, tiledObjects[i].GetIntProperty("ID"));
                }
                else if (objectTypes[i] == "Spawnpoint")
                {
                    a_scene.m_player.x = tiledObjects[i].X;
                    a_scene.m_player.y = tiledObjects[i].Y;
                }
                else if (objectTypes[i] == "Collidable")
                {
                    BoundsObject obj = new BoundsObject(a_scene, tiledObjects[i].Width, tiledObjects[i].Height);
                    obj.x = tiledObjects[i].X;
                    obj.y = tiledObjects[i].Y;
                    obj.rotation = tiledObjects[i].rotation;
                    a_scene.AddChild(obj);
                }

                if (gameObject == null) continue;

                a_scene.AddChild(gameObject);
                gameObject.x = tiledObjects[i].X;
                gameObject.y = tiledObjects[i].Y - tileSet.TileHeight;
            }

            Goal goalObject = new Goal(a_scene);

            for (int i = 0; i < objectNames.Count; i++)
            {
                if (objectNames[i] == null) continue;

                GameObject gameObject = null;
                float rotation = 0;

                if (objectNames[i].IndexOf("Rot") >= 0)
                {
                    objectNames[i] = objectNames[i].Substring(3);
                    if (objectNames[i].Substring(0, 1) == "R")
                        rotation = 1;
                    else
                        rotation = -1;

                    objectNames[i] = objectNames[i].Substring(1);
                }

                if (objectNames[i] == "magnet")
                {
                    Sprite sprite = new Sprite("Textures/key.png");
                    gameObject = new PickUp(a_scene, sprite, tiledObjects[i].Name);
                }
                else if (objectNames[i] == "fan")
                {
                    AnimationSprite sprite = new AnimationSprite(tileSet.Image.FileName, tileSet.Columns, tileSet.Rows);
                    sprite.SetFrame(tiledObjects[i].GID - 1);
                    gameObject = new Door(a_scene, sprite, a_scene.m_player as Player, tiledObjects[i].GetIntProperty("ID"));                   // conveyorbelt R/L
                }                                                                                                                               // fan
                else if (objectNames[i] == "snapbox")
                {

                }
                else if (objectNames[i] == "spawnpoint")                                                                                        // magnet
                {                                                                                                                               // RotR/L prefit for rotational
                    a_scene.m_player.x = tiledObjects[i].X;                                                                                     // spawnpoint
                    a_scene.m_player.y = tiledObjects[i].Y;                                                                                     // snapbox
                }                                                                                                                               // goal
                else if (objectNames[i] == "-")                                                                                                 // empty boxes = colission
                {
                    BoundsObject obj = new BoundsObject(a_scene, tiledObjects[i].Width, tiledObjects[i].Height);
                    obj.x = tiledObjects[i].X;
                    obj.y = tiledObjects[i].Y;
                    obj.rotation = tiledObjects[i].rotation;
                    a_scene.AddChild(obj);
                }

                if (gameObject == null) continue;

                a_scene.AddChild(gameObject);
                gameObject.x = tiledObjects[i].X;
                gameObject.y = tiledObjects[i].Y - tileSet.TileHeight;
            }
        }

        static private Map tileMapLoad(string filename)
        {
            Map _map = TiledParser.ReadMap(filename);
            return _map;
        }

        static private int[,] tilesLoad(int layer)
        {
            int[,] output = new int[map.Width, map.Height];
            string[] tiles = map.Layers[layer].Data.innerXML.Split(',');
            for (int i = 0; i < map.Height; i++)
            {
                for (int j = 0; j < map.Width; j++)
                {
                    output[j, i] = Convert.ToInt32(tiles[i * map.Width + j]);
                }
            }
            return output;
        }

        static private TiledObject[] objectsLoad()
        {
            return map.ObjectGroups[0].Objects;
        }

        static private TiledObject[] circlesLoad()
        {
            return map.ObjectGroups[0].Circles;
        }
    }
}
