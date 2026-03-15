import React, { useState, useEffect } from 'react';
import orderService from '../../../services/orderService';
import OrderCard from './components/OrderCard';
import OrderDetailModal from './components/OrderDetailModal';

// Tab configuration
const TABS = [
  { id: 'all', label: 'Tất cả', status: null },
  { id: 'pending', label: 'Chờ xác nhận', status: 1 },
  { id: 'confirmed', label: 'Đã xác nhận', status: 2 },
  { id: 'completed', label: 'Đã hoàn thành', status: 3 },
  { id: 'cancelled', label: 'Đã hủy', status: 4 }
];

// Service type options
const SERVICE_TYPES = [
  { value: '', label: 'Tất cả loại dịch vụ' },
  { value: 1, label: 'Tour' },
  { value: 2, label: 'Khách sạn' },
  { value: 3, label: 'Resort' },
  { value: 4, label: 'Xe' },
  { value: 5, label: 'Nhà hàng' }
];

const MyOrdersPage = () => {
  // State
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [pagination, setPagination] = useState({
    pageNumber: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0
  });
  
  // Filters
  const [activeTab, setActiveTab] = useState('all');
  const [searchQuery, setSearchQuery] = useState('');
  const [serviceType, setServiceType] = useState('');
  
  // Modal
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  
  // Statistics
  const [stats, setStats] = useState(null);

  // Load orders
  useEffect(() => {
    loadOrders();
  }, [pagination.pageNumber, activeTab, serviceType]);

  // Load statistics
  useEffect(() => {
    loadStatistics();
  }, []);

  const loadOrders = async () => {
    setLoading(true);
    setError(null);
    try {
      const currentTab = TABS.find(t => t.id === activeTab);
      const response = await orderService.getOrders({
        page: pagination.pageNumber,
        pageSize: pagination.pageSize,
        searchQuery: searchQuery,
        status: currentTab?.status,
        serviceType: serviceType || null
      });
      
      setOrders(response.orders || []);
      setPagination(prev => ({
        ...prev,
        totalCount: response.totalCount || 0,
        totalPages: response.totalPages || 0
      }));
    } catch (err) {
      setError('Không thể tải danh sách đơn hàng');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const loadStatistics = async () => {
    try {
      const data = await orderService.getStatistics();
      setStats(data);
    } catch (err) {
      console.error('Failed to load statistics:', err);
    }
  };

  const handleSearch = (e) => {
    e.preventDefault();
    setPagination(prev => ({ ...prev, pageNumber: 1 }));
    loadOrders();
  };

  const handlePageChange = (newPage) => {
    if (newPage >= 1 && newPage <= pagination.totalPages) {
      setPagination(prev => ({ ...prev, pageNumber: newPage }));
    }
  };

  const handleOrderClick = (order) => {
    setSelectedOrder(order);
    setIsModalOpen(true);
  };

  const handleCancelOrder = async (order) => {
    if (!window.confirm('Bạn có chắc chắn muốn hủy đơn hàng này?')) return;
    
    try {
      const result = await orderService.cancelOrder(order.orderId, 'Khách hàng hủy');
      if (result.success) {
        alert('Đơn hàng đã được hủy thành công');
        loadOrders();
        loadStatistics();
      } else {
        alert(result.message || 'Không thể hủy đơn hàng');
      }
    } catch (err) {
      alert('Lỗi khi hủy đơn hàng');
    }
  };

  const handleChangeDate = async (order) => {
    const newDate = prompt('Nhập ngày mới (YYYY-MM-DD):');
    if (!newDate) return;
    
    try {
      const result = await orderService.changeDate(order.orderId, newDate, null, 'Khách yêu cầu đổi ngày');
      if (result.success) {
        alert('Yêu cầu đổi ngày đã được gửi');
        loadOrders();
      } else {
        alert(result.message || 'Không thể đổi ngày');
      }
    } catch (err) {
      alert('Lỗi khi đổi ngày');
    }
  };

  const handleReview = (order) => {
    // Navigate to review page or open review modal
    console.log('Review order:', order);
    alert('Chức năng đánh giá sẽ được phát triển');
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="container mx-auto px-4">
        {/* Page Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-800 mb-2">Đơn đã đặt</h1>
          <p className="text-gray-600">Quản lý và theo dõi các đơn hàng của bạn</p>
        </div>

        {/* Statistics Cards */}
        {stats && (
          <div className="grid grid-cols-2 md:grid-cols-5 gap-4 mb-8">
            <div className="bg-white rounded-lg shadow p-4">
              <p className="text-sm text-gray-500">Tổng đơn</p>
              <p className="text-2xl font-bold text-gray-800">{stats.totalOrders}</p>
            </div>
            <div className="bg-yellow-50 rounded-lg shadow p-4">
              <p className="text-sm text-yellow-600">Chờ xác nhận</p>
              <p className="text-2xl font-bold text-yellow-600">{stats.pendingOrders}</p>
            </div>
            <div className="bg-blue-50 rounded-lg shadow p-4">
              <p className="text-sm text-blue-600">Đã xác nhận</p>
              <p className="text-2xl font-bold text-blue-600">{stats.confirmedOrders}</p>
            </div>
            <div className="bg-green-50 rounded-lg shadow p-4">
              <p className="text-sm text-green-600">Hoàn thành</p>
              <p className="text-2xl font-bold text-green-600">{stats.completedOrders}</p>
            </div>
            <div className="bg-gray-50 rounded-lg shadow p-4">
              <p className="text-sm text-gray-500">Tổng chi tiêu</p>
              <p className="text-2xl font-bold text-gray-800">
                {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(stats.totalSpent || 0)}
              </p>
            </div>
          </div>
        )}

        {/* Filters */}
        <div className="bg-white rounded-lg shadow mb-6 p-4">
          <div className="flex flex-col md:flex-row gap-4">
            {/* Search */}
            <form onSubmit={handleSearch} className="flex-1 flex gap-2">
              <input
                type="text"
                placeholder="Tìm theo mã đơn..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <button
                type="submit"
                className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
              >
                Tìm kiếm
              </button>
            </form>
            
            {/* Service Type Filter */}
            <select
              value={serviceType}
              onChange={(e) => setServiceType(e.target.value)}
              className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              {SERVICE_TYPES.map(type => (
                <option key={type.value} value={type.value}>
                  {type.label}
                </option>
              ))}
            </select>
          </div>
        </div>

        {/* Tabs */}
        <div className="flex flex-wrap gap-2 mb-6">
          {TABS.map(tab => (
            <button
              key={tab.id}
              onClick={() => {
                setActiveTab(tab.id);
                setPagination(prev => ({ ...prev, pageNumber: 1 }));
              }}
              className={`px-4 py-2 rounded-lg font-medium transition-colors ${
                activeTab === tab.id
                  ? 'bg-blue-600 text-white'
                  : 'bg-white text-gray-600 hover:bg-gray-100'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>

        {/* Error Message */}
        {error && (
          <div className="bg-red-50 text-red-600 p-4 rounded-lg mb-4">
            {error}
          </div>
        )}

        {/* Loading */}
        {loading && (
          <div className="flex justify-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
          </div>
        )}

        {/* Orders List */}
        {!loading && (
          <>
            {orders.length === 0 ? (
              <div className="text-center py-12">
                <div className="text-6xl mb-4">📦</div>
                <h3 className="text-xl font-semibold text-gray-600 mb-2">Chưa có đơn hàng</h3>
                <p className="text-gray-500">Bạn chưa có đơn hàng nào. Hãy bắt đầu đặt dịch vụ ngay!</p>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {orders.map(order => (
                  <OrderCard
                    key={order.orderId}
                    order={order}
                    onClick={handleOrderClick}
                    onCancel={handleCancelOrder}
                    onChangeDate={handleChangeDate}
                  />
                ))}
              </div>
            )}

            {/* Pagination */}
            {orders.length > 0 && pagination.totalPages > 1 && (
              <div className="flex justify-center items-center gap-2 mt-8">
                <button
                  onClick={() => handlePageChange(pagination.pageNumber - 1)}
                  disabled={pagination.pageNumber === 1}
                  className="px-4 py-2 rounded-lg bg-white border border-gray-300 disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-100"
                >
                  ← Trước
                </button>
                
                <span className="px-4 py-2 text-gray-600">
                  Trang {pagination.pageNumber} / {pagination.totalPages}
                </span>
                
                <button
                  onClick={() => handlePageChange(pagination.pageNumber + 1)}
                  disabled={pagination.pageNumber === pagination.totalPages}
                  className="px-4 py-2 rounded-lg bg-white border border-gray-300 disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-100"
                >
                  Sau →
                </button>
              </div>
            )}
          </>
        )}

        {/* Order Detail Modal */}
        <OrderDetailModal
          orderId={selectedOrder?.orderId}
          isOpen={isModalOpen}
          onClose={() => {
            setIsModalOpen(false);
            setSelectedOrder(null);
          }}
          onReview={handleReview}
          onDownloadInvoice={(orderId) => orderService.downloadInvoice(orderId)}
        />
      </div>
    </div>
  );
};

export default MyOrdersPage;
