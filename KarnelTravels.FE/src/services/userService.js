import api from './api';

const API_URL = '/admin/users';

export const userService = {
  // F219: Lấy danh sách người dùng với phân trang và tìm kiếm
  getUsers: async (params = {}) => {
    const response = await api.get(API_URL, { params });
    return response.data;
  },

  // Lấy thông tin một người dùng
  getUserById: async (id) => {
    const response = await api.get(`${API_URL}/${id}`);
    return response.data;
  },

  // F220: Tạo mới người dùng
  createUser: async (userData) => {
    const response = await api.post(API_URL, userData);
    return response.data;
  },

  // F221: Cập nhật thông tin người dùng
  updateUser: async (id, userData) => {
    const response = await api.put(`${API_URL}/${id}`, userData);
    return response.data;
  },

  // F222: Xóa người dùng
  deleteUser: async (id) => {
    const response = await api.delete(`${API_URL}/${id}`);
    return response.data;
  },

  // F225: Kích hoạt/Vô hiệu hóa tài khoản
  updateUserStatus: async (id, isLocked) => {
    const response = await api.patch(`${API_URL}/${id}/status`, { isLocked });
    return response.data;
  },

  // F224: Đặt lại mật khẩu
  resetPassword: async (id, newPassword) => {
    const response = await api.post(`${API_URL}/${id}/reset-password`, { newPassword });
    return response.data;
  },

  // F226: Lấy danh sách đơn đặt của người dùng
  getUserBookings: async (id) => {
    const response = await api.get(`${API_URL}/${id}/bookings`);
    return response.data;
  },
};

export default userService;
