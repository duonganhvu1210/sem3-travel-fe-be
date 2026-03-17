import { Outlet, Link, useLocation } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { useAuth } from '@/context/AuthContext/AuthContext';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import {
  Plane,
  Search,
  Menu,
  X,
  ChevronDown,
  User,
  LogOut,
  Heart,
  MapPin,
  Palmtree,
  Bus,
  Building2,
  Utensils,
  Home,
  Info,
  Phone,
  Calendar,
  MessageCircle,
  Mail,
  Settings
} from 'lucide-react';

const MainLayout = () => {
  const location = useLocation();
  const { user, isAuthenticated, logout } = useAuth();
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [isScrolled, setIsScrolled] = useState(false);
  const [activeDropdown, setActiveDropdown] = useState(null);

  useEffect(() => {
    const handleScroll = () => setIsScrolled(window.scrollY > 20);
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  useEffect(() => {
    setIsMobileMenuOpen(false);
    setActiveDropdown(null);
  }, [location.pathname]);

  const navItems = [
    { name: 'Home', path: '/', icon: Home },
    { name: 'About', path: '/about', icon: Info },

    { name: 'Search', path: '/search', icon: Search },
    {
      name: 'Information',
      path: '/info',
      icon: Info,
      hasDropdown: true,
      dropdownItems: [
        { name: 'Tourist Spots', path: '/info/destinations', icon: MapPin },
        { name: 'Tours', path: '/info/tours', icon: Palmtree },
        { name: 'Transports', path: '/info/transports', icon: Bus },
        { name: 'Hotels', path: '/info/hotels', icon: Building2 },
        { name: 'Restaurants', path: '/info/restaurants', icon: Utensils },
        { name: 'Resorts', path: '/info/resorts', icon: Palmtree },
      ]
    },
    
    { name: 'Contact', path: '/contact', icon: Phone },
  ];

  const handleDropdownToggle = (index) => {
    setActiveDropdown(activeDropdown === index ? null : index);
  };

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      {/* Header */}
      <header
  className={`fixed top-0 left-0 right-0 z-50 transition-all duration-300 ${
    isScrolled
      ? 'bg-white/90 backdrop-blur-xl shadow-lg border-b border-gray-200/60 py-1'
      : 'bg-white/80 backdrop-blur-md py-2'
  }`}
>
        <div className="container mx-auto px-4">
          <div className="flex items-center justify-between">
            {/* Logo */}
            <Link to="/" className="flex items-center gap-2 group">
              <div className="w-10 h-10 bg-gradient-to-br from-teal-500 to-cyan-600 rounded-2xl flex items-center justify-center shadow-lg group-hover:scale-110 group-hover:rotate-3 transition-all duration-300">
                <Plane className="w-5 h-5 text-white" />
              </div>
              <span className={`font-bold text-xl text-gray-800`}>
                KarnelTravels
              </span>
            </Link>

            {/* Desktop Navigation */}
            <nav className="hidden lg:flex items-center gap-1">
              {navItems.map((item, index) => (
                <div key={item.path} className="relative">
                  {item.hasDropdown ? (
                    <button
                      onClick={() => handleDropdownToggle(index)}
                      className={`flex items-center gap-1.5 px-4 py-2.5 rounded-xl font-medium transition-all duration-300 text-gray-700 hover:bg-teal-50 hover:text-teal-600 ${
                        location.pathname === item.path ? 'bg-teal-500/15 text-teal-600 shadow-sm' : ''
                      }`}
                    >
                      {item.name}
                      <ChevronDown className={`w-4 h-4 transition-transform ${
                        activeDropdown === index ? 'rotate-180' : ''
                      }`} />
                    </button>
                  ) : (
                    <Link
                      to={item.path}
                      className={`flex items-center gap-1.5 px-4 py-2.5 rounded-lg font-medium transition-all text-gray-700 hover:bg-teal-50 hover:text-teal-600 ${location.pathname === item.path ? 'bg-teal-500/20 text-teal-600' : ''}`}
                    >
                      {item.name}
                    </Link>
                  )}

                  {/* Mega Menu Dropdown */}
                  {item.hasDropdown && activeDropdown === index && (
                    <div className="absolute top-full left-0 mt-2 w-[600px] bg-white rounded-2xl shadow-2xl border border-gray-100 overflow-hidden animate-in fade-in slide-in-from-top-2 duration-200">
                      <div className="grid grid-cols-2 gap-1 p-2">
                        {item.dropdownItems.map((dropdownItem) => (
                          <Link
                            key={dropdownItem.path}
                            to={dropdownItem.path}
                            className="flex items-center gap-3 p-4 rounded-xl hover:bg-teal-50 transition-colors group"
                          >
                            <div className="w-10 h-10 bg-gradient-to-br from-teal-100 to-cyan-100 rounded-lg flex items-center justify-center group-hover:scale-110 transition-transform">
                              <dropdownItem.icon className="w-5 h-5 text-teal-600" />
                            </div>
                            <div>
                              <p className="font-semibold text-gray-800">{dropdownItem.name}</p>
                              
                            </div>
                          </Link>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              ))}
            </nav>

            {/* Right Section */}
            <div className="flex items-center gap-3">
              {/* Search Button */}
              <Link
                to="/search"
                className="p-2.5 rounded-full bg-gray-100 text-gray-700 hover:bg-teal-50 hover:text-teal-600 transition-all"
              >
                <Search className="w-5 h-5" />
              </Link>

              {/* Admin Link */}
              <Link
                to="/admin"
                className="p-2.5 rounded-full bg-gray-100 text-gray-700 hover:bg-teal-50 hover:text-teal-600 transition-all"
                title="Admin Dashboard"
              >
                <Settings className="w-5 h-5" />
              </Link>

              {isAuthenticated ? (
                <div className="flex items-center gap-3">
                  {/* Wishlist */}
                  <Link
                    to="/wishlist"
                    className="p-2.5 rounded-full bg-gray-100 text-gray-700 hover:bg-teal-50 hover:text-teal-600 transition-all"
                  >
                    <Heart className="w-5 h-5" />
                  </Link>

                  {/* Bookings */}
                  <Link
                    to="/bookings"
                    className="p-2.5 rounded-full bg-gray-100 text-gray-700 hover:bg-teal-50 hover:text-teal-600 transition-all"
                  >
                    <Calendar className="w-5 h-5" />
                  </Link>

                  {/* User Menu */}
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" className="flex items-center gap-2 pl-2 pr-1 py-1.5 rounded-full bg-teal-500 hover:bg-teal-600 text-white">
                        <div className="w-8 h-8 bg-white/20 rounded-full flex items-center justify-center">
                          <User className="w-4 h-4" />
                        </div>
                        <span className="font-medium text-sm">{user?.fullName?.split(' ')[0]}</span>
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end" className="w-56">
                      <DropdownMenuLabel>
                        <div className="flex flex-col">
                          <span className="font-semibold">{user?.fullName}</span>
                          <span className="text-xs text-muted-foreground font-normal">{user?.email}</span>
                        </div>
                      </DropdownMenuLabel>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem asChild>
                        <Link to="/profile" className="flex items-center">
                          <User className="mr-2 h-4 w-4" />
                          Profile
                        </Link>
                      </DropdownMenuItem>
                      <DropdownMenuItem asChild>
                        <Link to="/bookings" className="flex items-center">
                          <Calendar className="mr-2 h-4 w-4" />
                          Bookings
                        </Link>
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem onClick={logout} className="text-red-600 cursor-pointer">
                        <LogOut className="mr-2 h-4 w-4" />
                        Logout
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </div>
              ) : (
                <div className="flex items-center gap-2">
                  <Link
                    to="/login"
                    className="px-5 py-2.5 rounded-full font-medium transition-all text-gray-700 hover:bg-gray-100 hover:text-teal-600"
                  >
                    Login
                  </Link>
                  <Link
                    to="/register"
                    className="px-5 py-2.5 bg-gradient-to-r from-teal-500 to-cyan-600 text-white font-medium rounded-full transition-all hover:shadow-lg hover:shadow-teal-500/30 hover:-translate-y-0.5"
                  >
                    Register
                  </Link>
                </div>
              )}

              {/* Mobile Menu Toggle */}
              <button
                onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                className="lg:hidden p-2.5 rounded-lg bg-gray-100 text-gray-700"
              >
                {isMobileMenuOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
              </button>
            </div>
          </div>
        </div>

        {/* Mobile Menu */}
        <div
          className={`lg:hidden absolute top-full left-0 right-0 bg-white shadow-2xl transition-all duration-300 ${
            isMobileMenuOpen ? 'opacity-100 visible' : 'opacity-0 invisible'
          }`}
        >
          <nav className="container mx-auto px-4 py-4 space-y-2">
            {navItems.map((item) => (
              <div key={item.path}>
                {item.hasDropdown ? (
                  <>
                    <button
                      onClick={() => handleDropdownToggle(navItems.indexOf(item))}
                      className="w-full flex items-center justify-between px-4 py-3 rounded-xl text-gray-700 hover:bg-teal-50"
                    >
                      <span className="flex items-center gap-3 font-medium">
                        <item.icon className="w-5 h-5" />
                        {item.name}
                      </span>
                      <ChevronDown className={`w-5 h-5 transition-transform ${
                        activeDropdown === navItems.indexOf(item) ? 'rotate-180' : ''
                      }`} />
                    </button>
                    {activeDropdown === navItems.indexOf(item) && (
                      <div className="pl-6 mt-2 space-y-1">
                        {item.dropdownItems.map((dropdownItem) => (
                          <Link
                            key={dropdownItem.path}
                            to={dropdownItem.path}
                            className="flex items-center gap-3 px-4 py-3 rounded-xl text-gray-600 hover:bg-teal-50"
                          >
                            <dropdownItem.icon className="w-5 h-5" />
                            {dropdownItem.name}
                          </Link>
                        ))}
                      </div>
                    )}
                  </>
                ) : (
                  <Link
                    to={item.path}
                    className={`flex items-center gap-3 px-4 py-3 rounded-xl font-medium ${
                      location.pathname === item.path
                        ? 'bg-teal-50 text-teal-600'
                        : 'text-gray-700 hover:bg-teal-50'
                    }`}
                  >
                    <item.icon className="w-5 h-5" />
                    {item.name}
                  </Link>
                )}
              </div>
            ))}
          </nav>
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1 pt-16">
        <Outlet />
      </main>

      {/* Floating Phone Button */}
      <a
        href="tel:19006677"
        className="fixed bottom-6 right-6 z-50 w-14 h-14 bg-green-500 hover:bg-green-600 text-white rounded-full flex items-center justify-center shadow-lg hover:shadow-xl transition-all transform hover:scale-110 animate-bounce"
        aria-label="Gọi hotline"
      >
        <Phone className="w-6 h-6" />
      </a>

            {/* Footer */}
            <footer className="bg-gray-950 text-white relative overflow-hidden">
        {/* Background effect */}
        <div className="absolute inset-0 pointer-events-none">
          <div className="absolute -top-10 -left-10 w-72 h-72 bg-teal-500/10 rounded-full blur-3xl" />
          <div className="absolute -bottom-10 -right-10 w-72 h-72 bg-cyan-500/10 rounded-full blur-3xl" />
        </div>

        <div className="relative container mx-auto px-4 py-16">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-10">
            {/* Brand */}
            <div>
              <div className="flex items-center gap-3 mb-6">
                <div className="w-11 h-11 bg-gradient-to-br from-teal-500 to-cyan-600 rounded-2xl flex items-center justify-center shadow-lg">
                  <Plane className="w-5 h-5 text-white" />
                </div>
                <div>
                  <span className="block font-bold text-xl">KarnelTravels</span>
                  <span className="text-sm text-gray-400">Travel with confidence</span>
                </div>
              </div>

              <p className="text-gray-400 mb-6 leading-7">
                Your trusted partner for exploring Vietnam and the world with
                unforgettable journeys, top destinations, and premium travel services.
              </p>

              <div className="flex items-center gap-3">
                <a
                  href="#"
                  className="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center hover:bg-teal-500 hover:border-teal-500 transition-all"
                >
                  <MessageCircle className="w-5 h-5" />
                </a>
                <a
                  href="#"
                  className="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center hover:bg-teal-500 hover:border-teal-500 transition-all"
                >
                  <Mail className="w-5 h-5" />
                </a>
                <a
                  href="#"
                  className="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center hover:bg-teal-500 hover:border-teal-500 transition-all"
                >
                  <Phone className="w-5 h-5" />
                </a>
              </div>
            </div>

            {/* Quick Links */}
            <div>
              <h4 className="font-semibold text-lg mb-6 text-white">Quick Links</h4>
              <ul className="space-y-3">
                {[
                  { name: 'Home', path: '/' },
                  { name: 'About', path: '/about' },
                  { name: 'Search', path: '/search' },
                  { name: 'Contact', path: '/contact' }
                ].map((item) => (
                  <li key={item.name}>
                    <Link
                      to={item.path}
                      className="text-gray-400 hover:text-white transition-all duration-200 hover:pl-1 inline-block"
                    >
                      {item.name}
                    </Link>
                  </li>
                ))}
              </ul>
            </div>

            {/* Services */}
            <div>
              <h4 className="font-semibold text-lg mb-6 text-white">Services</h4>
              <ul className="space-y-3">
                {[
                  { name: 'Tours', path: '/info/tours' },
                  { name: 'Hotels', path: '/info/hotels' },
                  { name: 'Restaurants', path: '/info/restaurants' },
                  { name: 'Resorts', path: '/info/resorts' },
                  { name: 'Transports', path: '/info/transports' }
                ].map((item) => (
                  <li key={item.name}>
                    <Link
                      to={item.path}
                      className="text-gray-400 hover:text-white transition-all duration-200 hover:pl-1 inline-block"
                    >
                      {item.name}
                    </Link>
                  </li>
                ))}
              </ul>
            </div>

            {/* Contact + Map */}
            <div>
              <h4 className="font-semibold text-lg mb-6 text-white">Contact</h4>

              <div className="space-y-4 mb-6">
                <div className="flex items-start gap-3 p-3 rounded-2xl bg-white/5 border border-white/10">
                  <MapPin className="w-5 h-5 mt-0.5 text-teal-400" />
                  <span className="text-gray-300 text-sm leading-6">
                    123 Nguyen Trai Street, District 1, Ho Chi Minh City
                  </span>
                </div>

                <div className="flex items-center gap-3 p-3 rounded-2xl bg-white/5 border border-white/10">
                  <Phone className="w-5 h-5 text-teal-400" />
                  <span className="text-gray-300 text-sm">+84 28 1234 5678</span>
                </div>

                <div className="flex items-center gap-3 p-3 rounded-2xl bg-white/5 border border-white/10">
                  <Mail className="w-5 h-5 text-teal-400" />
                  <span className="text-gray-300 text-sm">info@karneltravels.com</span>
                </div>
              </div>

              <div className="overflow-hidden rounded-2xl border border-white/10 shadow-lg">
                <iframe
                  title="KarnelTravels Location"
                  src="https://www.google.com/maps?q=District+1,+Ho+Chi+Minh+City&z=14&output=embed"
                  width="100%"
                  height="180"
                  style={{ border: 0 }}
                  allowFullScreen=""
                  loading="lazy"
                  referrerPolicy="no-referrer-when-downgrade"
                />
              </div>
            </div>
          </div>

          <div className="border-t border-white/10 mt-12 pt-8 flex flex-col md:flex-row items-center justify-between gap-4 text-gray-500 text-sm">
            <p>&copy; 2026 KarnelTravels. All rights reserved.</p>

            <div className="flex items-center gap-5">
              <Link to="/" className="hover:text-white transition-colors">
                Privacy
              </Link>
              <Link to="/" className="hover:text-white transition-colors">
                Terms
              </Link>
              <Link to="/contact" className="hover:text-white transition-colors">
                Support
              </Link>
            </div>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default MainLayout;
