namespace AirportTracker.RankTree
{
    public class RankingTree
    {
        private RankingNode? root = null;

        public int GetRank(int value) 
        {
            if (root == null) return 1;

            int returnRank = 1;
            RankingNode current = root;
            while (current != null) 
            {
                if (value < current.Value)
                {
                    returnRank += current.Rank + 1;
                    current = current.Right;
                }
                else if(value == current.Value)
                {
                    returnRank += current.Rank;
                    break;
                }
                else 
                {
                    current = current.Left;
                }
            }

            return returnRank;
        }

        public void Insert(int id, int value) 
        {
            if (value == 0) return;

            if (root == null) 
            {
                root = new RankingNode() {Id = id, Value = value, Rank = 0};
                return;
            }

            RankingNode current = root;
            while (true) 
            {
                if (value >= current.Value)
                {
                    current.Rank++;
                    if (current.Left != null)
                    {
                        current = current.Left;
                    }
                    else
                    {
                        current.Left = new RankingNode() { Id = id, Value = value, Rank = 0 };
                        return;
                    }
                }
                else
                {
                    if (current.Right != null)
                    {
                        current = current.Right;
                    }
                    else
                    {
                        current.Right = new RankingNode() { Id = id, Value = value, Rank = 0 };
                        return;
                    }
                }
            }
        }

        private class RankingNode 
        {
            public int Id;
            public int Value;
            public int Rank;
            public RankingNode? Left;
            public RankingNode? Right;
        }
    }
}
