using System.Collections.Generic;
using Xonix.Common.Characters;

namespace Xonix.Core
{
    public class EnemyPool
    {
        private GameManager gameManager;
        private Queue<Enemy> enemiesQueue;

        public EnemyPool(GameManager gm) 
        {
            gameManager = gm;
            enemiesQueue = new Queue<Enemy>();
        }

        public void AddToPool(Enemy enemy) 
        {
            enemiesQueue.Enqueue(enemy);
        }

        public Enemy SpawnFromPool() 
        {
            return enemiesQueue.Dequeue();
        }
    }
}
