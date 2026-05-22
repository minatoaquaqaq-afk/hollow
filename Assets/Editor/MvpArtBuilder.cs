using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HollowStyleMVP.EditorTools
{
    public static class MvpArtBuilder
    {
        public const string UserRoot = "Assets/Art/UserPack/";
        public const string ImageGenRoot = "Assets/Art/ImageGen/";
        public const string ImageGenSceneReferencePath = ImageGenRoot + "References/metroidvania_testroom_reference.png";
        public const string ImageGenAtlasPath = ImageGenRoot + "Atlases/metroidvania_full_art_atlas.png";
        public const string ImageGenSliced = ImageGenRoot + "Sliced/";
        public const string KenneyDefault = "Assets/Art/External/KenneyScribble/Extracted/PNG/Default/";
        public const string PlayerFrames = UserRoot + "Player/";
        public const string EnemyFrames = UserRoot + "Enemy/";
        public const string BossFrames = UserRoot + "Boss/";
        public const string SceneArt = UserRoot + "Scene/";
        public const string UiArt = UserRoot + "UI/";
        public const string SlashFrames = UserRoot + "Effects/Slash/";
        public const string AudioBgmPath = UserRoot + "Audio/bgm.wav";
        public const string PreferredBgmBasePath = UserRoot + "Audio/Men I Trust - Show Me How";
        public const string FontPath = UserRoot + "Fonts/Pixel.ttf";

        [MenuItem("Hollow Style MVP/Prepare Art Assets")]
        [MenuItem("Tools/Hollow Style MVP/Prepare Art Assets")]
        public static void PrepareArtAssets()
        {
            PrepareFolder(UserRoot);
            PrepareFolder(ImageGenRoot);
            PrepareFolder("Assets/Art/External");
            PrepareFolder("Assets/Art/Generated");
            PreparePreferredBgm();
            PrepareAudio(AudioBgmPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static string U(string relativePath) => UserRoot + relativePath;

        public static Sprite Sprite(string path)
        {
            PrepareTexture(path);
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static AudioClip AudioClip(string path)
        {
            PrepareAudio(path);
            return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        }

        public static Font UiFont()
        {
            return AssetDatabase.LoadAssetAtPath<Font>(FontPath) ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        public static Sprite[] Sprites(params string[] paths)
        {
            var list = new List<Sprite>();
            foreach (string path in paths)
            {
                var sprite = Sprite(path);
                if (sprite != null) list.Add(sprite);
            }
            return list.ToArray();
        }

        public static string IG(int row, int col) => ImageGenSliced + $"r{row:00}_c{col:00}.png";

        public static string K(string name)
        {
            return name switch
            {
                "character_roundGreen.png" => IG(0, 2),
                "character_roundYellow.png" => IG(9, 9),
                "tile_door.png" => IG(9, 7),
                "tile_chest.png" => IG(9, 0),
                "tile_blockDoor.png" => IG(9, 8),
                "tile_cog.png" => IG(9, 17),
                "tile_flag.png" => IG(9, 2),
                "tile_coin.png" => IG(11, 11),
                "tile_spikes.png" => IG(8, 10),
                "tile_bridge.png" => IG(8, 15),
                "tile_grass.png" => IG(8, 0),
                "tile_stone.png" => IG(8, 1),
                "ui_box.png" => IG(11, 0),
                "ui_button.png" => IG(11, 19),
                _ => KenneyDefault + name
            };
        }

        public static string P(int row, int col)
        {
            return row switch
            {
                0 => IG(0, Clamp(col, 0, 5)),
                1 => IG(1, Clamp(col, 0, 7)),
                2 => IG(2, Clamp(col + 3, 3, 6)),
                3 => IG(1, Clamp(col + 8, 8, 12)),
                4 => IG(2, Clamp(col + 7, 7, 9)),
                5 => IG(3, Clamp(col + 2, 2, 5)),
                6 => IG(3, Clamp(col + 4, 4, 10)),
                _ => IG(0, 0)
            };
        }

        public static string E(int row, int col)
        {
            return row switch
            {
                0 => IG(4, Clamp(col, 0, 6)),
                1 => IG(4, Clamp(col, 0, 6)),
                2 => IG(5, Clamp(col, 0, 7)),
                3 => IG(5, Clamp(col + 8, 8, 12)),
                _ => IG(4, 0)
            };
        }

        public static string B(int row, int col)
        {
            return row switch
            {
                0 => IG(6, Clamp(col, 0, 5)),
                1 => IG(6, Clamp(col + 6, 6, 11)),
                2 => IG(6, Clamp(col + 12, 12, 20)),
                3 => IG(7, Clamp(col, 0, 12)),
                _ => IG(6, 0)
            };
        }

        public static string S(int row, int col)
        {
            int index = row * 4 + col;
            return IG(10, Clamp(index, 0, 15));
        }
        private static int Clamp(int value, int min, int max) => Mathf.Clamp(value, min, max);

        private static void PrepareFolder(string folder)
        {
            if (!Directory.Exists(folder)) return;
            foreach (string file in Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories))
            {
                string assetPath = file.Replace('\\', '/');
                PrepareTexture(assetPath);
            }
        }

        private static void PrepareTexture(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath) || !File.Exists(ToDiskPath(assetPath))) return;
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            }
            if (importer == null) return;
            bool dirty = false;
            if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; dirty = true; }
            if (importer.spriteImportMode != SpriteImportMode.Single) { importer.spriteImportMode = SpriteImportMode.Single; dirty = true; }
            if (importer.spritePixelsPerUnit != 64f) { importer.spritePixelsPerUnit = 64f; dirty = true; }
            if (importer.filterMode != FilterMode.Point) { importer.filterMode = FilterMode.Point; dirty = true; }
            if (importer.textureCompression != TextureImporterCompression.Uncompressed) { importer.textureCompression = TextureImporterCompression.Uncompressed; dirty = true; }
            if (importer.mipmapEnabled) { importer.mipmapEnabled = false; dirty = true; }
            if (importer.maxTextureSize < 4096) { importer.maxTextureSize = 4096; dirty = true; }

            dirty |= ApplyPlatformTextureSettings(importer, "DefaultTexturePlatform");
            dirty |= ApplyPlatformTextureSettings(importer, "Standalone");
            if (dirty) importer.SaveAndReimport();
        }

        private static bool ApplyPlatformTextureSettings(TextureImporter importer, string buildTarget)
        {
            var settings = importer.GetPlatformTextureSettings(buildTarget);
            bool dirty = false;
            if (!settings.overridden) { settings.overridden = true; dirty = true; }
            if (settings.maxTextureSize < 4096) { settings.maxTextureSize = 4096; dirty = true; }
            if (settings.textureCompression != TextureImporterCompression.Uncompressed)
            {
                settings.textureCompression = TextureImporterCompression.Uncompressed;
                dirty = true;
            }
            if (settings.format != TextureImporterFormat.RGBA32)
            {
                settings.format = TextureImporterFormat.RGBA32;
                dirty = true;
            }
            if (dirty) importer.SetPlatformTextureSettings(settings);
            return dirty;
        }

        private static void PrepareAudio(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath) || !File.Exists(ToDiskPath(assetPath))) return;
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }

        public static string PreferredBgmPath()
        {
            foreach (string basePath in new[] { PreferredBgmBasePath, UserRoot + "Audio/Show Me How" })
            {
                foreach (string ext in new[] { ".mp3", ".wav", ".ogg", ".aiff", ".aif" })
                {
                    string path = basePath + ext;
                    if (File.Exists(ToDiskPath(path))) return path;
                }
            }
            return AudioBgmPath;
        }

        private static void PreparePreferredBgm()
        {
            string path = PreferredBgmPath();
            if (path != AudioBgmPath) PrepareAudio(path);
        }

        private static string ToDiskPath(string assetPath)
        {
            string normalized = assetPath.Replace('\\', '/');
            if (!normalized.StartsWith("Assets/")) return assetPath;
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? Directory.GetCurrentDirectory();
            return Path.Combine(projectRoot, normalized);
        }
    }
}




