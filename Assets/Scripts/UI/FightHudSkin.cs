using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace HollowStyleMVP.UI
{
    public static class FightHudSkin
    {
        private const string HudAssetPath = "Assets/ui_fight_hud";
        private static readonly Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>();
        private static readonly Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();

        public static Sprite LoadSprite(string fileName)
        {
            if (SpriteCache.TryGetValue(fileName, out var cached)) return cached;

            string resourceName = ResourceName(fileName);
            var sprite = Resources.Load<Sprite>($"ui_fight_hud/{resourceName}");
#if UNITY_EDITOR
            if (sprite == null)
            {
                string path = $"{HudAssetPath}/{fileName}";
                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite == null)
                {
                    foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path))
                    {
                        if (asset is Sprite candidate)
                        {
                            sprite = candidate;
                            break;
                        }
                    }
                }
            }
#endif
            if (sprite == null)
            {
                var texture = LoadTexture(fileName);
                if (texture != null)
                    sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            }

            SpriteCache[fileName] = sprite;
            return sprite;
        }

        public static Texture2D LoadTexture(string fileName)
        {
            if (TextureCache.TryGetValue(fileName, out var cached)) return cached;

            string resourceName = ResourceName(fileName);
            var texture = Resources.Load<Texture2D>($"ui_fight_hud/{resourceName}");
#if UNITY_EDITOR
            if (texture == null)
                texture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{HudAssetPath}/{fileName}");
#endif
            TextureCache[fileName] = texture;
            return texture;
        }

        private static string ResourceName(string fileName)
        {
            return fileName.EndsWith(".png") ? fileName.Substring(0, fileName.Length - 4) : fileName;
        }
    }
}
