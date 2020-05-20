using System;

namespace MyMood
{
    [Serializable]
    public class MyAudio
    {
        private string name;
        private string style;
        private string group;
        private string path;

        public string Name
        {
            get { return name; }
            set
            {
                this.name = value;
            }
        }

        public string Style
        {
            get { return style; }
            set
            {
                this.style = value;
            }
        }
        public string Group
        {
            get { return group; }
            set
            {
                this.group = value;
            }
        }
        public string Path
        {
            get { return path; }
            set
            {
                this.path = value;
            }
        }

        public MyAudio(string name = null, string style = null, string group = null, string path = null)
        {
            this.name = name;
            this.style = style;
            this.group = group;
            this.path = path;
        }

        public MyAudio(MyAudio r)
        {
            this.name = r.name;
            this.style = r.style;
            this.group = r.group;
            this.path = r.path;
        }
    }
}