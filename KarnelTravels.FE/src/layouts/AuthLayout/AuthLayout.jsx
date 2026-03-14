import { Outlet, Link } from 'react-router-dom';
import { Plane } from 'lucide-react';

const AuthLayout = () => {
  return (
    <div className="min-h-screen flex items-center justify-center relative overflow-hidden bg-gradient-to-br from-teal-600 via-cyan-600 to-blue-700">
      {/* Animated Background Shapes */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <div className="absolute -top-40 -right-40 w-80 h-80 bg-white/10 rounded-full blur-3xl animate-pulse"></div>
        <div className="absolute -bottom-40 -left-40 w-80 h-80 bg-white/10 rounded-full blur-3xl animate-pulse delay-1000"></div>
        <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-96 h-96 bg-cyan-300/20 rounded-full blur-3xl"></div>
      </div>

      {/* Floating Shapes */}
      <div className="absolute top-20 left-20 w-4 h-4 bg-white/30 rounded-full animate-bounce"></div>
      <div className="absolute bottom-20 right-20 w-6 h-6 bg-white/20 rounded-full animate-bounce delay-300"></div>
      <div className="absolute top-1/3 right-1/4 w-3 h-3 bg-white/40 rounded-full animate-bounce delay-500"></div>
      <div className="absolute bottom-1/3 left-1/4 w-5 h-5 bg-white/20 rounded-full animate-bounce delay-700"></div>

      {/* Back to Home Button */}
      <Link
        to="/"
        className="absolute top-6 left-6 flex items-center gap-2 px-4 py-2 bg-white/10 backdrop-blur-sm text-white/90 hover:text-white hover:bg-white/20 rounded-full transition-all"
      >
        <Plane className="w-4 h-4" />
        <span className="font-medium">Về trang chủ</span>
      </Link>

      {/* Auth Card */}
      <div className="relative z-10 w-full max-w-md mx-4">
        <Outlet />
      </div>
    </div>
  );
};

export default AuthLayout;
