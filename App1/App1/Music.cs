using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioApp
{
    class Music
    {
        public Music(string Title_, string Artist_)
        {
            Title = Title_;
            Artist = Artist_;

            Debug.WriteLine(Title + " - " + Artist + "\n");
        }

        public string Title { get; set; }
        public string Artist { get; set; }
    }
}
