import { Navigate, Routes, Route } from 'react-router-dom';

// Layouts
import MainLayout from '@/layouts/MainLayout/MainLayout';
import AdminLayout from '@/layouts/AdminLayout/AdminLayout';
import AuthLayout from '@/layouts/AuthLayout/AuthLayout';

// Auth Pages
import LoginPage from '@/pages/auth/LoginPage/LoginPage';
import RegisterPage from '@/pages/auth/RegisterPage/RegisterPage';
import ForbiddenPage from '@/pages/error/ForbiddenPage/ForbiddenPage';

// Pages
import HomePage from '@/pages/home/HomePage/HomePage';
import InformationPage from '@/pages/info/InformationPage/InformationPage';
import TouristSpotsPage from '@/pages/info/TouristSpotsPage/TouristSpotsPage';
import TouristSpotDetailPage from '@/pages/info/TouristSpotDetailPage/TouristSpotDetailPage';

// Admin Dashboard
import DashboardPage from '@/pages/admin/dashboard/DashboardPage';

const AboutPage = () => (
  <div className="min-h-screen flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Giới thiệu</h1>
  </div>
);

const SearchPage = () => (
  <div className="min-h-screen flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Tìm kiếm</h1>
  </div>
);

const ContactPage = () => (
  <div className="min-h-screen flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Liên hệ</h1>
  </div>
);

const ProfilePage = () => (
  <div className="min-h-screen flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Hồ sơ</h1>
  </div>
);

const WishlistPage = () => (
  <div className="min-h-screen flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Danh sách yêu thích</h1>
  </div>
);

const DestinationsPage = () => <TouristSpotsPage />;

const HotelsPage = () => (
  <div className="min-h-screen flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Khách sạn</h1>
  </div>
);

const RestaurantsPage = () => (
  <div className="min-h-screen flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Nhà hàng</h1>
  </div>
);

const ResortsPage = () => (
  <div className="min-h-screen flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Resort</h1>
  </div>
);

// Admin Pages
import AdminHotelsPage from '@/pages/admin/hotels/HotelsPage';
import AdminRestaurantsPage from '@/pages/admin/restaurants/RestaurantsPage';
import AdminResortsPage from '@/pages/admin/resorts/ResortsPage';
import RoomDetailsPage from '@/pages/admin/hotels/RoomDetailsPage';
import BookingsPage from '@/pages/admin/bookings/BookingsPage';
import PromotionsPage from '@/pages/admin/promotions/PromotionsPage';
import ContactsPage from '@/pages/admin/contacts/ContactsPage';
import UsersManagement from '@/pages/admin/users/UsersManagement';
import TransportsPage from '@/pages/admin/transports/TransportsPage';
import ToursPage from '@/pages/admin/tours/ToursPage';

const AdminDashboard = () => <DashboardPage />;

const AdminUsers = () => <UsersManagement />;

const AdminBookings = () => <BookingsPage />;

const AdminDestinations = () => <TouristSpotsPage />;

const AdminTours = () => <ToursPage />;

const AdminHotels = () => <AdminHotelsPage />;

const AdminRestaurants = () => <AdminRestaurantsPage />;

const AdminResorts = () => <AdminResortsPage />;

const AdminTransports = () => <TransportsPage />;

import ReportsPage from '@/pages/admin/reports/ReportsPage';

const AdminReports = () => <ReportsPage />;

const AdminSettings = () => (
  <div className="min-h-screen flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Cài đặt</h1>
  </div>
);

const AppRoutes = () => {
  return (
    <Routes>
      {/* ==================== AUTH LAYOUT (Public - No Protection) ==================== */}
      <Route element={<AuthLayout />}>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
      </Route>

      {/* ==================== MAIN LAYOUT (User - Protected) ==================== */}
      <Route path="/" element={<MainLayout />}>
        <Route index element={<HomePage />} />
        <Route path="info" element={<InformationPage />} />
        <Route path="about" element={<AboutPage />} />
        <Route path="search" element={<SearchPage />} />
        <Route path="contact" element={<ContactPage />} />
        <Route path="profile" element={<ProfilePage />} />
        <Route path="bookings" element={<BookingsPage />} />
        <Route path="promotions" element={<PromotionsPage />} />
        <Route path="contacts" element={<ContactsPage />} />
        <Route path="wishlist" element={<WishlistPage />} />
        <Route path="info/destinations" element={<DestinationsPage />} />
        <Route path="info/destinations/:id" element={<TouristSpotDetailPage />} />
        <Route path="info/tours" element={<ToursPage />} />
        <Route path="info/hotels" element={<HotelsPage />} />
        <Route path="info/restaurants" element={<RestaurantsPage />} />
        <Route path="info/resorts" element={<ResortsPage />} />
        <Route path="info/transports" element={<TransportsPage />} />
      </Route>

      {/* ==================== ADMIN LAYOUT (Admin Only) ==================== */}
      <Route path="/admin" element={<AdminLayout />}>
        <Route index element={<AdminDashboard />} />
        <Route path="users" element={<AdminUsers />} />
        <Route path="bookings" element={<AdminBookings />} />
        <Route path="destinations" element={<AdminDestinations />} />
        <Route path="tours" element={<AdminTours />} />
        <Route path="hotels" element={<AdminHotels />} />
        <Route path="hotels/:hotelId/rooms" element={<RoomDetailsPage />} />
        <Route path="promotions" element={<PromotionsPage />} />
        <Route path="contacts" element={<ContactsPage />} />
        <Route path="restaurants" element={<AdminRestaurants />} />
        <Route path="resorts" element={<AdminResorts />} />
        <Route path="transports" element={<AdminTransports />} />
        <Route path="reports" element={<AdminReports />} />
        <Route path="settings" element={<AdminSettings />} />
      </Route>

      {/* ==================== ERROR PAGES ==================== */}
      <Route path="/403" element={<ForbiddenPage />} />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};

export default AppRoutes;
