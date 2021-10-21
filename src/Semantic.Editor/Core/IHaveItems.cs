using System;
using System.Collections.Generic;
using System.Text;

namespace Semantic.Editor.Core
{
    public interface IHaveItems
    {
        public IList<Block> ItemList { get; set; }
        
        public int SelectedIndex { get; set; }
    }
}
