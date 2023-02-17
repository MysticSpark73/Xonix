using UnityEngine;

namespace Xonix.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Common.Grid.Grid grid;
        private void Awake()
        {
            grid.CreateGrid();
        }

        // Start is called before the first frame update
        void Start()
        {
            //InitIALIZE grid and prepare game
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
