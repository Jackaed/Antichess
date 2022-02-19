using System.Threading;
using UnityEngine;

namespace Assets.Scripts {
    public class BoardRenderer : MonoBehaviour {
        
        private GraphicalBoard _board;

        
        private void Start()
        {
            _board = new GraphicalBoard();
        }
    }
}