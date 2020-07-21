﻿using System;
using System.Collections.Generic;
using Procool.Utils;
using UnityEditor.U2D.Animation;
using UnityEngine;

namespace Procool.Map.SpacePartition
{
    public partial class Space : ObjectWithPool<Space>
    {
        public readonly List<Region> Regions = new List<Region>();

        public static Space Get()
        {
            var space = GetInternal();
            space.Regions.Clear();
            return space;
        }

        public static void Release(Space space)
        {
            if(!space)
                return;
            foreach (var region in space.Regions)
            {
                Region.Release(region);
            }
            ReleaseInternal(space);
        }

        public Region CreateRegion(List<Vertex> vertices, List<Edge> edges)
        {
            var region = Region.Get(this);
            
            region.StartConstruct();
            region.AddVertices(vertices);
            region.AddEdges(edges);
            region.EndConstruct();

            Regions.Add(region);

            foreach (var edge in edges)
            {
                edge.AddRegion(region);
                var (a, b) = edge.Points;
                a.AddEdge(edge);
                b.AddEdge(edge);
                
            }

            return region;
        }

        public void SplitByLine(Vector2 origin, Vector2 direction)
        {
            var count = Regions.Count;
            for (var i = 0; i < count; i++)
            {
                SplitRegionByLine(Regions[i], origin, direction);
            }
        }

        public (Region, Region) SplitRegionByLine(Region region, Vector2 origin, Vector2 direction)
        {
            if(region.Space != this)
                throw new Exception("Region not belongs to space.");

            var (regionA, regionB) = region.SplitByLine(origin, direction);

            if (!regionA)
                return (null, null);

            var index = Regions.IndexOf(region);
            Regions[index] = regionA;
            Regions.Add(regionB);
            
            Region.Release(region);
            
            return (regionA, regionB);
        }

    }
}