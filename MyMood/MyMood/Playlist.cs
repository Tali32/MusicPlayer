using System;
using System.Collections.Generic;

namespace MyMood
{
    [Serializable]
    public class Playlist
    {
        private string name;
        private List<MyAudio> playlist;

        public string Name
        {
            get { return name; }
            set
            {
                this.name = value;
            }
        }
        public List<MyAudio> Playlist_
        {
            get { return playlist; }
            set
            {
                this.playlist = value;
            }
        }

        public Playlist()
        {
            this.name = null;
            playlist = new List<MyAudio>();
        }

        public Playlist(string name, List<MyAudio> playlist)
        {
            Name = name;
            Playlist_ = playlist;
        }

        public Playlist(Playlist r)
        {
            this.name = r.name;
            this.playlist = r.playlist;
        }

    }
}