import api from './api';

export const profileService = {
  // F334: Get current user profile
  getProfile: async () => {
    try {
      const response = await api.get('/profile');
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to get profile' };
    }
  },

  // F335, F338, F339, F343: Update profile
  updateProfile: async (profileData) => {
    try {
      const response = await api.put('/profile/update', profileData);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to update profile' };
    }
  },

  // F336: Upload avatar
  uploadAvatar: async (file) => {
    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await api.post('/profile/avatar', formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to upload avatar' };
    }
  },

  // F337: Change password
  changePassword: async (passwordData) => {
    try {
      const response = await api.post('/profile/change-password', passwordData);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to change password' };
    }
  },

  // F340: Get activity history
  getActivities: async (pageIndex = 1, pageSize = 20) => {
    try {
      const response = await api.get('/profile/activities', {
        params: { pageIndex, pageSize },
      });
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to get activities' };
    }
  },

  // F341: Get addresses
  getAddresses: async () => {
    try {
      const response = await api.get('/profile/addresses');
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to get addresses' };
    }
  },

  // F341: Create address
  createAddress: async (addressData) => {
    try {
      const response = await api.post('/profile/addresses', addressData);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to create address' };
    }
  },

  // F341: Update address
  updateAddress: async (addressData) => {
    try {
      const response = await api.put('/profile/addresses', addressData);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to update address' };
    }
  },

  // F341: Delete address
  deleteAddress: async (addressId) => {
    try {
      const response = await api.delete(`/profile/addresses/${addressId}`);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to delete address' };
    }
  },

  // F341: Set default address
  setDefaultAddress: async (addressId) => {
    try {
      const response = await api.put(`/profile/addresses/${addressId}/set-default`);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to set default address' };
    }
  },

  // F342: Resend verification email
  resendVerification: async () => {
    try {
      const response = await api.post('/profile/resend-verification');
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to resend verification' };
    }
  },
};

export default profileService;
