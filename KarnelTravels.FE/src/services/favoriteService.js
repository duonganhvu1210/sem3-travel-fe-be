import api from './api';

const favoriteService = {
  // Get all favorites
  getFavorites: async () => {
    const response = await api.get('/favorites');
    return response.data;
  },

  // Check if items are favorited
  checkFavorites: async (itemIds, itemType) => {
    const params = new URLSearchParams();
    params.append('itemIds', itemIds.join(','));
    params.append('itemType', itemType);
    const response = await api.get(`/favorites/check?${params.toString()}`);
    return response.data;
  },

  // Add to favorites
  addFavorite: async (itemType, itemId) => {
    const response = await api.post('/favorites', {
      itemType,
      itemId
    });
    return response.data;
  },

  // Remove from favorites
  removeFavorite: async (favoriteId) => {
    const response = await api.delete(`/favorites/${favoriteId}`);
    return response.data;
  },

  // Remove by item
  removeByItem: async (itemType, itemId) => {
    // First get the favorite id
    const favoritesResponse = await favoriteService.getFavorites();
    if (favoritesResponse.success && favoritesResponse.data) {
      const favorite = favoritesResponse.data.find(
        f => f.itemType === itemType && f.itemId === itemId
      );
      if (favorite) {
        return await favoriteService.removeFavorite(favorite.favoriteId);
      }
    }
    return { success: true, message: 'Not in favorites' };
  }
};

export default favoriteService;
