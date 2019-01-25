
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace FindAStar.Extension
{
    using FindAStar.Enumerators;

    public static class ColorExtension
    {
        public static Color ChangeColor(NodeType type)
        {
            Color content = new Color();
            switch (type)
            {
                case NodeType.Passable:
                    content = Color.FromArgb(255, 20, 20, 20);
                    break;

                case NodeType.Wall:
                    content = Color.FromArgb(255, 50, 120, 210);
                    break;

                case NodeType.Current:
                    content = Color.FromArgb(255, 70, 212, 50);
                    break;

                case NodeType.Target:
                    content = Color.FromArgb(255, 212, 50, 50);
                    break;

                case NodeType.Path:
                    content = Color.FromArgb(255, 228, 228, 0);
                    break;

                case NodeType.SearchPath:
                    content = Color.FromArgb(255, 255, 140, 0);
                    break;

                default:
                    break;
            }

            return content;
        }

    }
}
