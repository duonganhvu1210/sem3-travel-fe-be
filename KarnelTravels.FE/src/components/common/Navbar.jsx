import { useState, useEffect, useRef } from 'react';
import { Link } from 'react-router-dom';
import { 
  Menu, 
  X, 
  ChevronDown, 
  MapPin, 
  Plane, 
  Bus, 
  Building2, 
  Utensils, 
  Palmtree,
  Search,
  Compass
} from 'lucide-react';

const navItems = [
  { name: 'Trang chủ', path: '/' },
  
  { name: 'Tìm kiếm', path: '/search' },
  { 
    name: 'Thông tin', 
    path: '/info',
    hasDropdown: true,
    dropdownItems: [
      { name: 'Điểm du lịch', path: '/info/destinations', icon: Compass },
      { name: 'Tour Du lịch', path: '/info/tours', icon: Palmtree },
      { name: 'Phương tiện', path: '/info/transport', icon: Bus },
      { name: 'Thông tin Khách sạn', path: '/info/hotels', icon: Building2 },
      { name: 'Thông tin Nhà hàng', path: '/info/restaurants', icon: Utensils },
      { name: 'Thông tin Resort', path: '/info/resorts', icon: Palmtree },
    ]
  },
  { name: 'Liên hệ', path: '/contact' },
];

const Navbar = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [activeDropdown, setActiveDropdown] = useState(null);
  const [isScrolled, setIsScrolled] = useState(false);
  const dropdownRef = useRef(null);

  useEffect(() => {
    const handleScroll = () => {
      setIsScrolled(window.scrollY > 20);
    };
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setActiveDropdown(null);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleDropdownToggle = (index) => {
    setActiveDropdown(activeDropdown === index ? null : index);
  };

  return (
    <nav 
      ref={dropdownRef}
      className={`fixed top-0 left-0 right-0 z-50 transition-all duration-300 ${
        isScrolled 
          ? 'bg-white/80 backdrop-blur-md shadow-lg border-b border-gray-100' 
          : 'bg-transparent'
      }`}
    >
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16 lg:h-20">
          {/* Logo */}
          <Link to="/" className="flex items-center space-x-2 group">
            <div className="w-10 h-10 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-xl flex items-center justify-center shadow-lg group-hover:shadow-indigo-500/30 transition-shadow">
              <Plane className="w-6 h-6 text-white" />
            </div>
            <span className="text-xl font-bold bg-gradient-to-r from-indigo-600 to-purple-600 bg-clip-text text-transparent">
              KarnelTravels
            </span>
          </Link>

          {/* Desktop Navigation */}
          <div className="hidden lg:flex items-center space-x-1">
            {navItems.map((item, index) => (
              <div key={item.name} className="relative">
                {item.hasDropdown ? (
                  <button
                    onClick={() => handleDropdownToggle(index)}
                    className={`flex items-center space-x-1 px-4 py-2 rounded-lg text-gray-700 font-medium transition-all duration-200 hover:text-indigo-600 hover:bg-indigo-50 ${
                      activeDropdown === index ? 'text-indigo-600 bg-indigo-50' : ''
                    }`}
                  >
                    <span>{item.name}</span>
                    <ChevronDown 
                      className={`w-4 h-4 transition-transform duration-200 ${
                        activeDropdown === index ? 'rotate-180' : ''
                      }`} 
                    />
                  </button>
                ) : (
                  <Link
                    to={item.path}
                    className="relative px-4 py-2 rounded-lg text-gray-700 font-medium transition-all duration-200 hover:text-indigo-600 hover:bg-indigo-50 group"
                  >
                    <span>{item.name}</span>
                    <span className="absolute bottom-1 left-4 right-4 h-0.5 bg-indigo-600 transform scale-x-0 group-hover:scale-x-100 transition-transform duration-200 origin-left" />
                  </Link>
                )}

                {/* Dropdown Menu */}
                {item.hasDropdown && activeDropdown === index && (
                  <div className="absolute top-full left-0 mt-2 w-64 bg-white rounded-xl shadow-xl border border-gray-100 py-2 animate-fadeIn">
                    {item.dropdownItems.map((dropdownItem, idx) => (
                      <Link
                        key={idx}
                        to={dropdownItem.path}
                        className="flex items-center space-x-3 px-4 py-3 text-gray-700 hover:bg-indigo-50 hover:text-indigo-600 transition-colors duration-150"
                        onClick={() => setActiveDropdown(null)}
                      >
                        {dropdownItem.icon && (
                          <dropdownItem.icon className="w-5 h-5 text-indigo-500" />
                        )}
                        <span className="font-medium">{dropdownItem.name}</span>
                      </Link>
                    ))}
                  </div>
                )}
              </div>
            ))}
          </div>

          {/* Search Button - Desktop */}
          <div className="hidden lg:flex items-center space-x-3">
            <Link
              to="/search"
              className="p-2.5 rounded-lg text-gray-600 hover:text-indigo-600 hover:bg-indigo-50 transition-all duration-200"
            >
              <Search className="w-5 h-5" />
            </Link>
            <Link
              to="/login"
              className="px-5 py-2.5 bg-indigo-600 text-white font-medium rounded-lg hover:bg-indigo-700 transition-colors duration-200 shadow-md hover:shadow-lg"
            >
              Đăng nhập
            </Link>
          </div>

          {/* Mobile Menu Button */}
          <button
            onClick={() => setIsOpen(!isOpen)}
            className="lg:hidden p-2 rounded-lg text-gray-700 hover:bg-gray-100 transition-colors"
          >
            {isOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
          </button>
        </div>
      </div>

      {/* Mobile Menu */}
      <div 
        className={`lg:hidden overflow-hidden transition-all duration-300 ${
          isOpen ? 'max-h-96 border-t border-gray-100' : 'max-h-0'
        }`}
      >
        <div className="bg-white/95 backdrop-blur-md px-4 py-4 space-y-2">
          {navItems.map((item, index) => (
            <div key={item.name}>
              {item.hasDropdown ? (
                <>
                  <button
                    onClick={() => handleDropdownToggle(index)}
                    className="flex items-center justify-between w-full px-4 py-3 text-gray-700 font-medium rounded-lg hover:bg-indigo-50 transition-colors"
                  >
                    <span>{item.name}</span>
                    <ChevronDown 
                      className={`w-4 h-4 transition-transform duration-200 ${
                        activeDropdown === index ? 'rotate-180' : ''
                      }`} 
                    />
                  </button>
                  {activeDropdown === index && (
                    <div className="ml-4 mt-1 space-y-1 bg-gray-50 rounded-lg p-2">
                      {item.dropdownItems.map((dropdownItem, idx) => (
                        <Link
                          key={idx}
                          to={dropdownItem.path}
                          className="flex items-center space-x-3 px-3 py-2.5 text-gray-600 rounded-lg hover:bg-indigo-100 hover:text-indigo-600 transition-colors"
                          onClick={() => {
                            setActiveDropdown(null);
                            setIsOpen(false);
                          }}
                        >
                          {dropdownItem.icon && (
                            <dropdownItem.icon className="w-4 h-4 text-indigo-500" />
                          )}
                          <span className="text-sm">{dropdownItem.name}</span>
                        </Link>
                      ))}
                    </div>
                  )}
                </>
              ) : (
                <Link
                  to={item.path}
                  className="block px-4 py-3 text-gray-700 font-medium rounded-lg hover:bg-indigo-50 hover:text-indigo-600 transition-colors"
                  onClick={() => setIsOpen(false)}
                >
                  {item.name}
                </Link>
              )}
            </div>
          ))}
          <div className="pt-2 border-t border-gray-200 mt-2">
            <Link
              to="/login"
              className="block w-full px-4 py-3 bg-indigo-600 text-white font-medium text-center rounded-lg hover:bg-indigo-700 transition-colors"
              onClick={() => setIsOpen(false)}
            >
              Đăng nhập
            </Link>
          </div>
        </div>
      </div>

      <style>{`
        @keyframes fadeIn {
          from {
            opacity: 0;
            transform: translateY(-10px);
          }
          to {
            opacity: 1;
            transform: translateY(0);
          }
        }
        .animate-fadeIn {
          animation: fadeIn 0.2s ease-out forwards;
        }
      `}</style>
    </nav>
  );
};

export default Navbar;
