using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
    internal class GraphicalBoard : Board
    {
        public Dictionary<IPiece, GameObject> GameObjects = new();
        public override void AddPiece(IPiece piece, Vector2Int pos) {
            base.AddPiece(piece, pos);
            GameObjects.Add(piece, Object.Instantiate(piece.Model,
                ObjectLoader.GetRealCoords(pos), piece.Model.transform.rotation));
        }
        public override void MovePiece(Vector2Int from, Vector2Int to) {
            if (GameObjects.ContainsKey(Data[from.x, from.y]))
            {
                GameObjects[Data[from.x, from.y]].transform.position = ObjectLoader.GetRealCoords(to);
            }

            if (GameObjects.ContainsKey(Data[to.x, to.y]))
            {
                Object.Destroy(GameObjects[Data[to.x, to.y]]);
                GameObjects.Remove(Data[to.x, to.y]);
            }
            base.MovePiece(from, to);
        }
    }
}