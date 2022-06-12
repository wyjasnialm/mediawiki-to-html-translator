using System.Linq;

namespace mediawiki_antlr_web.Models
{
    public class TreeListElement
    {
        public TreeList? Root;
        public int Level;
        public string? Content;
        public TreeList? List;
        public int Depth;
        
        public TreeListElement(TreeList? root, int level, string? content, TreeList? list = null)
        {
            Root = root;
            Level = level;
            Content = content;
            List = list;
            Depth = list?.First().Depth + 1 ?? 0;

            if (list != null)
            {
                list.Root = this;
            }
        }

        public bool IsNestedList()
        {
            return List != null;
        }

        public void AttachList(TreeList list)
        {
            List = list;
            Depth = List.Last().Depth + 1;
            list.Root = this;
        }

        public TreeListElement GetLastLeaf()
        {
            if (IsNestedList())
            {
                return List.Last().GetLastLeaf();
            }
            
            return this;
        }
    }
}