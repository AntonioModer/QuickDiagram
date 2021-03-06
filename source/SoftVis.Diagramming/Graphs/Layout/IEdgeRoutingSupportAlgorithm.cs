﻿using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout
{
    internal interface IEdgeRoutingSupportAlgorithm<TVertex, TEdge> : IAlgorithm
        where TEdge: IEdge<TVertex>
    {
        IDictionary<TEdge, Route> InterimRoutePointsOfEdges { get; }
    }
}
