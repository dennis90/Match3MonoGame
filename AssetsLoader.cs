using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Match3Mono
{
    internal class AssetsLoader : ContentManager
    {
        private Dictionary<string, Texture2D> texture2dMap;
        private Dictionary<string, Texture2D[]> texture2dList;
        private Dictionary<string, SpriteFont> spriteFontMap;
        private Dictionary<string, SoundEffect> soundEffectMap;
        private Dictionary<string, Song> songMap;
        private Dictionary<string, SoundEffect[]> soundEffectList;

        private static AssetsLoader instance;

        private AssetsLoader(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
            texture2dMap = new();
            texture2dList = new();
            spriteFontMap = new();
            soundEffectMap = new();
            soundEffectList = new();
            songMap = new();
        }

        public static AssetsLoader GetInstance(
            IServiceProvider serviceProvider,
            string rootDirectory
        )
        {
            if (instance == null)
            {
                instance = new AssetsLoader(serviceProvider, rootDirectory);
            }
            return instance;
        }

        public static AssetsLoader GetInstance()
        {
            if (instance == null)
            {
                throw new Exception("AssetsLoader must be initialized first");
            }

            return instance;
        }

        public void LoadTexture(string path)
        {
            var texture = Load<Texture2D>(path);
            if (texture != null)
            {
                texture2dMap.Add(path, texture);
            }
        }

        public void LoadFont(string path)
        {
            var font = Load<SpriteFont>(path);
            if (font != null)
            {
                spriteFontMap.Add(path, font);
            }
        }

        public void LoadSoundEffect(string path)
        {
            var sound = Load<SoundEffect>(path);
            if (sound != null)
            {
                soundEffectMap.Add(path, sound);
            }
        }

        public void LoadSong(string path)
        {
            var sound = Load<Song>(path);
            if (sound != null)
            {
                songMap.Add(path, sound);
            }
        }

        public void LoadSoundEffectList(string listKey, string[] paths)
        {
            List<SoundEffect> sounds = new();

            foreach (var path in paths)
            {
                var sound = Load<SoundEffect>(path);
                if (sound != null)
                {
                    sounds.Add(sound);
                }
            }

            if (sounds.Count > 0)
            {
                soundEffectList.Add(listKey, sounds.ToArray());
            }
        }

        public void LoadTextureList(string listKey, string[] paths)
        {
            List<Texture2D> textures = new();

            foreach (var path in paths)
            {
                var texture = Load<Texture2D>(path);
                if (texture != null)
                {
                    textures.Add(texture);
                }
            }

            if (textures.Count > 0)
            {
                texture2dList.Add(listKey, textures.ToArray());
            }
        }

        public Texture2D GetTexture(string path)
        {
            if (texture2dMap.ContainsKey(path))
            {
                return texture2dMap[path];
            }
            return null;
        }

        public Texture2D[] GetTextureList(string path)
        {
            if (texture2dList.ContainsKey(path))
            {
                return texture2dList[path];
            }
            return null;
        }

        public SpriteFont GetFont(string path)
        {
            if (spriteFontMap.ContainsKey(path))
            {
                return spriteFontMap[path];
            }
            return null;
        }

        public SoundEffect GetSoundEffect(string path)
        {
            if (soundEffectMap.ContainsKey(path))
            {
                return soundEffectMap[path];
            }
            return null;
        }

        public Song GetSong(string path)
        {
            if (songMap.ContainsKey(path))
            {
                return songMap[path];
            }
            return null;
        }

        public SoundEffect[] GetSoundEffectList(string path)
        {
            if (soundEffectList.ContainsKey(path))
            {
                return soundEffectList[path];
            }

            return null;
        }
    }
}
