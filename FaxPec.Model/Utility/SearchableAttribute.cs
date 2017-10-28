using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model
{
    /// <summary>
    /// Attributo.
    /// </summary>
    class SearchableAttribute : Attribute
    {
        private readonly bool _searchable;
        public SearchableAttribute(bool searchable) { this._searchable = searchable; }
        public bool IsSearchable { get { return _searchable; } }
        public override string ToString()
        {
            return "searchable = " +
                _searchable.ToString();
        }
    }
}
