import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '@/context/AuthContext/AuthContext';
import { Loader2 } from 'lucide-react';

// ============ ADMIN ROUTE ============
// Bảo vệ route chỉ cho phép Admin truy cập
const AdminRoute = ({ children }) => {
  const { isAuthenticated, isAdmin, isLoading } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-50 via-white to-purple-50">
        <div className="flex flex-col items-center gap-4">
          <Loader2 className="w-10 h-10 animate-spin text-indigo-600" />
          <p className="text-gray-600 font-medium">Đang tải...</p>
        </div>
      </div>
    );
  }

  // Chưa đăng nhập -> Chuyển về login
  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // Đăng nhập nhưng không phải Admin -> Chuyển về trang 403
  if (!isAdmin) {
    return <Navigate to="/403" replace />;
  }

  // Là Admin -> Cho phép truy cập
  return children;
};

export default AdminRoute;
