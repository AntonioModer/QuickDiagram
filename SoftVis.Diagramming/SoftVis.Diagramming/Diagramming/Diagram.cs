﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.Modeling;
using System;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram is a partial, graphical representation of a model. 
    /// A diagram shows a subset of the model and there can be many diagrams depicting different areas/aspects of the same model.
    /// A diagram consists of shapes that represent model elements.
    /// The shapes form a directed graph: some shapes are nodes in the graph and others are connectors between nodes.
    /// The layout of the shapes (relative positions and size) also conveys meaning.
    /// </summary>
    [DebuggerDisplay("VertexCount={_graph.VertexCount}, EdgeCount={_graph.EdgeCount}")]
    public class Diagram
    {
        private DiagramGraph _graph = new DiagramGraph();

        public event EventHandler<DiagramShape> ShapeAdded;
        public event EventHandler<DiagramShape> ShapeModified;
        public event EventHandler<DiagramShape> ShapeRemoved;

        public IEnumerable<DiagramNode> Nodes
        {
            get { return _graph.Vertices.OfType<DiagramNode>(); }
        }

        public IEnumerable<DiagramConnector> Connectors
        {
            get { return _graph.Edges.OfType<DiagramConnector>(); }
        }

        public DiagramRect GetEnclosingRect()
        {
            return Nodes.Select(i => i.Rect).Union();
        }

        public void ShowModelElement(UmlModelElement modelElement)
        {
            if (modelElement is UmlTypeOrPackage)
            {
                ShowUmlTypeOrPackage((UmlTypeOrPackage)modelElement);
            }
            else if (modelElement is UmlRelationship)
            {
                ShowUmlRelationship((UmlRelationship)modelElement);
            }
        }

        private void ShowUmlTypeOrPackage(UmlTypeOrPackage umlTypeOrPackage)
        {
            if (_graph.Vertices.Any(i => i.ModelElement == umlTypeOrPackage))
                return;

            var node = ModelToNodeTranslator.Translate(umlTypeOrPackage);
            _graph.AddVertex(node);
            SignalShapeAddedEvent(node);

            foreach (var relationship in umlTypeOrPackage.OutgoingRelationships)
            {
                if (_graph.Vertices.Any(i => i.ModelElement == relationship.SourceElement) &&
                    _graph.Vertices.Any(i => i.ModelElement == relationship.TargetElement))
                {
                    ShowModelElement(relationship);
                }
            }
        }

        private void ShowUmlRelationship(UmlRelationship umlRelationship)
        {
            if (_graph.Edges.Any(i => i.ModelElement == umlRelationship))
                return;

            var connector = ModelToConnectorTranslator.Translate(_graph, umlRelationship);
            _graph.AddEdge(connector);
            SignalShapeAddedEvent(connector);
        }

        private void SignalShapeAddedEvent(DiagramShape shape)
        {
            if (shape != null && ShapeAdded != null)
                ShapeAdded(this, shape);
        }

        private void SignalShapeModifiedEvent(DiagramShape shape)
        {
            if (shape != null && ShapeModified != null)
                ShapeModified(this, shape);
        }

        public void HideModelElement(UmlModelElement modelElement)
        {
            var node = _graph.FindNode(modelElement);
            _graph.RemoveVertex(node);

            Layout();

            if (node != null && ShapeRemoved != null)
                ShapeRemoved(this, node);
        }

        public void Layout()
        {
            Debug.WriteLine("Layout vertices {0}, edges {1}.", _graph.VertexCount, _graph.EdgeCount);

            var algorithm = new SimpleTreeLayoutAlgorithm(_graph);
            var newVertexPositions = algorithm.ComputeNewVertexPositions();

            _graph.PositionNodes(newVertexPositions);

            foreach (var vertex in _graph.Vertices)
                SignalShapeModifiedEvent(vertex);

            foreach (var edge in _graph.Edges)
                SignalShapeModifiedEvent(edge);
        }
    }
}
