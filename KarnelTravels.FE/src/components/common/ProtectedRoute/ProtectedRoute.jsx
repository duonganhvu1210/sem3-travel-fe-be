import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '@/context/AuthContext/AuthContext';
import { Loader2 } from 'lucide-react';

// ============ PROTECTED ROUTE ============
// Bảo vệ route cho user đã đăng nhập
const ProtectedRoute = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuth();
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

  if (!isAuthenticated) {
    // Lưu lại vị trí hiện tại để sau khi login sẽ quay lại
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return children;
};

export default ProtectedRoute;
