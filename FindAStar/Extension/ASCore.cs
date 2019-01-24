using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FindAStar.Extension
{
    using FindAStar.Enumerators;
    using FindAStar.Model;

    public class ASCore
    {

        private static ASCore _instance;
        private static readonly object syncRoot = new Object();

        private ASCore() { }

        public static ASCore GetInstance()
        {
            if (_instance == null)
            {
                lock (syncRoot)
                {
                    if (_instance == null)
                        _instance = new ASCore();
                }
            }
            return _instance;
        }

        public async Task<List<Point>> FindPath(int[,] field, Point start, Point goal, List<Point> wall)
        {
            var closedSet = new Collection<PathNode>();
            var openSet = new Collection<PathNode>();

            PathNode startNode = new PathNode()
            {
                Position = start,
                CameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = await GetHeuristicPathLength(start, goal),
                StartOrDefault = true
            };
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.OrderBy(node =>
                  node.EstimateFullPathLength).First();

                if (currentNode.Position == goal)
                {
                    currentNode.StartOrDefault = true;
                    return await Task.FromResult(await GetPathForNode(currentNode));
                }
                    
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (var neighbourNode in await GetNeighbours(currentNode, goal, field))
                {
                    if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
                        continue;

                    var openNode = openSet.FirstOrDefault(node => node.Position == neighbourNode.Position);

                    bool isExist = false;
                    var searchWall = wall.Where(x => x == neighbourNode.Position);
                    foreach (var item in searchWall)
                    {
                        isExist = true;
                        break;
                    }

                    if (isExist)
                        continue;

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

        private async Task<int> GetDistanceBetweenNeighbours()
        {
            return await Task.FromResult(1);
        }
        private async Task<int> GetHeuristicPathLength(Point from, Point to)
        {
            return await Task.FromResult((int)(Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y)));
        }
        private async Task<Collection<PathNode>> GetNeighbours(PathNode pathNode, Point goal, int[,] field)
        {
            var result = new Collection<PathNode>();

            Point[] neighbourPoints = new Point[4];
            neighbourPoints[0] = new Point(pathNode.Position.X + 1, pathNode.Position.Y);
            neighbourPoints[1] = new Point(pathNode.Position.X - 1, pathNode.Position.Y);
            neighbourPoints[2] = new Point(pathNode.Position.X, pathNode.Position.Y + 1);
            neighbourPoints[3] = new Point(pathNode.Position.X, pathNode.Position.Y - 1);

            foreach (var point in neighbourPoints)
            {
                var pointX = (int)point.X;
                var pointY = (int)point.Y;

                if (pointX < 0 || pointX >= field.GetLength(0))
                    continue;
                if (pointY < 0 || pointY >= field.GetLength(1))
                    continue;

                if ((field[pointX, pointY] != 0) && (field[pointX, pointY] != 1))
                    continue;

                var neighbourNode = new PathNode()
                {
                    Position = point,
                    CameFrom = pathNode,
                    PathLengthFromStart = pathNode.PathLengthFromStart + await GetDistanceBetweenNeighbours(),
                    HeuristicEstimatePathLength = await GetHeuristicPathLength(point, goal)
                };

                result.Add(neighbourNode);
            }

            return await Task.FromResult(result);
        }
        private async Task<List<Point>> GetPathForNode(PathNode pathNode)
        {
            var result = new List<Point>();
            var currentNode = pathNode;
            while (currentNode != null)
            {
                if (currentNode.StartOrDefault != true)
                    result.Add(currentNode.Position);

                currentNode = currentNode.CameFrom;
            }
            result.Reverse();
            return await Task.FromResult(result);
        }
    }
}
