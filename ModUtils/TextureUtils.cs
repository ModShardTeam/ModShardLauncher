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
    public static partial class Msl
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
            UndertaleSprite sprite = ModLoader.Data.Sprites.First(t => t.Name.Content == name);
            Log.Information("Found sprite: {0}", name);
            return sprite;
        }
        public static UndertaleEmbeddedTexture GetEmbeddedTexture(string name)
        {
            UndertaleEmbeddedTexture embeddedTexture = ModLoader.Data.EmbeddedTextures.First(t => t.Name.Content == name);
            Log.Information("Found embedded texture: {0}", name);
            return embeddedTexture;
        }
        public static UndertaleTexturePageItem GetTexturePageItem(string name)
        {
            UndertaleTexturePageItem texturePageItem = ModLoader.Data.TexturePageItems.First(t => t.Name.Content == name);
            Log.Information("Found texture page item: {0}", name);
            return texturePageItem;
        }
        public static string AddNewTexturePageItem(string embeddedTextureName, RectTexture source, RectTexture target, BoundingData<ushort> bounding)
        {
            UndertaleEmbeddedTexture embeddedTexture = GetEmbeddedTexture(embeddedTextureName);

            UndertaleTexturePageItem texturePageItem = CreateTexureItem(
                embeddedTexture, 
                source, 
                target, 
                bounding
            );
            ModLoader.Data.TexturePageItems.Add(texturePageItem);
            Log.Information("Successfully added a new texture from: {0}", embeddedTextureName);
            return texturePageItem.Name.Content;
        }
        public static string AddNewSprite(string spriteName, List<string> texturePageItemNames, MarginData margin, OriginData origin, BoundingData<uint> bounding)
        {
            UndertaleSprite newSprite = CreateSpriteNoCollisionMasks(
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

            Log.Information("Successfully added new sprite: {0}", newSprite.Name.Content);
            return newSprite.Name.Content;
        }
    }
}