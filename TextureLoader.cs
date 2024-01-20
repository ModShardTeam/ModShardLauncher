using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using UndertaleModLib.Models;
using UndertaleModLib;
using System.Text.RegularExpressions;
using System.Collections;
using System.Drawing.Imaging;

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
        public static StringWriter Log;
        public static StringWriter Error;
        public static int Padding;
        public static int AtlasSize;
        public static BestFitHeuristic FitHeuristic;
        public static List<Atlas> Atlasses;
        public static Regex sprFrameRegex = new(@"^(.+?)(?:_(-*\d+))*$", RegexOptions.Compiled);
        public static UndertaleData Data => DataLoader.data;
        public TextureLoader()
        {
            SourceTextures = new List<TextureInfo>();
            Log = new StringWriter();
            Error = new StringWriter();
        }
        public static void LoadTextures(ModFile mod)
        {
            Process(mod, 2048, 2, false);
            foreach (Atlas atlas in Atlasses)
            {
                UndertaleEmbeddedTexture ueTexture = new()
                {
                    Name = Data.Strings.MakeString(mod.Name)
                };
                var ms = new MemoryStream();
                var img = CreateAtlasImage(atlas);
                img.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                ueTexture.TextureData.TextureBlob = new byte[ms.Length];
                ms.Read(ueTexture.TextureData.TextureBlob);
                Data.EmbeddedTextures.Add(ueTexture);
                Bitmap atlasBitmap = new(img);
                foreach (Node node in atlas.Nodes)
                {
                    if(node.Texture != null)
                    {
                        var texturePageItem = new UndertaleTexturePageItem()
                        {
                            Name = Data.Strings.MakeString("PageItem " + Data.TexturePageItems.Count),
                            SourceX = (ushort)node.Bounds.X,
                            SourceY = (ushort)node.Bounds.Y,
                            SourceHeight = (ushort)node.Bounds.Height,
                            SourceWidth = (ushort)node.Bounds.Width,
                            TargetX = 0,
                            TargetY = 0,
                            TargetHeight = (ushort)node.Bounds.Height,
                            TargetWidth = (ushort)node.Bounds.Width,
                            BoundingHeight = (ushort)node.Bounds.Height,
                            BoundingWidth = (ushort)node.Bounds.Width,
                            TexturePage = ueTexture
                        };

                        Data.TexturePageItems.Add(texturePageItem);

                        string stripped = Path.GetFileNameWithoutExtension(node.Texture.Source);

                        string spriteName;
                        int frame;
                        
                        try
                        {
                            var spriteParts = sprFrameRegex.Match(stripped);
                            spriteName = spriteParts.Groups[1].Value;
                            int.TryParse(spriteParts.Groups[2].Value, out frame);
                        }
                        catch 
                        {
                            continue;
                        }

                        UndertaleSprite sprite = Data.Sprites.ByName(spriteName);
                        UndertaleSprite.TextureEntry textureEntry = new()
                        {
                            Texture = texturePageItem
                        };

                        if (sprite == null)
                        {
                            UndertaleString spriteUTString = Data.Strings.MakeString(spriteName);
                            UndertaleSprite newSprite = new()
                            {
                                Name = spriteUTString,
                                Width = (uint)node.Bounds.Width,
                                Height = (uint)node.Bounds.Height,
                                MarginLeft = 0,
                                MarginRight = node.Bounds.Width - 1,
                                MarginTop = 0,
                                MarginBottom = node.Bounds.Height - 1,
                                OriginX = 0,
                                OriginY = 0,
                            };

                            if(frame > 0)
                            {
                                for (int i = 0; i < frame; i++)
                                    newSprite.Textures.Add(null);
                            }

                            newSprite.CollisionMasks.Add(newSprite.NewMaskEntry());
                            Rectangle bmpRect = new(node.Bounds.X, node.Bounds.Y, node.Bounds.Width, node.Bounds.Height);
                            var format = atlasBitmap.PixelFormat;
                            var clone = atlasBitmap.Clone(bmpRect, format);
                            int width = (node.Bounds.Width + 7) / 8 * 8;
                            BitArray maskingBitArray = new BitArray(width * node.Bounds.Height);
                            for(int y = 0; y < node.Bounds.Height; y++)
                            {
                                for (int x = 0; x < node.Bounds.Width; x++)
                                {
                                    Color pixelColor = clone.GetPixel(x, y);
                                    maskingBitArray[y * width + x] = (pixelColor.A > 0);
                                }
                            }
                            BitArray ba = new BitArray(width * node.Bounds.Height);
                            for(int i = 0; i < maskingBitArray.Length; i += 8)
                            {
                                for(int j = 0; j < 8; j++)
                                {
                                    ba[j + i] = maskingBitArray[-(j - 7) + i];
                                }
                            }
                            int numBytes = maskingBitArray.Length / 8;
                            byte[] bytes = new byte[numBytes];
                            ba.CopyTo(bytes, 0);
                            for(int i = 0; i < bytes.Length; i++)
                                newSprite.CollisionMasks[0].Data[i] = bytes[i];
                            newSprite.Textures.Add(textureEntry);
                            Data.Sprites.Add(newSprite);
                            continue;
                        }
                        if (frame > sprite.Textures.Count - 1)
                        {
                            while (frame > sprite.Textures.Count - 1)
                            {
                                sprite.Textures.Add(textureEntry);
                            }
                            continue;
                        }
                        sprite.Textures[frame] = textureEntry;
                    }
                }
            }
            Atlasses.Clear();
        }
        public static void Process(ModFile mod, int _AtlasSize, int _Padding, bool _DebugMode)
        {
            Padding = _Padding;
            AtlasSize = _AtlasSize;

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
        public static void SaveAtlasses(string _Destination)
        {
            int atlasCount = 0;
            string prefix = _Destination.Replace(Path.GetExtension(_Destination), "");
            StreamWriter tw = new(_Destination);

            tw.WriteLine("source_tex, atlas_tex, u, v, scale_u, scale_v");

            foreach (Atlas atlas in Atlasses)
            {
                string atlasName = String.Format(prefix + "{0:000}" + ".png", atlasCount);

                //1: Save images
                Image img = CreateAtlasImage(atlas);
                img.Save(atlasName, System.Drawing.Imaging.ImageFormat.Png);

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
            tw.WriteLine(Log.ToString());
            tw.WriteLine("--- ERROR -----------------------------------------");
            tw.WriteLine(Error.ToString());
            tw.Close();
        }
        private static void ScanForTextures(ModFile mod)
        {
            foreach (FileChunk fi in mod.Files)
            {
                if (!fi.name.EndsWith("png")) continue;
                if (fi.name == mod.Name + "\\" + "icon.png") continue;
                var ms = new MemoryStream(mod.GetFile(fi.name));
                Image img = Image.FromStream(ms);
                if (img != null && img.Width <= AtlasSize && img.Height <= AtlasSize)
                {
                    TextureInfo ti = new()
                    {
                        Width = img.Width,
                        Height = img.Height,
                        Source = fi.name.Split("\\")[^1],
                        Data = mod.GetFile(fi.name)
                    };

                    SourceTextures.Add(ti);
                }
            }
        }
        private static void HorizontalSplit(Node _ToSplit, int _Width, int _Height, List<Node> _List)
        {
            Node n1 = new();
            n1.Bounds.X = _ToSplit.Bounds.X + _Width + Padding;
            n1.Bounds.Y = _ToSplit.Bounds.Y;
            n1.Bounds.Width = _ToSplit.Bounds.Width - _Width - Padding;
            n1.Bounds.Height = _Height;
            n1.SplitType = SplitType.Vertical;

            Node n2 = new();
            n2.Bounds.X = _ToSplit.Bounds.X;
            n2.Bounds.Y = _ToSplit.Bounds.Y + _Height + Padding;
            n2.Bounds.Width = _ToSplit.Bounds.Width;
            n2.Bounds.Height = _ToSplit.Bounds.Height - _Height - Padding;
            n2.SplitType = SplitType.Horizontal;

            if (n1.Bounds.Width > 0 && n1.Bounds.Height > 0)
                _List.Add(n1);
            if (n2.Bounds.Width > 0 && n2.Bounds.Height > 0)
                _List.Add(n2);
        }
        private static void VerticalSplit(Node _ToSplit, int _Width, int _Height, List<Node> _List)
        {
            Node n1 = new();
            n1.Bounds.X = _ToSplit.Bounds.X + _Width + Padding;
            n1.Bounds.Y = _ToSplit.Bounds.Y;
            n1.Bounds.Width = _ToSplit.Bounds.Width - _Width - Padding;
            n1.Bounds.Height = _ToSplit.Bounds.Height;
            n1.SplitType = SplitType.Vertical;

            Node n2 = new();
            n2.Bounds.X = _ToSplit.Bounds.X;
            n2.Bounds.Y = _ToSplit.Bounds.Y + _Height + Padding;
            n2.Bounds.Width = _Width;
            n2.Bounds.Height = _ToSplit.Bounds.Height - _Height - Padding;
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
        private static List<TextureInfo> LayoutAtlas(List<TextureInfo> _Textures, Atlas _Atlas)
        {
            List<Node> freeList = new();
            _Atlas.Nodes = new List<Node>();
            List<TextureInfo> textures = _Textures.ToList();

            Node root = new();
            root.Bounds.Size = new Size(_Atlas.Width, _Atlas.Height);
            root.SplitType = SplitType.Horizontal;

            freeList.Add(root);

            while (freeList.Count > 0 && textures.Count > 0)
            {
                Node node = freeList[0];
                freeList.RemoveAt(0);

                TextureInfo? bestFit = FindBestFitForNode(node, textures);
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

                    textures.Remove(bestFit);
                }

                _Atlas.Nodes.Add(node);
            }

            return textures;
        }
        private static Image CreateAtlasImage(Atlas _Atlas)
        {
            Image img = new Bitmap(_Atlas.Width, _Atlas.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(img);

            foreach (Node n in _Atlas.Nodes)
            {
                if (n.Texture != null)
                {
                    Image sourceImg = Image.FromStream(new MemoryStream(n.Texture.Data));
                    g.DrawImage(sourceImg, n.Bounds);
                }
                else
                {
                    g.FillRectangle(Brushes.DarkMagenta, n.Bounds);
                }
            }

            return img;
        }
    }
}
