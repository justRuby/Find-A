using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;

namespace FindAStar
{
    using FindAStar.Extension;
    using FindAStar.Model;
    using FindAStar.Enumerators;

    public partial class MainWindow : Window
    {
        private const int MATRIX_SIZE = 24;
        private static ASCore aSCore;

        private List<BlockModel> BlockList { get; set; }
        private List<BlockModel> CurrentBlockList { get; set; }
        private BlockModel TargetBlock { get; set; }

        private NodeType SelectedMode;

        public MainWindow()
        {
            InitializeComponent();
            aSCore = ASCore.GetInstance();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double left = 0;
            double top = 0;
            double indent = 0d;

            BlockList = new List<BlockModel>();
            CurrentBlockList = new List<BlockModel>();

            for (int i = 0; i < MATRIX_SIZE; i++)
            {
                for (int j = 0; j < MATRIX_SIZE * 2; j++)
                {
                    BlockModel blockModel = new BlockModel
                    (
                        new Button()
                        {
                            Name = "block_" + i + "_" + j,
                            //Content = i + j,
                            //Foreground = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)),
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Width = 10,
                            Height = 10,
                            Margin = new Thickness(left + indent, top + indent, 0, 0),
                            Background = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20)),
                            BorderBrush = null
                        },
                        new Point(i, j),
                        NodeType.Passable
                    );

                    blockModel.Block.Click += BlockClick;

                    left += 10;

                    BlockList.Add(blockModel);
                    viewArrayGrid.Children.Add(blockModel.Block);
                }
                left = 0;
                top += 10;
            }
        }

        public void BlockClick(object sender, RoutedEventArgs e)
        {
            var block = BlockList.Where(x => x.Block == sender as Button).FirstOrDefault();
            BlockModel findedObj;

            if (block == null)
                return;

            CheckAndRemoveItem(block);

            switch (SelectedMode)
            {
                case NodeType.Passable:
                    block.NType = NodeType.Passable;
                    break;

                case NodeType.Wall:
                    block.NType = NodeType.Wall;
                    break;

                case NodeType.Target:

                    if (block.NType == NodeType.Target)
                        break;

                    findedObj = BlockList.Where(x => x.NType == NodeType.Target).FirstOrDefault();
                    if(findedObj != null)
                    {
                        block.NType = findedObj.NType;
                        findedObj.NType = NodeType.Passable;
                        TargetBlock = block;
                    }
                    else
                    {
                        block.NType = NodeType.Target;
                        TargetBlock = block;
                    }

                    break;

                case NodeType.Current:

                    block.NType = NodeType.Current;
                    CurrentBlockList.Add(block);

                    break;

                default:
                    break;
            }


        }

        private void CheckAndRemoveItem(BlockModel item)
        {
            BlockModel findedObj;

            switch (item.NType)
            {
                case NodeType.Passable:
                    break;
                case NodeType.Wall:
                    break;
                case NodeType.Target:

                    if (SelectedMode == NodeType.Target)
                        break;

                    TargetBlock.NType = NodeType.Passable;
                    TargetBlock = null;

                    break;
                case NodeType.Current:

                    if (SelectedMode == NodeType.Current)
                        break;

                    findedObj = CurrentBlockList.Where(x => x == item).FirstOrDefault();

                    if (findedObj != null)
                    {
                        findedObj.NType = NodeType.Passable;
                        CurrentBlockList.Remove(findedObj);
                    }

                    break;
                default:
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var obj = sender as Button;

            switch (obj.Name)
            {
                case "borderSelectButton":
                    statusLabel.Content = "Выбран режим преграды";
                    SelectedMode = NodeType.Wall;
                    break;

                case "startSelectButton":
                    statusLabel.Content = "Выбран режим стартовой позиции";
                    SelectedMode = NodeType.Current;
                    break;

                case "endSelectButton":
                    statusLabel.Content = "Выбран режим цели";
                    SelectedMode = NodeType.Target;
                    break;

                default:
                    statusLabel.Content = "Выбран режим проходимого путя";
                    SelectedMode = NodeType.Passable;
                    break;
            }
        }

        private async void FindPathButton_Click(object sender, RoutedEventArgs e)
        {
            Point startPosition = new Point(-1, -1);
            Point targetPosition = new Point(-1, -1);
            var field = new int[MATRIX_SIZE, MATRIX_SIZE * 2];
            List<Point> wallList = new List<Point>();

            targetPosition = BlockList.Where(x => x.NType == NodeType.Target).Select(x => x.Position).FirstOrDefault();

            if (targetPosition == null)
                return;

            var searchWall = BlockList.Where(x => x.NType == NodeType.Wall).Select(x => x.Position);
            foreach (var item in searchWall)
                wallList.Add(item);

            int countPaths = CurrentBlockList.Count;
            List<Point>[] PathsList = new List<Point>[countPaths];

            ASpeedModifier speedModifier = new ASpeedModifier()
            {
                SearchDelayInMilliseconds = 10,
                ViewDelayInMilliseconds = 20
            };

            List<List<Point>> pathListArray = new List<List<Point>>();

            long timeElapseWithDisplay = 0;
            long timeElapseWithoutDisplay = 0;

            var tasks = CurrentBlockList.Select(async (x) =>
               {
                   var watchWithDisplay = Stopwatch.StartNew();
                   var watchWithoutDisplay = Stopwatch.StartNew();

                   await aSCore.FindPathWithoutDisplayAsync(field, startPosition, targetPosition, wallList)
                       .ContinueWith(t => watchWithoutDisplay.Stop());

                   pathListArray.Add(await aSCore.FindPathAsync(field, x.Position, targetPosition, wallList, BlockList, speedModifier));

                   watchWithDisplay.Stop();

                   timeElapseWithDisplay += watchWithDisplay.ElapsedMilliseconds;
                   timeElapseWithoutDisplay += watchWithoutDisplay.ElapsedMilliseconds;
               });

            await Task.WhenAll(tasks);

            //Тест
            BlockList.ForEach((x) =>
            {
                if (x.NType == NodeType.SearchPath)
                    x.NType = NodeType.Passable;
            });

            pathListArray.ForEach(async x => await ViewPass(x, speedModifier));

            elapsedTimeLabel.Content = " Скорость: " + Math.Round((double)timeElapseWithoutDisplay / timeElapseWithDisplay, 4)
                                     + " Всего: " + timeElapseWithDisplay
                                     + " мс. | Всего за поиск: " + timeElapseWithoutDisplay
                                     + " мс.";
        }

        private async Task<bool> ViewPass(List<Point> pointList, ASpeedModifier modifier)
        {
            if(pointList == null)
                return false;

            var searchBlock = BlockList.Where((x) =>
            {
                
                return ComparePoints(pointList, x.Position);
            });
            foreach (var item in searchBlock)
            {
                modifier.TimePassedForView += modifier.ViewDelayInMilliseconds;
                await Task.Delay(modifier.ViewDelayInMilliseconds);
                item.NType = NodeType.Path;
            }

            return await Task.FromResult(true);
        }

        private bool ComparePoints(List<Point> list, Point x)
        {
            var searchComp = list.Where(p => p == x);
            foreach (var item in searchComp)
            {
                return true;
            }

            return false;
        }

        private void СlearFieldButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentBlockList.Clear();
            TargetBlock = null;

            foreach (var item in BlockList)
            {
                item.NType = NodeType.Passable;
            }
        }

        private void ClearPathButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in BlockList)
            {
                if (item.NType == NodeType.Path)
                    item.NType = NodeType.Passable;
            }
        }
    }
}
