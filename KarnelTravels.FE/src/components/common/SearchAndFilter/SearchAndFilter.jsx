import { Search, Filter, X, ChevronDown } from 'lucide-react';
import { useState } from 'react';

const SearchAndFilter = ({
  searchTerm,
  onSearchChange,
  filters = {},
  onFilterChange,
  placeholder = "Tìm kiếm...",
  showSearch = true,
  showFilters = true,
  filterOptions = {}
}) => {
  const [showFilterPanel, setShowFilterPanel] = useState(false);
  const [localSearchTerm, setLocalSearchTerm] = useState(searchTerm || '');

  const handleSearchSubmit = (e) => {
    e.preventDefault();
    onSearchChange?.(localSearchTerm);
  };

  const handleSearchChange = (e) => {
    setLocalSearchTerm(e.target.value);
  };

  const handleSearchKeyDown = (e) => {
    if (e.key === 'Enter') {
      onSearchChange?.(localSearchTerm);
    }
  };

  const handleClearSearch = () => {
    setLocalSearchTerm('');
    onSearchChange?.('');
  };

  const activeFilterCount = Object.values(filters).filter(v => 
    v !== undefined && v !== null && v !== '' && 
    (Array.isArray(v) ? v.length > 0 : true)
  ).length;

  return (
    <div className="space-y-4">
      {/* Search and Filter Toggle Row */}
      <div className="flex flex-col md:flex-row gap-3">
        {/* Search Input */}
        {showSearch && (
          <form onSubmit={handleSearchSubmit} className="flex-1 relative">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
              <input
                type="text"
                value={localSearchTerm}
                onChange={handleSearchChange}
                onKeyDown={handleSearchKeyDown}
                placeholder={placeholder}
                className="w-full pl-10 pr-10 py-2.5 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent"
              />
              {localSearchTerm && (
                <button
                  type="button"
                  onClick={handleClearSearch}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                >
                  <X className="w-5 h-5" />
                </button>
              )}
            </div>
          </form>
        )}

        {/* Filter Toggle Button */}
        {showFilters && (
          <button
            onClick={() => setShowFilterPanel(!showFilterPanel)}
            className={`
              flex items-center justify-center gap-2 px-4 py-2.5 rounded-lg border transition-colors
              ${activeFilterCount > 0 
                ? 'bg-teal-50 border-teal-300 text-teal-700' 
                : 'border-gray-300 text-gray-700 hover:bg-gray-50'}
            `}
          >
            <Filter className="w-5 h-5" />
            <span>Bộ lọc</span>
            {activeFilterCount > 0 && (
              <span className="bg-teal-500 text-white text-xs px-2 py-0.5 rounded-full">
                {activeFilterCount}
              </span>
            )}
            <ChevronDown className={`w-4 h-4 transition-transform ${showFilterPanel ? 'rotate-180' : ''}`} />
          </button>
        )}
      </div>

      {/* Filter Panel */}
      {showFilters && showFilterPanel && (
        <div className="p-4 bg-gray-50 rounded-lg border border-gray-200">
          <div className="flex flex-wrap gap-4">
            {/* Render filter dropdowns based on filterOptions */}
            {Object.entries(filterOptions).map(([key, config]) => (
              <div key={key} className="min-w-[180px]">
                <label className="block text-sm font-medium text-gray-700 mb-1.5">
                  {config.label}
                </label>
                {config.type === 'select' ? (
                  <select
                    value={filters[key] ?? ''}
                    onChange={(e) => onFilterChange?.(key, e.target.value || undefined)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500"
                  >
                    <option value="">Tất cả</option>
                    {config.options?.map((opt) => (
                      <option key={opt.value} value={opt.value}>
                        {opt.label}
                      </option>
                    ))}
                  </select>
                ) : config.type === 'checkbox' ? (
                  <div className="flex flex-wrap gap-2 mt-1">
                    {config.options?.map((opt) => (
                      <label key={opt.value} className="flex items-center gap-2 cursor-pointer">
                        <input
                          type="checkbox"
                          checked={filters[key]?.includes(opt.value) || false}
                          onChange={(e) => {
                            const current = filters[key] || [];
                            const newValue = e.target.checked
                              ? [...current, opt.value]
                              : current.filter(v => v !== opt.value);
                            onFilterChange?.(key, newValue.length > 0 ? newValue : undefined);
                          }}
                          className="w-4 h-4 text-teal-500 rounded border-gray-300 focus:ring-teal-500"
                        />
                        <span className="text-sm text-gray-600">{opt.label}</span>
                      </label>
                    ))}
                  </div>
                ) : null}
              </div>
            ))}
          </div>

          {/* Clear Filters Button */}
          {activeFilterCount > 0 && (
            <div className="mt-4 pt-4 border-t border-gray-200 flex justify-end">
              <button
                onClick={() => {
                  Object.keys(filters).forEach(key => {
                    onFilterChange?.(key, undefined);
                  });
                }}
                className="text-sm text-red-600 hover:text-red-700"
              >
                Xóa tất cả bộ lọc
              </button>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default SearchAndFilter;
