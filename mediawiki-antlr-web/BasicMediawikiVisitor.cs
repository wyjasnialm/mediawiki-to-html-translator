using System.Collections.Generic;
using System.Linq;
using mediawiki_antlr_web.Gen;
using mediawiki_antlr_web.Models;

namespace mediawiki_antlr_web
{    
    public class BasicMediawikiVisitor : MediawikiBaseVisitor<object?>
    {
        public List<ParserLine> Lines = new List<ParserLine>();
        
        public override object? VisitHeading(MediawikiParser.HeadingContext context)
        {
            int level;
            var item = context.GetChild(0);
            var itemType = item.GetType();
            MediawikiParser.PlaintextContext plaintext;

            if (itemType == typeof(MediawikiParser.Heading1Context))
            {
                level = 1;
                plaintext = ((MediawikiParser.Heading1Context) item).plaintext();
            }
            else if (itemType == typeof(MediawikiParser.Heading2Context))
            {
                level = 2;
                plaintext = ((MediawikiParser.Heading2Context) item).plaintext();
            }
            else if (itemType == typeof(MediawikiParser.Heading3Context))
            {
                level = 3;
                plaintext = ((MediawikiParser.Heading3Context) item).plaintext();
            }
            else if (itemType == typeof(MediawikiParser.Heading4Context))
            {
                level = 4;
                plaintext = ((MediawikiParser.Heading4Context) item).plaintext();
            }
            else if (itemType == typeof(MediawikiParser.Heading5Context))
            {
                level = 5;
                plaintext = ((MediawikiParser.Heading5Context) item).plaintext();
            }
            else
            {
                level = 6;
                plaintext = ((MediawikiParser.Heading6Context) item).plaintext();
            }
            
            var html = "<h" + level + ">" + plaintext.GetText() + "</h" + level + ">";
            Lines.Add(new ParserLine() { Content = html });

            return base.VisitHeading(context);
        }
        
        public override object? VisitParagraph(MediawikiParser.ParagraphContext context)
        {
            var content = context.content();
            var html = "<p>";

            foreach (var item in content.children)
            {
                var itemType = item.GetType();

                if (itemType == typeof(MediawikiParser.FormattedContext))
                {
                    var itemChild = item.GetChild(0);
                    var itemChildType = itemChild.GetType();
                    MediawikiParser.UnformattedContext unformatted;

                    if (itemChildType == typeof(MediawikiParser.ItalicContext))
                    {
                        unformatted = ((MediawikiParser.ItalicContext) itemChild).unformatted();
                        html += "<i>" + BuildHtmlForUnformatted(unformatted) + "</i>";
                    }
                    else if (itemChildType == typeof(MediawikiParser.BoldContext))
                    {
                        unformatted = ((MediawikiParser.BoldContext) itemChild).unformatted();
                        html += "<b>" + BuildHtmlForUnformatted(unformatted) + "</b>";
                    }
                    else
                    {
                        unformatted = ((MediawikiParser.Bold_italicContext) itemChild).unformatted();
                        html += "<b><i>" + BuildHtmlForUnformatted(unformatted) + "</i></b>";
                    }
                }
                else if (itemType == typeof(MediawikiParser.UnformattedContext))
                {
                    html += BuildHtmlForUnformatted((MediawikiParser.UnformattedContext) item);
                }
            }

            html += "</p>";
            
            Lines.Add(new ParserLine() { Content = html });

            return base.VisitParagraph(context);
        }
        
        private string BuildHtmlForUnformatted(MediawikiParser.UnformattedContext context)
        {
            var html = "";

            foreach (var item in context.children)
            {
                var itemType = item.GetType();
                
                if (itemType == typeof(MediawikiParser.LinkContext))
                {
                    var itemChild = item.GetChild(0);
                    var itemChildType = itemChild.GetType();

                    if (itemChildType == typeof(MediawikiParser.External_linkContext))
                    {
                        html += BuildHtmlForExternalLink((MediawikiParser.External_linkContext) itemChild);
                    }
                    else if (itemChildType == typeof(MediawikiParser.Internal_linkContext))
                    {
                        html += BuildHtmlForInternalLink((MediawikiParser.Internal_linkContext) itemChild);
                    }
                }
                else if (itemType == typeof(MediawikiParser.PlaintextContext))
                {
                    html += item.GetText();
                }
            }

            return html;
        }

        private string BuildHtmlForExternalLink(MediawikiParser.External_linkContext context)
        {
            var text = context.external_link_title()?.GetText() ?? context.external_link_uri().GetText();
            return "<a href=\"" + context.external_link_uri().GetText() + "\">" + text + "</a>";
        }
        
        private string BuildHtmlForInternalLink(MediawikiParser.Internal_linkContext context)
        {
            var text = context.internal_link_title()?.GetText() ?? context.internal_link_ref().GetText();
            return "<a href=\"/wiki/" + context.internal_link_ref().GetText().Replace(' ', '_') + "\">" + text + "</a>";
        }

        public override object? VisitHorizontal_line(MediawikiParser.Horizontal_lineContext context)
        {
            var html = "<hr />";
            Lines.Add(new ParserLine() { Content = html });

            return base.VisitHorizontal_line(context);
        }

        private TreeListElement BuildUnorderedListItem(MediawikiParser.Unordered_list_itemContext item, TreeList? root = null, int level = 1)
        {
            TreeListElement treeElement;
            var itemContent = item.list_item_content();
            var content = itemContent.content();
            var unorderedListItem = itemContent.unordered_list_item();
            var orderedListItem = itemContent.ordered_list_item();

            if (content != null)
            {
                treeElement = new TreeListElement(root, level, content.GetText());
            }
            else if (unorderedListItem != null)
            {
                var nestedTree = new TreeList("ul");
                nestedTree.Add(BuildUnorderedListItem(unorderedListItem, nestedTree, level + 1));
                treeElement = new TreeListElement(root, level, null, nestedTree);
            }
            else
            {
                var nestedTree = new TreeList("ol");
                nestedTree.Add(BuildOrderedListItem(orderedListItem, nestedTree, level + 1));
                treeElement = new TreeListElement(root, level, null, nestedTree);
            }

            return treeElement;
        }
        
        private TreeListElement BuildOrderedListItem(MediawikiParser.Ordered_list_itemContext item, TreeList? root = null, int level = 1)
        {
            TreeListElement treeElement;
            var itemContent = item.list_item_content();
            var content = itemContent.content();
            var unorderedListItem = itemContent.unordered_list_item();
            var orderedListItem = itemContent.ordered_list_item();

            if (content != null)
            {
                treeElement = new TreeListElement(root, level, content.GetText());
            }
            else if (unorderedListItem != null)
            {
                var nestedTree = new TreeList("ul");
                nestedTree.Add(BuildUnorderedListItem(unorderedListItem, nestedTree, level + 1));
                treeElement = new TreeListElement(root, level, null, nestedTree);
            }
            else
            {
                var nestedTree = new TreeList("ol");
                nestedTree.Add(BuildOrderedListItem(orderedListItem, nestedTree, level + 1));
                treeElement = new TreeListElement(root, level, null, nestedTree);
            }

            return treeElement;
        }
        
        private string BuildListHtml(TreeList list)
        {
            var html = "\n<" + list.Type + ">";

            foreach (var item in list)
            {
                html += "\n<li>" + item.Content;

                if (item.IsNestedList())
                {
                    html += BuildListHtml(item.List);
                }
                
                html += "\n</li>";
            }
            
            html += "\n</" + list.Type + ">";

            return html;
        }
        
        private string PrintList(TreeList list, int level = 0)
        {
            var text = "level" + (level + 1) + "---------------";
            var tab = "";

            for (var i = 0; i < level; i++)
            {
                tab += "\t";
            }

            foreach (var item in list)
            {
                text += "\n" + tab + item.Content + ": depth = " + item.Depth;
                
                if (item.IsNestedList())
                {
                    text += "\n" + tab + PrintList(item.List, level + 1);
                }
            }

            return text;
        }

        public override object? VisitUnordered_list(MediawikiParser.Unordered_listContext context)
        {
            var tree = new TreeList("ul");
            var unorderedListItems = context.unordered_list_item();

            foreach (var item in unorderedListItems)
            {
                tree.RecalculateDepths();
                //Console.WriteLine("write\n" + PrintList(tree) + "\n");
                var treeElement = BuildUnorderedListItem(item);

                if (tree.Count == 0)
                {
                    tree.Add(treeElement);
                }
                else
                {
                    if (treeElement.IsNestedList())
                    {
                        var lastElement = tree.Last();

                        if (!lastElement.IsNestedList())
                        {
                            lastElement.AttachList(treeElement.List);
                        }
                        else
                        {
                            if (treeElement.Depth < lastElement.Depth)
                            {
                                while (treeElement.Depth > 0)
                                {
                                    treeElement = treeElement.List.First();
                                    lastElement = lastElement.List.Last();
                                }
                                
                                lastElement.Root.Add(treeElement);
                            }
                            else if (treeElement.Depth == lastElement.Depth)
                            {
                                var lastElementLastLeafRoot = lastElement.GetLastLeaf().Root;
                                lastElementLastLeafRoot.Add(treeElement.GetLastLeaf());
                            }
                            else
                            {
                                while (lastElement.Depth > 0)
                                {
                                    treeElement = treeElement.List.First();
                                    lastElement = lastElement.List.Last();
                                }
                                
                                lastElement.AttachList(treeElement.List);
                            }
                        }
                    }
                    else
                    {
                        tree.Add(treeElement);
                    }
                }
            }
            
            tree.RecalculateDepths();
            //Console.WriteLine(PrintList(tree));
            
            var html = BuildListHtml(tree);
            
            Lines.Add(new ParserLine() { Content = html });

            return base.VisitUnordered_list(context);
        }
        
        public override object? VisitOrdered_list(MediawikiParser.Ordered_listContext context)
        {
            var tree = new TreeList("ol");
            var orderedListItems = context.ordered_list_item();

            foreach (var item in orderedListItems)
            {
                tree.RecalculateDepths();
                //Console.WriteLine("write\n" + PrintList(tree) + "\n");
                var treeElement = BuildOrderedListItem(item);

                if (tree.Count == 0)
                {
                    tree.Add(treeElement);
                }
                else
                {
                    if (treeElement.IsNestedList())
                    {
                        var lastElement = tree.Last();

                        if (!lastElement.IsNestedList())
                        {
                            lastElement.AttachList(treeElement.List);
                        }
                        else
                        {
                            if (treeElement.Depth < lastElement.Depth)
                            {
                                while (treeElement.Depth > 0)
                                {
                                    treeElement = treeElement.List.First();
                                    lastElement = lastElement.List.Last();
                                }
                                
                                lastElement.Root.Add(treeElement);
                            }
                            else if (treeElement.Depth == lastElement.Depth)
                            {
                                var lastElementLastLeafRoot = lastElement.GetLastLeaf().Root;
                                lastElementLastLeafRoot.Add(treeElement.GetLastLeaf());
                            }
                            else
                            {
                                while (lastElement.Depth > 0)
                                {
                                    treeElement = treeElement.List.First();
                                    lastElement = lastElement.List.Last();
                                }
                                
                                lastElement.AttachList(treeElement.List);
                            }
                        }
                    }
                    else
                    {
                        tree.Add(treeElement);
                    }
                }
            }
            
            tree.RecalculateDepths();
            //Console.WriteLine(PrintList(tree));
            
            var html = BuildListHtml(tree);
            
            Lines.Add(new ParserLine() { Content = html });

            return base.VisitOrdered_list(context);
        }
        
        public override object? VisitImage(MediawikiParser.ImageContext context)
        {
            var url = context.image_filename().GetText();
            var caption = context.image_caption().GetText();
            var html = "<a href=\"" + url + "\" title=\"" + caption + "\"><img src=\"" + url + "\" alt=\"" + caption + "\" /></a>";
            Lines.Add(new ParserLine() { Content = html });

            return base.VisitImage(context);
        }

        public override object? VisitTable(MediawikiParser.TableContext context)
        {
            Lines.Add(new ParserLine() { Content = "<table>" });
            var rows = context.table_row();

            foreach (var row in rows)
            {
                Lines.Add(new ParserLine() { Content = "<tr>" });
                var cols = row.table_cell();

                foreach (var col in cols)
                {
                    Lines.Add(new ParserLine() { Content = "<td>" });
                    Visit(col.line());
                    Lines.Add(new ParserLine() { Content = "</td>" });
                }
                
                Lines.Add(new ParserLine() { Content = "</tr>" });
            }
            
            Lines.Add(new ParserLine() { Content = "</table>" });

            return null;
        }
    }
}