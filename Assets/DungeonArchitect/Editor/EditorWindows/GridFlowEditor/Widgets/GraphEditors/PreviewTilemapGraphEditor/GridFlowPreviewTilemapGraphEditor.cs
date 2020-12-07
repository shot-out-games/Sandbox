﻿using DungeonArchitect.Builders.GridFlow;
using DungeonArchitect.Builders.GridFlow.Graphs.Preview.Tilemap;
using DungeonArchitect.Builders.GridFlow.Tilemap;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets;
using DungeonArchitect.UI.Widgets.GraphEditors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Editors.GridFlow.Tilemap
{

    public delegate void OnGridFlowPreviewTileClicked(GridFlowTilemap tilemap, int tileX, int tileY);

    public class GridFlowPreviewTilemapGraphEditor : GraphEditor
    {
        private Dictionary<System.Guid, Rect> itemScreenPositions = new Dictionary<System.Guid, Rect>();
        public event OnGridFlowPreviewTileClicked TileClicked;

        protected override GraphContextMenu CreateContextMenu()
        {
            return null;
        }

        protected override IGraphLinkRenderer CreateGraphLinkRenderer()
        {
            return new StraightLineGraphLinkRenderer();
        }

        protected override string GetGraphNotInitializedMessage()
        {
            return "Graph not initialized";
        }

        protected override void InitializeNodeRenderers(GraphNodeRendererFactory nodeRenderers)
        {
            var tilemapRenderer = new GridFlowPreviewTilemapNodeRenderer();
            tilemapRenderer.GridFlowAbstractGraphItemRendered += OnGraphItemRendered;
            nodeRenderers.RegisterNodeRenderer(typeof(GridFlowPreviewTilemapGraphNode), tilemapRenderer);
        }

        private void OnGraphItemRendered(GridFlowItem item, Rect screenBounds)
        {
            itemScreenPositions[item.itemId] = screenBounds;
        }

        public override void Draw(UISystem uiSystem, UIRenderer renderer)
        {
            itemScreenPositions.Clear();
            base.Draw(uiSystem, renderer);

            var node = graph.Nodes.Count > 0 ? graph.Nodes[0] as GridFlowPreviewTilemapGraphNode : null;
            if (node != null)
            {
                var items = node.AbstractGraph.GetAllItems();
                foreach (var item in items)
                {
                    if (!itemScreenPositions.ContainsKey(item.itemId)) continue;
                    foreach (var referencedItemId in item.referencedItemIds)
                    {
                        if (!itemScreenPositions.ContainsKey(referencedItemId)) continue;
                        DrawReferenceLink(renderer, itemScreenPositions[item.itemId], itemScreenPositions[referencedItemId]);
                    }
                }
            }
        }

        void DrawReferenceLink(UIRenderer renderer, Rect start, Rect end)
        {
            var centerA = start.center;
            var centerB = end.center;
            var radiusA = Mathf.Max(start.size.x, start.size.y) * 0.5f;
            var radiusB = Mathf.Max(end.size.x, end.size.y) * 0.5f;
            var vecAtoB = (centerB - centerA);
            var lenAtoB = vecAtoB.magnitude;
            var dirAtoB = vecAtoB / lenAtoB;

            var startPos = centerA + dirAtoB * radiusA;
            var endPos = centerA + dirAtoB * (lenAtoB - radiusB);
            StraightLineGraphLinkRenderer.DrawLine(renderer, startPos, endPos, camera, Color.red, 2);
        }

        public override void HandleInput(Event e, UISystem uiSystem)
        {
            base.HandleInput(e, uiSystem);


            var buttonId = 0;
            if (e.type == EventType.MouseDown && e.button == buttonId)
            {
                var clickPos = camera.ScreenToWorld(e.mousePosition);

                var node = graph.Nodes.Count > 0 ? graph.Nodes[0] as GridFlowPreviewTilemapGraphNode : null;
                if (node != null)
                {
                    var tilemap = node.Tilemap;
                    clickPos -= node.Position;
                    var tileX = Mathf.FloorToInt(clickPos.x / node.tileRenderSize);
                    var tileY = Mathf.FloorToInt(clickPos.y / node.tileRenderSize);
                    tileY = tilemap.Height - tileY - 1;
                    if (tileX >= 0 && tileX < tilemap.Width && tileY >= 0 && tileY < tilemap.Height)
                    {
                        if (TileClicked != null)
                        {
                            TileClicked.Invoke(tilemap, tileX, tileY);
                        }
                    }
                }
            }
        }

        protected override void OnMenuItemClicked(object userdata, GraphContextMenuEvent e)
        {

        }

        public void UpdateGridSpacing()
        {
            var node = graph.Nodes.Count > 0 ? graph.Nodes[0] as GridFlowPreviewTilemapGraphNode : null;
            if (node != null)
            {
                EditorStyle.gridCellSpacing = node.tileRenderSize * 2;
            }
        }
    }
}
