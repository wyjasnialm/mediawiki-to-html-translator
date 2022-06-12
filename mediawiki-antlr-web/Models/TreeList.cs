using System.Collections.Generic;

namespace mediawiki_antlr_web.Models
{
    public class TreeList : List<TreeListElement>
    {
        public TreeListElement? Root;
        public string Type;
        //public int Depth = 1;

        public TreeList(string type)
        {
            Type = type;
        }

        public new void Add(TreeListElement item)
        {
            base.Add(item);

            item.Root = this;
        }
        
        public void RecalculateDepths()
        {
            foreach (var item in this)
            {
                var depth = 0;
                var itemToUpdate = item.GetLastLeaf();

                while (itemToUpdate != null)
                {
                    itemToUpdate.Depth = depth++;
                    itemToUpdate = itemToUpdate.Root.Root;
                }
            }
        }

        /*public void RecalculateDepths1(TreeList? list = null, int depth = 0)
        {
            if (list == null && depth == 0)
            {
                list = this;
            }
            
            foreach (var item in list)
            {
                if (!item.IsNestedList())
                {
                    item.Depth = 0;
                }
                else
                {
                    var itemLastLeaf = item.GetLastLeaf();
                    itemLastLeaf.Depth = 0;
                }
            }
        }*/
    }
}