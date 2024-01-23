using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    public class RectTexture
    {
        public ushort X;
        public ushort Y;
        public ushort Width;
        public ushort Height;

        public RectTexture(ushort x, ushort y, ushort width, ushort height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
    public class MarginData
    {
        public int Top;
        public int Bottom;
        public int Left;
        public int Right;

        public MarginData(int top, int bottom, int left, int right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }
    }
    public class BoundingData<T>
    {
        public T Width;
        public T Height;

        public BoundingData(T width, T height)
        {
            Width = width;
            Height = height;
        }
    }
    public class OriginData
    {
        public int X;
        public int Y;

        public OriginData(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public static class TextureUtils
    {
        public static UndertaleTexturePageItem CreateTexureItem(UndertaleEmbeddedTexture texture, RectTexture source, RectTexture target, BoundingData<ushort> bounding) 
        {
            return new()
            {
                Name = ModLoader.Data.Strings.MakeString("PageItem " + ModLoader.Data.TexturePageItems.Count),

                SourceX = source.X,
                SourceY = source.Y,
                SourceHeight = source.Height,
                SourceWidth = source.Width,

                TargetX = target.X,
                TargetY = target.Y,
                TargetHeight = target.Height,
                TargetWidth = target.Width,

                BoundingHeight = bounding.Height,
                BoundingWidth = bounding.Width,
                TexturePage = texture
            };
        }
        public static UndertaleSprite CreateSpriteNoCollisionMasks(string spriteName, MarginData margin, OriginData origin, BoundingData<uint> bounding) 
        {
            UndertaleSprite newSprite = new()
            {
                Name = ModLoader.Data.Strings.MakeString(spriteName),
                Width = bounding.Width,
                Height = bounding.Height,
                MarginLeft = margin.Left,
                MarginRight = margin.Right,
                MarginTop = margin.Top,
                MarginBottom = margin.Bottom,
                OriginX = origin.X,
                OriginY = origin.Y,
            };
            
            return newSprite;
        }
        public static UndertaleSprite GetSprite(string name)
        {
            try
            {
                UndertaleSprite sprite = ModLoader.Data.Sprites.First(t => t.Name.Content == name);
                Log.Information(string.Format("Found sprite: {0}", name.ToString()));
                return sprite;
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static UndertaleEmbeddedTexture GetEmbeddedTexture(string name)
        {
            try
            {
                UndertaleEmbeddedTexture embeddedTexture = ModLoader.Data.EmbeddedTextures.First(t => t.Name.Content == name);
                Log.Information(string.Format("Found embedded texture: {0}", name.ToString()));
                return embeddedTexture;
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static UndertaleTexturePageItem GetTexturePageItem(string name)
        {
            try
            {
                UndertaleTexturePageItem texturePageItem = ModLoader.Data.TexturePageItems.First(t => t.Name.Content == name);
                Log.Information(string.Format("Found texture page item: {0}", name.ToString()));
                return texturePageItem;
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static string AddNewTexturePageItem(string embeddedTextureName, RectTexture source, RectTexture target, BoundingData<ushort> bounding)
        {
            try
            {
                UndertaleEmbeddedTexture embeddedTexture = GetEmbeddedTexture(embeddedTextureName);

                UndertaleTexturePageItem texturePageItem = TextureUtils.CreateTexureItem(
                    embeddedTexture, 
                    source, 
                    target, 
                    bounding
                );
                ModLoader.Data.TexturePageItems.Add(texturePageItem);
                Log.Information(string.Format("Successfully added a new texture from: {0}", embeddedTextureName.ToString()));
                return texturePageItem.Name.Content;

            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static string AddNewSprite(string spriteName, List<string> texturePageItemNames, MarginData margin, OriginData origin, BoundingData<uint> bounding)
        {
            try
            {
                UndertaleSprite newSprite = TextureUtils.CreateSpriteNoCollisionMasks(
                    spriteName,
                    margin,
                    origin,
                    bounding
                );

                IEnumerable<UndertaleSprite.TextureEntry> texturePageItems = texturePageItemNames
                    .Select(x => GetTexturePageItem(x))
                    .Select(x => new UndertaleSprite.TextureEntry(){ Texture = x });

                foreach(UndertaleSprite.TextureEntry texturePageItem in texturePageItems)
                {
                    newSprite.Textures.Add(texturePageItem);
                }
                
                ModLoader.Data.Sprites.Add(newSprite);

                Log.Information(string.Format("Successfully added new sprite: {0}", newSprite.Name.Content));
                return newSprite.Name.Content;

            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
    }
}