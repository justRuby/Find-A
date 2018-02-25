using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using FindAStar.Model;

namespace FindAStar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int COUNT_BLOCKS_HEIGHT = 24;
        private const int COUNT_BLOCKS_WIDTH = 24; //56

        //private Button[,] blocks;
        //private Point[,] positions;
        //private int[,] options;

        private BlockModel[,] blocks;
        private BlockModel[] earlyBlock;
        private int countStartBlock = 0, countEndBlock = 0;
        private int selectedBlock;

        /// <summary>
        /// Gray - Свободный путь | 0
        /// Orange - Старый Маршрут | 1
        /// Blue - Преграда | 2
        /// Green - Начало | 3
        /// Red - Конец | 4
        /// Yellow - Маршрут | 5
        /// </summary>

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double left = 0;
            double top = 0;
            double indent = 0d;
            //int count = COUNT_BLOCKS_HEIGHT * COUNT_BLOCKS_WIDTH;

            //blocks = new List<BlockModel>(count)
            //{
            //    new BlockModel()
            //    {
            //        Block = new Button()
            //    }
            //};

            //positions = new Point[COUNT_BLOCKS_HEIGHT, COUNT_BLOCKS_WIDTH];
            //options = new int[COUNT_BLOCKS_HEIGHT, COUNT_BLOCKS_WIDTH];

            blocks = new BlockModel[COUNT_BLOCKS_HEIGHT, COUNT_BLOCKS_WIDTH];
            earlyBlock = new BlockModel[2];

            for (int i = 0; i < COUNT_BLOCKS_HEIGHT; i++)
            {
                for (int j = 0; j < COUNT_BLOCKS_WIDTH; j++)
                {
                    blocks[i, j] = new BlockModel()
                    {
                        Block = new Button()
                        {
                            Name = "block_" + i + "_" + j,
                            //Content = 0,
                            //Foreground = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)),
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Width = 10,
                            Height = 10,
                            Margin = new Thickness(left + indent, top + indent, 0, 0),
                            Background = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20)),
                            BorderBrush = null
                        }
                    };

                    blocks[i, j].Block.Click += Block_Click;

                    blocks[i, j].Position = new Point(left + indent, top + indent);
                    viewArrayGrid.Children.Add(blocks[i, j].Block);

                    left += 10;
                }

                left = 0;
                top += 10;
            }

            //for (int i = 1; i < count + 1; i++)
            //{
            //    blocks.Add(new BlockModel());
            //    blocks[i].Block = new Button()
            //    {
            //        Name = "block_" + i,
            //        VerticalAlignment = VerticalAlignment.Top,
            //        HorizontalAlignment = HorizontalAlignment.Left,
            //        Width = 20,
            //        Height = 20,
            //        Margin = new Thickness(left + indent, top + indent, 0, 0),
            //        Background = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20)),
            //        BorderBrush = null
            //    };

            //    blocks[i].Block.Click += Block_Click;

            //    blocks[i].Position = new Point(left + indent, top + indent);
            //    viewArrayGrid.Children.Add(blocks[i].Block);

            //    left += 20;

            //    if(i % COUNT_BLOCKS_WIDTH == 0)
            //    {
            //        left = 0;
            //        top += 20;
            //    }
            //}
        }

        private void Block_Click(object sender, RoutedEventArgs e)
        {
            Button obj = sender as Button;

            for (int i = 0; i < COUNT_BLOCKS_HEIGHT; i++)
            {
                for (int j = 0; j < COUNT_BLOCKS_WIDTH; j++)
                {
                    if (obj.Name != blocks[i,j].Block.Name)
                        continue;

                    switch (selectedBlock)
                    {
                        case 1:

                            //Blue
                            blocks[i, j].Option = 2;
                            break;

                        case 2:

                            //Green
                            if(countStartBlock == 1 && blocks[i,j] != earlyBlock[0])
                            {
                                GetIndex(earlyBlock[0].Block.Name, out int i2, out int j2);

                                blocks[i2, j2].Option = 0;
                                blocks[i2, j2].Block.Background = new SolidColorBrush(SetColor(0));

                                blocks[i, j].Option = 3;
                                earlyBlock[0] = blocks[i, j];
                            }
                            else
                            {
                                countStartBlock = 1;
                                blocks[i, j].Option = 3;
                                earlyBlock[0] = blocks[i, j];
                            }
                            break;

                        case 3:

                            //Red
                            if (countEndBlock == 1)
                            {
                                GetIndex(earlyBlock[1].Block.Name, out int i2, out int j2);

                                blocks[i2, j2].Option = 0;
                                blocks[i2, j2].Block.Background = new SolidColorBrush(SetColor(0));

                                blocks[i, j].Option = 4;
                                earlyBlock[1] = blocks[i, j];
                            }
                            else
                            {
                                countEndBlock = 1;
                                blocks[i, j].Option = 4;
                                earlyBlock[1] = blocks[i, j];
                            }
                            break;

                        default:

                            //Gray
                            if (blocks[i,j].Option == 3)
                                countStartBlock = 0;

                            if (blocks[i, j].Option == 4)
                                countEndBlock = 0;

                            blocks[i, j].Option = 0;
                            break;
                    }

                    blocks[i, j].Block.Background = new SolidColorBrush(SetColor(blocks[i, j].Option));
                    return;
                }
            }

            //foreach (var item in search)
            //{
            //    if(item.Option < 3)
            //    {
            //        item.Option++;
            //    }
            //    else
            //    {
            //        item.Option = 0;
            //    }

            //    blocks[0] = item;
            //    item.Block.Background = new SolidColorBrush(SetColor(item.Option));
            //    break;
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var obj = sender as Button;

            switch (obj.Name)
            {
                case "borderSelectButton":
                    statusLabel.Content = "Выбран синий блок";
                    selectedBlock = 1;
                    break;

                case "startSelectButton":
                    selectedBlock = 2;
                    statusLabel.Content = "Выбран зеленый блок";
                    break;

                case "endSelectButton":
                    statusLabel.Content = "Выбран красный блок";
                    selectedBlock = 3;
                    break;

                default:
                    statusLabel.Content = "Выбран режим удаления";
                    selectedBlock = 0;
                    break;
            }
        }

        private async void FindPathButton_Click(object sender, RoutedEventArgs e)
        {
            Point startPosition = new Point(-1, -1);
            Point endPosition = new Point(-1, -1);
            var field = new int[COUNT_BLOCKS_HEIGHT, COUNT_BLOCKS_WIDTH];

            for (int i = 0; i < COUNT_BLOCKS_HEIGHT; i++)
            {
                for (int j = 0; j < COUNT_BLOCKS_WIDTH; j++)
                {
                    field[i, j] = blocks[i, j].Option;

                    if (blocks[i, j].Option == 3)
                    {
                        startPosition = blocks[i, j].Position;
                        field[i, j] = 0;
                    }
                        
                    if (blocks[i, j].Option == 4)
                    {
                        endPosition = blocks[i, j].Position;
                        field[i, j] = 0;
                    }
                }
            }

            startPosition = new Point(startPosition.X / 10, startPosition.Y / 10);
            endPosition = new Point(endPosition.X / 10, endPosition.Y / 10);

            if (startPosition.X != -1 && endPosition.X != -1)
            {
                var path = await Task.Run( async () => await FindPath(field, startPosition, endPosition));

                if(path != null)
                ColorizePath(path);
            }
        }

        private void СlearFieldButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < COUNT_BLOCKS_HEIGHT; i++)
            {
                for (int j = 0; j < COUNT_BLOCKS_WIDTH; j++)
                {
                    blocks[i, j].Block.Background = new SolidColorBrush(SetColor(0));
                    blocks[i, j].Option = 0;
                }
            }
        }

        private void ClearPathButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < COUNT_BLOCKS_HEIGHT; i++)
            {
                for (int j = 0; j < COUNT_BLOCKS_WIDTH; j++)
                {
                    if(blocks[i,j].Option == 5)
                    {
                        blocks[i, j].Block.Background = new SolidColorBrush(SetColor(0));
                        blocks[i, j].Option = 0;
                    }
                }
            }
        }

        private int GetIndex(string name, out int i, out int j)
        {
            i = 0;
            j = 0;

            var temp = name.Remove(0, 6);
            var array = temp.Split(new char[] { '_' });

            int.TryParse(array[0], out i);
            int.TryParse(array[1], out j);

            return 0;
        }

        private void ColorizePath(List<Point> path)
        {
            List<Point> temp = new List<Point>();

            for (int i = 1; i < path.Count - 1; i++)
                temp.Add(new Point(path[i].X * 10, path[i].Y * 10));
            
            for (int i = 0; i < COUNT_BLOCKS_HEIGHT; i++)
            {
                for (int j = 0; j < COUNT_BLOCKS_WIDTH; j++)
                {
                    for (int p = 0; p < temp.Count; p++)
                    {
                        if (temp[p] == blocks[i, j].Position)
                        {
                            blocks[i, j].Option = 5;
                            blocks[i, j].Block.Background = new SolidColorBrush(SetColor(5));
                        }
                    }
                }
            }
        }

        private Color SetColor(int item)
        {
            Color content = new Color();
            switch (item)
            {
                case 2:
                    content = Color.FromArgb(255, 50, 120, 210);
                    break;

                case 3:
                    content = Color.FromArgb(255, 70, 212, 50);
                    break;

                case 4:
                    content = Color.FromArgb(255, 212, 50, 50);
                    break;

                case 5:
                    content = Color.FromArgb(255, 228, 228, 0);
                    break;

                default:
                    content = Color.FromArgb(255, 20, 20, 20);
                    break;
            }

            return content;
        }

        #region Алгоритм поиска А*

        private async Task<List<Point>> FindPath(int[,] field, Point start, Point goal)
        {
            var closedSet = new Collection<PathNode>();
            var openSet = new Collection<PathNode>();

            PathNode startNode = new PathNode()
            {
                Position = start,
                CameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
            };
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                var currentNode = openSet.OrderBy(node =>
                  node.EstimateFullPathLength).First();

                if (currentNode.Position == goal)
                    return await Task.FromResult(GetPathForNode(currentNode));

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (var neighbourNode in GetNeighbours(currentNode, goal, field))
                {
                    if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
                        continue;
                    var openNode = openSet.FirstOrDefault(node =>
                      node.Position == neighbourNode.Position);

                    if (openNode == null)
                        openSet.Add(neighbourNode);
                    else if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                    {
                        openNode.CameFrom = currentNode;
                        openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                    }
                }
            }

            return null;
        }

        private int GetDistanceBetweenNeighbours()
        {
            return 1;
        }

        private int GetHeuristicPathLength(Point from, Point to)
        {
            
            //Полный перебор
            //var DeltaX = Math.Abs(from.X - to.X);
            //var DeltaY = Math.Abs(from.Y - to.Y);
            //var Dist = Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);

            //return Convert.ToInt32(Math.Round(Dist));

            return Convert.ToInt32(Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y));
        }

        private Collection<PathNode> GetNeighbours(PathNode pathNode, Point goal, int[,] field)
        {
            var result = new Collection<PathNode>();

            Point[] neighbourPoints = new Point[4];
            neighbourPoints[0] = new Point(pathNode.Position.X + 1, pathNode.Position.Y);
            neighbourPoints[1] = new Point(pathNode.Position.X - 1, pathNode.Position.Y);
            neighbourPoints[2] = new Point(pathNode.Position.X, pathNode.Position.Y + 1);
            neighbourPoints[3] = new Point(pathNode.Position.X, pathNode.Position.Y - 1);

            foreach (var point in neighbourPoints)
            {
                if (point.X < 0 || point.X >= field.GetLength(0))
                    continue;
                if (point.Y < 0 || point.Y >= field.GetLength(1))
                    continue;

                if ((field[Convert.ToInt32(point.X), Convert.ToInt32(point.Y)] != 0)
                    && (field[Convert.ToInt32(point.X), Convert.ToInt32(point.Y)] != 1))
                    continue;

                var neighbourNode = new PathNode()
                {
                    Position = point,
                    CameFrom = pathNode,
                    PathLengthFromStart = pathNode.PathLengthFromStart +
                    GetDistanceBetweenNeighbours(),
                    HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
                };
                result.Add(neighbourNode);
            }
            return result;
        }

        private List<Point> GetPathForNode(PathNode pathNode)
        {
            var result = new List<Point>();
            var currentNode = pathNode;
            while (currentNode != null)
            {
                result.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }
            result.Reverse();
            return result;
        }

        #endregion
    }
}
