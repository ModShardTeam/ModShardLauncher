﻿using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using UndertaleModLib.Models;
using UndertaleModLib;
using System.Text.RegularExpressions;
using System.Collections;
using System.Drawing.Imaging;
using Serilog;
using UndertaleModLib.Util;

namespace ModShardLauncher
{
    public class TextureInfo
    {
        public string Source;
        public byte[] Data;
        public int Width;
        public int Height;
    }
    public enum SplitType
    {
        Horizontal,
        Vertical
    }
    public enum BestFitHeuristic
    {
        Area,
        MaxOneAxis,
    }
    public class Node
    {
        public Rectangle Bounds;
        public TextureInfo Texture;
        public SplitType SplitType;
    }
    public class Atlas
    {
        public int Width;
        public int Height;
        public List<Node> Nodes;
    }
    public class TextureLoader
    {
        public static List<TextureInfo> SourceTextures = new();
        public static StringWriter LogWriter;
        public static StringWriter Error;
        public static int padding;
        public static int AtlasSize;
        public static BestFitHeuristic FitHeuristic;
        public static List<Atlas> Atlasses;
        public static Regex sprFrameRegex = new(@"^(.+?)(?:_(-*\d+))*$", RegexOptions.Compiled);
        public static UndertaleData Data => DataLoader.data;
        public TextureLoader()
        {
            SourceTextures = new List<TextureInfo>();
            LogWriter = new StringWriter();
            Error = new StringWriter();
        }
        public static void LoadTextures(ModFile mod)
        {
            Process(mod, 2048, 2, false);
            foreach (Atlas atlas in Atlasses)
            {
                // read an atlas as an image
                Image image = CreateAtlasImage(atlas);

                // save the image in a memory stream
                MemoryStream memoryStream = new();
                image.Save(memoryStream, ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // create a new embedded texture
                UndertaleEmbeddedTexture ueTexture = new()
                {
                    Name = Data.Strings.MakeString(mod.Name)
                };

                // вместо TextureBlob используем GMImage
                ueTexture.TextureData.Image = GMImage.FromPng(memoryStream.ToArray());

                // добавляем новую текстуру в Data
                Data.EmbeddedTextures.Add(ueTexture);


                // convert the image as a bitmap to compute the collision mask
                Bitmap atlasBitmap = new(image);
                foreach (Node node in atlas.Nodes)
                {
                    if(node.Texture != null)
                    {
                        // create a new texture page item and add it in Data
                        UndertaleTexturePageItem texturePageItem = Msl.CreateTexureItem(
                            ueTexture, 
                            new RectTexture((ushort)node.Bounds.X, (ushort)node.Bounds.Y, (ushort)node.Bounds.Width, (ushort)node.Bounds.Height), 
                            new RectTexture(0, 0, (ushort)node.Bounds.Width, (ushort)node.Bounds.Height), 
                            new BoundingData<ushort>((ushort)node.Bounds.Width, (ushort)node.Bounds.Height)
                        );
                        Data.TexturePageItems.Add(texturePageItem);

                        // this texture page item can be injected in a sprite
                        // reading the name of the png
                        // to find the frame number and the name of the sprite associated
                        string stripped = Path.GetFileNameWithoutExtension(node.Texture.Source);
                        string spriteName;
                        int frame;
                        
                        try
                        {
                            System.Text.RegularExpressions.Match spriteParts = sprFrameRegex.Match(stripped);
                            spriteName = spriteParts.Groups[1].Value;
                            int.TryParse(spriteParts.Groups[2].Value, out frame);
                        }
                        catch 
                        {
                            continue;
                        }

                        // find the sprite
                        UndertaleSprite sprite = Data.Sprites.ByName(spriteName);
                        // create a textureEntry from the texture page item
                        UndertaleSprite.TextureEntry textureEntry = new()
                        {
                            Texture = texturePageItem
                        };

                        // test if the sprite exists or not
                        if (sprite == null)
                        {
                            // clone the image
                            Rectangle bmpRect = new(node.Bounds.X, node.Bounds.Y, node.Bounds.Width, node.Bounds.Height);
                            PixelFormat format = atlasBitmap.PixelFormat;
                            Bitmap clone = atlasBitmap.Clone(bmpRect, format);

                            // create a new sprite
                            UndertaleSprite newSprite = Msl.CreateSpriteNoCollisionMasks(
                                spriteName,
                                new MarginData(0, node.Bounds.Height - 1, 0, node.Bounds.Width - 1),
                                new OriginData(0, 0),
                                new BoundingData<uint>((uint)node.Bounds.Width, (uint)node.Bounds.Height)
                            );

                            // populate the list of textures with null
                            // indeed, we want to add the texture at the position frame
                            // since the sprite didnt exist, we need to make the frame - 1 first texture as null
                            for (int i = 0; i < frame; i++)
                                newSprite.Textures.Add(null);

                            // create a new mask for this sprite but it seems Stoneshard does not use collision mask
                            newSprite.CollisionMasks.Add(newSprite.NewMaskEntry());

                            // ?
                            int correctedWidth = (node.Bounds.Width + 7) / 8 * 8;

                            // using a bit array to transpose masks, ?
                            BitArray maskingBitArray = new(correctedWidth * node.Bounds.Height);
                            BitArray bitArray = new(correctedWidth * node.Bounds.Height);

                            // loop on color off the image pixel by pixel
                            // and make a mask for alpha > 0
                            for(int y = 0; y < node.Bounds.Height; y++)
                            {
                                for (int x = 0; x < node.Bounds.Width; x++)
                                {
                                    Color pixelColor = clone.GetPixel(x, y);
                                    maskingBitArray[y * correctedWidth + x] = pixelColor.A > 0;
                                }
                            }
                            
                            // ?
                            for(int i = 0; i < maskingBitArray.Length; i += 8)
                            {
                                for(int j = 0; j < 8; j++)
                                {
                                    bitArray[j + i] = maskingBitArray[-(j - 7) + i];
                                }
                            }

                            // convert the bitArray into an array of bytes
                            byte[] bytes = new byte[maskingBitArray.Length / 8];
                            bitArray.CopyTo(bytes, 0);

                            // inject the array of bytes in the collision mask
                            for(int i = 0; i < bytes.Length; i++)
                                newSprite.CollisionMasks[0].Data[i] = bytes[i];
                            
                            // put the texture page item in the frame index
                            newSprite.Textures.Add(textureEntry);

                            // add the new Sprite in data
                            Data.Sprites.Add(newSprite);
                            continue;
                        }

                        // the sprite already exists
                        // we then need to check the list of textures
                        // we want to add the texture page item at the frame index
                        // if the index is out of bound, fill the list with the texture page item until we reach frame - 1
                        if (frame > sprite.Textures.Count - 1)
                        {
                            while (frame > sprite.Textures.Count - 1)
                            {
                                sprite.Textures.Add(textureEntry);
                            }
                            continue;
                        }

                        // put the texture page item at the frame index
                        sprite.Textures[frame] = textureEntry;
                    }
                }
            }
            Atlasses.Clear();
        }
        public static void Process(ModFile mod, int _AtlasSize, int _Padding, bool _DebugMode)
        {
            padding = _Padding;
            AtlasSize = _AtlasSize;

            // scan all textures in mod
            // and store them in SourceTextures
            ScanForTextures(mod);
            List<TextureInfo> textures = SourceTextures.ToList();

            //2: generate as many atlasses as needed (with the latest one as small as possible)
            Atlasses = new List<Atlas>();
            while (textures.Count > 0)
            {
                Atlas atlas = new()
                {
                    Width = _AtlasSize,
                    Height = _AtlasSize
                };

                List<TextureInfo> leftovers = LayoutAtlas(textures, atlas);

                if (leftovers.Count == 0)
                {
                    // we reached the last atlas. Check if this last atlas could have been twice smaller
                    while (leftovers.Count == 0)
                    {
                        atlas.Width /= 2;
                        atlas.Height /= 2;
                        leftovers = LayoutAtlas(textures, atlas);
                    }
                    // we need to go 1 step larger as we found the first size that is to small
                    atlas.Width *= 2;
                    atlas.Height *= 2;
                    leftovers = LayoutAtlas(textures, atlas);
                }

                Atlasses.Add(atlas);

                textures = leftovers;
            }
        }
        public static void SaveAtlasses(string destination)
        {
            int atlasCount = 0;
            string prefix = destination.Replace(Path.GetExtension(destination), "");
            StreamWriter tw = new(destination);

            tw.WriteLine("source_tex, atlas_tex, u, v, scale_u, scale_v");

            foreach (Atlas atlas in Atlasses)
            {
                string atlasName = string.Format(prefix + "{0:000}" + ".png", atlasCount);

                //1: Save images
                Image img = CreateAtlasImage(atlas);
                img.Save(atlasName, ImageFormat.Png);

                //2: save description in file
                foreach (Node n in atlas.Nodes)
                {
                    if (n.Texture != null)
                    {
                        tw.Write(n.Texture.Source + ", ");
                        tw.Write(atlasName + ", ");
                        tw.Write(((float)n.Bounds.X / atlas.Width).ToString() + ", ");
                        tw.Write(((float)n.Bounds.Y / atlas.Height).ToString() + ", ");
                        tw.Write(((float)n.Bounds.Width / atlas.Width).ToString() + ", ");
                        tw.WriteLine(((float)n.Bounds.Height / atlas.Height).ToString());
                    }
                }

                ++atlasCount;
            }
            tw.Close();

            tw = new StreamWriter(prefix + ".log");
            tw.WriteLine("--- LOG -------------------------------------------");
            tw.WriteLine(LogWriter.ToString());
            tw.WriteLine("--- ERROR -----------------------------------------");
            tw.WriteLine(Error.ToString());
            tw.Close();
        }
        // scan all png packed in the modFile
        // and add them in a List<TextureInfo>
        private static void ScanForTextures(ModFile modFile)
        {
            SourceTextures.Clear();
            // look for all elements in the modFile
            // and save the png ones
            foreach (FileChunk fileChunk in modFile.Files)
            {
                // all non png files are not images
                if (!fileChunk.name.EndsWith("png")) continue;
                // icon.png is tied to the mod itself
                if (fileChunk.name == modFile.Name + "\\" + "icon.png") continue;

                // GetFile can fail
                try
                {
                    // open the image with a memory stream
                    byte[] byteFile = modFile.GetFile(fileChunk.name);
                    using Image image = Image.FromStream(new MemoryStream(byteFile));

                    // check if the image loaded is correct regarding the AtlasSize
                    if (image != null && image.Width <= AtlasSize && image.Height <= AtlasSize)
                    {
                        // create a new texture information from the image
                        TextureInfo textureInfo = new()
                        {
                            Width = image.Width,
                            Height = image.Height,
                            Source = fileChunk.name.Split("\\")[^1],
                            Data = byteFile
                        };
                        Log.Information(string.Format("Successfully load texture {0}", fileChunk.name));
                        SourceTextures.Add(textureInfo);
                    }
                    else
                    {
                        throw new BadImageFormatException("Cannot load the image {0}", fileChunk.name);
                    }
                }
                catch(Exception ex)
                {
                    Log.Error(ex, "Something went wrong");
                }
            }
        }
        private static void HorizontalSplit(Node _ToSplit, int _Width, int _Height, List<Node> _List)
        {
            Node n1 = new();
            n1.Bounds.X = _ToSplit.Bounds.X + _Width + padding;
            n1.Bounds.Y = _ToSplit.Bounds.Y;
            n1.Bounds.Width = _ToSplit.Bounds.Width - _Width - padding;
            n1.Bounds.Height = _Height;
            n1.SplitType = SplitType.Vertical;

            Node n2 = new();
            n2.Bounds.X = _ToSplit.Bounds.X;
            n2.Bounds.Y = _ToSplit.Bounds.Y + _Height + padding;
            n2.Bounds.Width = _ToSplit.Bounds.Width;
            n2.Bounds.Height = _ToSplit.Bounds.Height - _Height - padding;
            n2.SplitType = SplitType.Horizontal;

            if (n1.Bounds.Width > 0 && n1.Bounds.Height > 0)
                _List.Add(n1);
            if (n2.Bounds.Width > 0 && n2.Bounds.Height > 0)
                _List.Add(n2);
        }
        private static void VerticalSplit(Node _ToSplit, int _Width, int _Height, List<Node> _List)
        {
            Node n1 = new();
            n1.Bounds.X = _ToSplit.Bounds.X + _Width + padding;
            n1.Bounds.Y = _ToSplit.Bounds.Y;
            n1.Bounds.Width = _ToSplit.Bounds.Width - _Width - padding;
            n1.Bounds.Height = _ToSplit.Bounds.Height;
            n1.SplitType = SplitType.Vertical;

            Node n2 = new();
            n2.Bounds.X = _ToSplit.Bounds.X;
            n2.Bounds.Y = _ToSplit.Bounds.Y + _Height + padding;
            n2.Bounds.Width = _Width;
            n2.Bounds.Height = _ToSplit.Bounds.Height - _Height - padding;
            n2.SplitType = SplitType.Horizontal;

            if (n1.Bounds.Width > 0 && n1.Bounds.Height > 0)
                _List.Add(n1);
            if (n2.Bounds.Width > 0 && n2.Bounds.Height > 0)
                _List.Add(n2);
        }
        private static bool MaxOneAxisFit(Node node, TextureInfo ti, out float ratio)
        {
            if (ti.Width <= node.Bounds.Width && ti.Height <= node.Bounds.Height)
            {
                float wRatio = ti.Width / (float)node.Bounds.Width;
                float hRatio = ti.Height / (float)node.Bounds.Height;
                ratio = wRatio > hRatio ? wRatio : hRatio;

                return true;
            }
            ratio = 0.0f;
            return false;
        }
        private static bool AreaFit(Node node, TextureInfo ti, float nodeArea, out float coverage)
        {
            if (ti.Width <= node.Bounds.Width && ti.Height <= node.Bounds.Height)
            {
                float textureArea = ti.Width * ti.Height;
                coverage = textureArea / nodeArea;

                return true;
            }
            coverage = 0.0f;
            return false;
        }
        private static TextureInfo? FindBestFitForNode(Node _Node, List<TextureInfo> _Textures)
        {
            TextureInfo? bestFit = null;

            float nodeArea = _Node.Bounds.Width * _Node.Bounds.Height;
            float maxCriteria = 0.0f;

            foreach (TextureInfo ti in _Textures)
            {
                switch (FitHeuristic)
                {
                    // Max of Width and Height ratios
                    case BestFitHeuristic.MaxOneAxis:
                        if(MaxOneAxisFit(_Node, ti, out float ratio) && ratio > maxCriteria)
                        {
                            maxCriteria = ratio;
                            bestFit = ti;
                        }
                        break;

                    // Maximize Area coverage
                    case BestFitHeuristic.Area:
                        if (AreaFit(_Node, ti, nodeArea, out float coverage) && coverage > maxCriteria)
                        {
                            maxCriteria = coverage;
                            bestFit = ti;
                        }
                        break;
                }
            }

            return bestFit;
        }
        private static List<TextureInfo> LayoutAtlas(List<TextureInfo> textures, Atlas atlas)
        {
            List<Node> freeList = new();
            atlas.Nodes = new List<Node>();
            List<TextureInfo> textures_d = textures.ToList();

            Node root = new();
            root.Bounds.Size = new Size(atlas.Width, atlas.Height);
            root.SplitType = SplitType.Horizontal;

            freeList.Add(root);

            while (freeList.Count > 0 && textures_d.Count > 0)
            {
                Node node = freeList[0];
                freeList.RemoveAt(0);

                TextureInfo? bestFit = FindBestFitForNode(node, textures_d);
                if (bestFit != null)
                {
                    if (node.SplitType == SplitType.Horizontal)
                    {
                        HorizontalSplit(node, bestFit.Width, bestFit.Height, freeList);
                    }
                    else
                    {
                        VerticalSplit(node, bestFit.Width, bestFit.Height, freeList);
                    }

                    node.Texture = bestFit;
                    node.Bounds.Width = bestFit.Width;
                    node.Bounds.Height = bestFit.Height;

                    textures_d.Remove(bestFit);
                }

                atlas.Nodes.Add(node);
            }

            return textures_d;
        }
        private static Image CreateAtlasImage(Atlas atlas)
        {
            // create a new empty image
            Image image = new Bitmap(atlas.Width, atlas.Height, PixelFormat.Format32bppArgb);

            // create the tool to populate this image
            Graphics graphics = Graphics.FromImage(image);

            foreach (Node n in atlas.Nodes)
            {
                if (n.Texture != null)
                {
                    Image sourceImage = Image.FromStream(new MemoryStream(n.Texture.Data));
                    graphics.DrawImage(sourceImage, n.Bounds);
                }
                else
                {
                    graphics.FillRectangle(Brushes.DarkMagenta, n.Bounds);
                }
            }

            return image;
        }
    }
}
