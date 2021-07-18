using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_resizing_magic
{
    public class ImageClassAux
    {
        private string _DirectoryRoute;
        private string _NameImage;
        private string _Extension;

        public ImageClassAux(string directoryRoute, string nameImage, string extension)
        {
            _DirectoryRoute = directoryRoute;
            _NameImage = nameImage;
            _Extension = extension;
        }

        public string GetDirectoryRoute() => _DirectoryRoute;
        public string GetNameImage() => _NameImage;
        public string GetExtension() => _Extension;
    }
}
