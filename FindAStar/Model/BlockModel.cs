
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace FindAStar.Model
{
    using FindAStar.Enumerators;
    using FindAStar.Extension;

    public class BlockModel 
    {
        public Button Block { get; private set; }
        public Point Position { get; private set; }

        private SolidColorBrush _colorValue;
        private NodeType _nType;
        public NodeType NType
        {
            get
            {
                return _nType;
            }
            set
            {
                _nType = value;
                Block.Background = new SolidColorBrush(ColorExtension.ChangeColor(value));
            }
        }

        public BlockModel(Button block, Point position, NodeType type)
        {
            _colorValue = new SolidColorBrush();
            this.Block = block;
            this.Position = position;
            this.NType = type;
        }
    }
}
