using System.Windows;
using System.Windows.Controls;

namespace FindAStar.Model
{
    internal class BlockModel
    {
        internal Button Block { get; set; }
        internal Point Position { get; set; }
        internal int Option { get; set; }

        //internal BlockModel()
        //{
        //    Block = new Button();
        //    Position = new Point();
        //}
    }
}
